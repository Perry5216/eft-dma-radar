using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Intrinsics;

namespace eft_dma_radar
{
    public class RegisteredPlayers
    {
        private readonly ulong _base;
        private readonly ulong _listBase;
        private readonly Stopwatch _regSw = new();
        private readonly Stopwatch _healthSw = new();
        private readonly ConcurrentDictionary<string, Player> _players = new();

        private int _currentPlayerGroup = -100;

        #region Getters
        public ReadOnlyDictionary<string, Player> Players { get; }
        public int PlayerCount
        {
            get
            {
                for (int i = 0; i < 5; i++) // Re-attempt if read fails
                {
                    try
                    {
                        var count = Memory.ReadValue<int>(_base + Offsets.UnityList.Count);
                        if (count < 1 || count > 1024) throw new ArgumentOutOfRangeException();
                        return count;
                    }
                    catch { Thread.Sleep(1000); } // short delay between read attempts
                }
                return -1; // error
            }
        }
        #endregion

        /// <summary>
        /// RegisteredPlayers List Constructor.
        /// </summary>
        public RegisteredPlayers(ulong baseAddr)
        {
            _base = baseAddr;
            Players = new(_players); // update readonly ref
            _listBase = Memory.ReadPtr(_base + Offsets.UnityList.Base);
            _regSw.Start();
            _healthSw.Start();
        }

