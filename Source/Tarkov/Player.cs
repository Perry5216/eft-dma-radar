using System;
using System.Diagnostics;
using System.Numerics;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace eft_dma_radar
{
    /// <summary>
    /// Class containing Game Player Data.
    /// </summary>
    public class Player
    {
        private static Dictionary<string, string> _watchlist; // init in Static Constructor
        private static Dictionary<string, int> _groups = new();

        private readonly Stopwatch _posRefreshSw = new();
        private readonly object _posLock = new(); // sync access to this.Position (non-atomic)
        private readonly ulong _playerBase;
        private readonly ulong _playerInfo;
        private readonly GearManager _gearManager;
        private Transform _transform;

        #region PlayerProperties
        /// <summary>
        /// Player is Alive/Not Dead.
        /// </summary>
        public volatile bool IsAlive = true;
        /// <summary>
        /// Player is Active (has not exfil'd).
        /// </summary>
        public volatile bool IsActive = true;

        /// <summary>
        /// Player name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Player Level (Based on experience).
        /// </summary>
        public int Lvl { get; } = 0;
        /// <summary>
        /// Group that the player belongs to.
        /// </summary>
        public int GroupID { get; } = -1;
        /// <summary>
        /// Type of player unit.
        /// </summary>
        public PlayerType Type { get; set; }
        /// <summary>
        /// Player's current health (sum of all 7 body parts).
        /// </summary>
        public int Health { get; private set; } = -1;
        private Vector3 _pos = new Vector3(0, 0, 0); // backing field
        /// <summary>
        /// Player's Unity Position in Local Game World.
        /// </summary>
        public Vector3 Position // 96 bits, cannot set atomically
        {
            get
            {
                lock (_posLock)
                {
                    return _pos;
                }
            }
            private set
            {
                lock (_posLock)
                {
                    _pos = value;
                }
            }
        }
        /// <summary>
        /// Player's Rotation (direction/pitch) in Local Game World.
        /// 90 degree offset ~already~ applied to account for 2D-Map orientation.
        /// </summary>
        public Vector2 Rotation { get; private set; } = new Vector2(0, 0); // 64 bits will be atomic
        /// <summary>
        /// (PMC ONLY) Player's Gear Loadout.
        /// Key = Slot Name, Value = Item 'Long Name' in Slot
        /// </summary>
        public ReadOnlyDictionary<string, string> Gear
        {
            get => _gearManager?.Gear;
        }
        /// <summary>
        /// If 'true', Player object is no longer in the RegisteredPlayers list.
        /// Will be checked if dead/exfil'd on next loop.
        /// </summary>
        public bool LastUpdate { get; set; } = false;
        /// <summary>
        /// Consecutive number of errors that this Player object has 'errored out' while updating.
        /// </summary>
        public int ErrorCount { get; set; } = 0;
        #endregion

        #region Getters
        private static readonly ConcurrentStack<PlayerHistoryEntry> _history = new(); // backing field
        /// <summary>
        /// Contains history of PMC Players that are allocated during program runtime.
        /// </summary>
        public static ListViewItem[] History
        {
            get => _history.Select(x => x.View).ToArray();
        }
        public ulong[] HealthEntries { get; }
        public ulong MovementContext { get; }
        public ulong CorpseAddr
        {
            get => _playerBase + Offsets.Player.Corpse;
        }
        /// <summary>
        /// IndicesAddress -> IndicesSize -> VerticesAddress -> VerticesSize
        /// </summary>
        public Tuple<ulong, int, ulong, int> TransformScatterReadParameters
        {
            get => _transform?.GetScatterReadParameters() ?? new Tuple<ulong, int, ulong, int>(0, 0, 0, 0);
        }
        #endregion

        #region Static_Constructor
        static Player()
        {
            LoadWatchlist();
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Player Constructor.
        /// </summary>
        public Player(string id, ulong playerBase, ulong playerProfile, Vector3? pos = null)
        {
            try
            {
                _playerBase = playerBase;
                if (pos is not null)
                {
                    this.Position = (Vector3)pos; // Populate provided Position (usually only for a re-alloc)
                }
                _playerInfo = Memory.ReadPtr(playerProfile + Offsets.Profile.PlayerInfo);
                var healthEntriesList = Memory.ReadPtrChain(_playerBase, 
                    new uint[] { Offsets.Player.HealthController , 
                        Offsets.HealthController.To_HealthEntries[0], 
                        Offsets.HealthController.To_HealthEntries[1] });
                HealthEntries = new ulong[7];
                for (uint i = 0; i < 7; i++)
                {                                                                   // 0x30, start at 3rd entry
                    HealthEntries[i] = Memory.ReadPtrChain(healthEntriesList, new uint[] { 0x30 + (i * 0x18), Offsets.HealthEntry.Value });
                }
                MovementContext = Memory.ReadPtr(_playerBase + Offsets.Player.MovementContext);
                var transform_internal = Memory.ReadPtrChain(_playerBase, Offsets.Player.To_TransformInternal);
                _transform = new Transform(transform_internal, true);
                var isLocalPlayer = Memory.ReadValue<bool>(_playerBase + Offsets.Player.IsLocalPlayer);
                if (isLocalPlayer)
                {
                    GroupID = GetGroupID();
                    Type = PlayerType.CurrentPlayer;
                }
                else
                {
                    var playerSide = Memory.ReadValue<int>(_playerInfo + Offsets.PlayerInfo.PlayerSide); // Scav, PMC, etc.
                    if (playerSide == 0x4)
                    {
                        var regDate = Memory.ReadValue<int>(_playerInfo + Offsets.PlayerInfo.RegDate); // Bots wont have 'reg date'
                        if (regDate == 0)
                        {
                            var settings = Memory.ReadPtr(_playerInfo + Offsets.PlayerInfo.Settings);
                            var roleFlag = (WildSpawnType)Memory.ReadValue<int>(settings + Offsets.PlayerSettings.Role);
                            var role = roleFlag.GetRole();
                            Name = role.Name;
                            Type = role.Type;
                        }
                        else
                        {
                            Name = "Scav";
                            GroupID = GetGroupID();
                            Lvl = GetPlayerLevel();
                            Type = PlayerType.PlayerScav;
                        }
                    }
                    else if (playerSide == 0x1 || playerSide == 0x2)
                    {
                        try { _gearManager = new GearManager(playerBase); } catch { } // Don't fail allocation - low prio
                        GroupID = GetGroupID();
                        Lvl = GetPlayerLevel();
                        var namePtr = Memory.ReadPtr(_playerInfo + Offsets.PlayerInfo.PlayerName);
                        Name = Memory.ReadUnityString(namePtr);
                        if (_watchlist.TryGetValue(id, out var reason)) // player is on watchlist
                        {
                            Type = PlayerType.WatchlistPMC;
                            var entry = new PlayerHistoryEntry(id, $"** WATCHLIST ALERT for {Name}  L:{Lvl}, G:{GroupID}, @{DateTime.Now.ToLongTimeString()} ~~ Reason: {reason}");
                            _history.Push(entry);
                        }
                        else // log normally to PMC History
                        {
                            Type = PlayerType.PMC;
                            var entry = new PlayerHistoryEntry(id, $"{Name}  L:{Lvl}, G:{GroupID}, @{DateTime.Now.ToLongTimeString()}");
                            _history.Push(entry);
                        }
                    }
                    else Type = PlayerType.Default;
                }
                Program.Log($"Player {Name} allocated.");
                    
            }
            catch (Exception ex)
            {
                throw new DMAException($"ERROR during Player constructor for base addr 0x{playerBase.ToString("X")}", ex);
            }
        }
        #endregion

        #region Setters
        /// <summary>
        /// Set player health.
        /// </summary>
        public bool SetHealth(object[] obj)
        {
            try
            {
                float totalHealth = 0;
                for (uint i = 0; i < HealthEntries.Length; i++)
                {
                    float health = (float)obj[i]; // unbox
                    totalHealth += health;
                }
                this.Health = (int)Math.Round(totalHealth);
                return true;
            }
            catch (Exception ex)
            {
                Program.Log($"ERROR getting Player {Name} Health: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Set player rotation (Direction/Pitch)
        /// </summary>
        public bool SetRotation(object obj)
        {
            try
            {
                Vector2 rotation = (Vector2)obj; // unbox
                Vector2 result;
                rotation.X -= 90; // degs offset
                if (rotation.X < 0) rotation.X += 360f; // handle if neg

                if (rotation.X < 0) result.X = 360f + rotation.X;
                else result.X = rotation.X;
                if (rotation.Y < 0) result.Y = 360f + rotation.Y;
                else result.Y = rotation.Y;
                this.Rotation = result;

                return true;
            }
            catch (Exception ex)
            {
                Program.Log($"ERROR getting Player {Name} Rotation: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Set player position (Vector3 X,Y,Z)
        /// </summary>
        public bool SetPosition(object[] obj)
        {
            try
            {
                if (obj is null) throw new NullReferenceException();
                this.Position = _transform.GetPosition(obj);
                return true;
            }
            catch (Exception ex)
            {
                Program.Log($"ERROR getting Player {Name} Position: {ex}");
                if (!_posRefreshSw.IsRunning) _posRefreshSw.Start();
                else if (_posRefreshSw.ElapsedMilliseconds < 250) // Rate limit attempts on getting pos to prevent stutters
                {
                    return false;
                }
                try
                {
                    var transform_internal = Memory.ReadPtrChain(_playerBase, Offsets.Player.To_TransformInternal);
                    var transform = new Transform(transform_internal, true);
                    _transform = transform;
                    Program.Log($"Player {Name} obtained new Position Transform OK.");
                }
                catch { } finally { _posRefreshSw.Restart(); }
                return false;
            }
        }
        #endregion

        #region Methods
        private int GetPlayerLevel()
        {
            var exp = Memory.ReadValue<int>(_playerInfo + Offsets.PlayerInfo.Experience);
            return Misc.EXP_TABLE.Where(x => x.Key > exp).FirstOrDefault().Value - 1;
        }

        private int GetGroupID()
        {
            try
            {
                var grpPtr = Memory.ReadPtr(_playerInfo + Offsets.PlayerInfo.GroupID);
                var grp = Memory.ReadUnityString(grpPtr);
                _groups.TryAdd(grp, _groups.Count);
                return _groups[grp];
            }
            catch { return -1; } // will return null if Solo / Don't have a team
        }
        /// <summary>
        /// Resets/Updates 'static' assets in preparation for a new game/raid instance.
        /// </summary>
        public static void Reset()
        {
            _groups = new();
            _history.Push(new PlayerHistoryEntry(null, "---NEW GAME---")); // Insert separator in PMC History Log
        }

        public static void LoadWatchlist()
        {
            _watchlist = new(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists("playerWatchlist.txt"))
            {
                File.WriteAllText("playerWatchlist.txt",
                    "PlayerBsgID : Watchlist reason/comment here (one entry per line)");
            }
            else
            {
                var lines = File.ReadAllLines("playerWatchlist.txt");
                foreach (var line in lines)
                {
                    var split = line.Split(':'); // remove single delimiting ':' character
                    if (split.Length == 2)
                    {
                        var id = split[0].Trim();
                        var reason = split[1].Trim();
                        _watchlist.TryAdd(id, reason);
                    }
                }
            }
        }
        #endregion
    }
}