        #region UpdateList
        /// <summary>
        /// Updates the ConcurrentDictionary of 'Players'
        /// </summary>
        public void UpdateList()
        {
            try
            {
                if (_regSw.ElapsedMilliseconds < 500) return; // Update every 500ms
                var count = this.PlayerCount; // cache count
                if (count < 1 || count > 1024)
                    throw new RaidEnded();
                var registered = new HashSet<string>();
                var scatterMap = new ScatterReadMap();
                var round1 = scatterMap.AddRound();
                var round2 = scatterMap.AddRound();
                var round3 = scatterMap.AddRound();
                var round4 = scatterMap.AddRound();
                var round5 = scatterMap.AddRound();
                for (int i = 0; i < count; i++)
                {
                    var playerBase = round1.AddEntry(i,
                        0,
                        _listBase + Offsets.UnityListBase.Start + (uint)(i * 0x8),
                        typeof(ulong));
                    var playerProfile = round2.AddEntry(i, 1, playerBase,
                        typeof(ulong), 0, Offsets.Player.Profile);

                    var playerId = round3.AddEntry(i, 2, playerProfile, typeof(ulong),
                        0, Offsets.Profile.PlayerID);
                    var playerIdLen = round4.AddEntry(i, 3, playerId, typeof(int),
                        0, Offsets.UnityString.Length);
                    var playerIdStr = round5.AddEntry(i, 4, playerId, typeof(string),
                        playerIdLen, Offsets.UnityString.Value);
                    playerIdStr.SizeMult = 2; // Unity String twice the length
                }
                scatterMap.Execute(count);
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if (scatterMap.Results[i][0].Result is null
                            || scatterMap.Results[i][1].Result is null
                            || scatterMap.Results[i][4].Result is null)
                            throw new NullReferenceException("Unable to acquire Player ID due to NULL Reads!");
                        var playerBase = (ulong)scatterMap.Results[i][0].Result;
                        var playerProfile = (ulong)scatterMap.Results[i][1].Result;
                        var id = (string)scatterMap.Results[i][4].Result;
                        if (!_players.ContainsKey(id))
                        {
                            var player = new Player(id, playerBase, playerProfile); // allocate player object
                            _players.TryAdd(id, player);
                        }
                        else
                        {
                            if (_players[id].ErrorCount > 100) // Erroring out a lot? Re-Alloc
                            {
                                Program.Log($"Existing player {_players[id].Name} being removed/re-allocated due to excessive errors...");
                                var player = new Player(id, playerBase,
                                    playerProfile,
                                    _players[id].Position); // allocate new player object (w/ last known pos)
                                // Allocated OK? Swap them out!
                                _players[id] = player; // swap refs
                                Program.Log($"Player {_players[id].Name} Re-Allocated successfully.");
                            }
                            else
                            {
                                _players[id].IsActive = true;
                                _players[id].IsAlive = true;
                            }
                        }
                        registered.Add(id);
                    }
                    catch (Exception ex)
                    {
                        Program.Log($"ERROR processing RegisteredPlayer at index {i}: {ex}");
                    }
                }
                var inactivePlayers = _players.Where(x => !registered.Contains(x.Key) && x.Value.IsActive);
                foreach (var player in inactivePlayers)
                {
                    player.Value.LastUpdate = true;
                }
                _regSw.Restart();
            }
            catch (DMAShutdown) { throw; }
            catch (RaidEnded) { throw; }
            catch (Exception ex)
            {
                Program.Log($"CRITICAL ERROR - RegisteredPlayers Loop FAILED: {ex}");
            }
        }
        #endregion

        #region UpdatePlayers
        /// <summary>
        /// Updates all 'Player' values (Position,health,direction,etc.)
        /// </summary>
        public void UpdateAllPlayers()
        {
            try
            {
                var players = _players.Select(x => x.Value)
                    .Where(x => x.IsActive && x.IsAlive).ToArray();
                if (_currentPlayerGroup == -100) // Check if current player group is set
                {
                    var currentPlayer = _players.FirstOrDefault(x => x.Value.Type is PlayerType.CurrentPlayer).Value;
                    if (currentPlayer is not null)
                    {
                        _currentPlayerGroup = currentPlayer.GroupID;
                    }
                }
                bool checkHealth = _healthSw.ElapsedMilliseconds > 250;
                var scatterMap = new ScatterReadMap();
                var round1 = scatterMap.AddRound();
                for (int i = 0; i < players.Length; i++)
                {
                    var player = players[i];
                    if (player.LastUpdate) // player may be dead/exfil'd
                    {
                        var corpse = round1.AddEntry(i, 10, player.CorpseAddr, typeof(ulong));
                    }
                    else
                    {
                        if (checkHealth) for (int p = 0; p < 7; p++)
                        {
                            var health = round1.AddEntry(i, p, player.HealthEntries[p] + Offsets.HealthEntry.Value,
                                typeof(float), null);
                        }
                        var rotation = round1.AddEntry(i, 7, player.MovementContext + Offsets.MovementContext.Rotation,
                            typeof(System.Numerics.Vector2), null); // x = dir , y = pitch
                        var posAddr = player.TransformScatterReadParameters;
                        var indices = round1.AddEntry(i, 8, posAddr.Item1,
                            typeof(List<int>), posAddr.Item2);
                        indices.SizeMult = 4;
                        var vertices = round1.AddEntry(i, 9, posAddr.Item3,
                            typeof(List<Vector128<float>>), posAddr.Item4);
                        vertices.SizeMult = 16;
                    }
                }
                scatterMap.Execute(players.Length);

                for (int i = 0; i < players.Length; i++)
                {
                    var player = players[i];
                    if (_currentPlayerGroup != -100
                        && player.GroupID != -1
                        && (player.Type is PlayerType.PMC
                        || player.Type is PlayerType.PlayerScav))
                    { // Teammate check
                        if (player.GroupID == _currentPlayerGroup)
                            player.Type = PlayerType.Teammate;
                    }
                    if (player.LastUpdate) // player may be dead/exfil'd
                    {
                        var corpse = (ulong?)scatterMap.Results[i][10].Result;
                        if (corpse is not null && corpse != 0x0) // dead
                        {
                            player.IsAlive = false;
                        }
                        player.IsActive = false; // mark inactive
                        player.LastUpdate = false; // Last update processed, clear flag
                    }
                    else
                    {
                        bool p1 = true;
                        if (checkHealth)
                        {
                            var bodyParts = new object[7];
                            for (int p = 0; p < 7; p++)
                            {
                                bodyParts[p] = scatterMap.Results[i][p].Result;
                            }
                            p1 = player.SetHealth(bodyParts);
                        }
                        var rotation = scatterMap.Results[i][7].Result;
                        bool p2 = player.SetRotation(rotation);
                        var posBufs = new object[2]
                        {
                            scatterMap.Results[i][8].Result,
                            scatterMap.Results[i][9].Result
                        };
                        bool p3 = player.SetPosition(posBufs);
                        if (p1 && p2 && p3) player.ErrorCount = 0;
                        else player.ErrorCount++;
                    }
                }
                if (checkHealth) _healthSw.Restart();
            }
            catch (DMAShutdown) { throw; }
            catch (Exception ex)
            {
                Program.Log($"CRITICAL ERROR - UpdatePlayers Loop FAILED: {ex}");
            }
        }
        #endregion
    }
}
