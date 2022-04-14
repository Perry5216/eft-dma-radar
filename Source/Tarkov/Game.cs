using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace eft_dma_radar
{

    /// <summary>
    /// Class containing Game (Raid) instance.
    /// </summary>
    public class Game
    {
        private readonly ulong _unityBase;
        private GameObjectManager _gom;
        private ulong _localGameWorld;
        private LootManager _lootManager;
        private RegisteredPlayers _rgtPlayers;
        private GrenadeManager _grenadeManager;
        private ExfilManager _exfilManager;
        private volatile bool _inGame = false;
        private volatile bool _loadingLoot = false;
        private volatile bool _refreshLoot = false;
        #region Getters
        public bool InGame
        {
            get => _inGame;
        }
        public bool LoadingLoot
        {
            get => _loadingLoot;
        }
        public ReadOnlyDictionary<string, Player> Players
        {
            get => _rgtPlayers?.Players;
        }
        public ReadOnlyCollection<LootItem> Loot
        {
            get => _lootManager?.Loot;
        }
        public ReadOnlyCollection<Grenade> Grenades
        {
            get => _grenadeManager?.Grenades;
        }
        public ReadOnlyCollection<Exfil> Exfils
        {
            get => _exfilManager?.Exfils;
        }
        #endregion

        /// <summary>
        /// Game Constructor.
        /// </summary>
        public Game(ulong unityBase)
        {
            _unityBase = unityBase;
        }

        #region GameLoop
        /// <summary>
        /// Main Game Loop executed by Memory Worker Thread. Updates player list, and updates all player values.
        /// </summary>
        public void GameLoop()
        {
            try
            {
                _rgtPlayers.UpdateList(); // Check for new players, add to list
                _rgtPlayers.UpdateAllPlayers(); // Update all player locations,etc.
                UpdateMisc(); // Loot, grenades, exfils,etc.
            }
            catch (DMAShutdown)
            {
                _inGame = false;
                throw;
            }
            catch (RaidEnded)
            {
                Program.Log("Raid has ended!");
                _inGame = false;
            }
            catch (Exception ex)
            {
                Program.Log($"CRITICAL ERROR - Raid ended due to unhandled exception: {ex}");
                _inGame = false;
                throw;
            }

        }
        #endregion

        #region Methods
        /// <summary>
        /// Waits until Raid has started before returning to caller.
        /// </summary>
        public void WaitForGame()
        {
            while (true)
            {
                if (GetGOM() && GetLGW())
                {
                    Thread.Sleep(1000);
                    break;
                }
                else Thread.Sleep(1500);
            }
            Program.Log("Raid has started!");
            _inGame = true;
            Thread.Sleep(1500); // brief pause before entering main loop / loading loot
        }

        /// <summary>
        /// Helper method to locate Game World object.
        /// </summary>
        private ulong GetObjectFromList(ulong listPtr, ulong lastObjectPtr, string objectName)
        {
            var activeObject = Memory.ReadValue<BaseObject>(Memory.ReadPtr(listPtr));
            var lastObject = Memory.ReadValue<BaseObject>(Memory.ReadPtr(lastObjectPtr));

            if (activeObject.obj != 0x0)
            {
                while (activeObject.obj != 0x0 && activeObject.obj != lastObject.obj)
                {
                    var objectNamePtr = Memory.ReadPtr(activeObject.obj + Offsets.GameObject.ObjectName);
                    var objectNameStr = Memory.ReadString(objectNamePtr, 64);
                    if (objectNameStr.Contains(objectName, StringComparison.OrdinalIgnoreCase))
                    {
                        Program.Log($"Found object {objectNameStr}");
                        return activeObject.obj;
                    }

                    activeObject = Memory.ReadValue<BaseObject>(activeObject.nextObjectLink); // Read next object
                }
            }
            Program.Log($"Couldn't find object {objectName}");
            return 0;
        }

        /// <summary>
        /// Gets Game Object Manager structure.
        /// </summary>
        private bool GetGOM()
        {
            try
            {
                var addr = Memory.ReadPtr(_unityBase + Offsets.ModuleBase.GameObjectManager);
                _gom = Memory.ReadValue<GameObjectManager>(addr);
                Program.Log($"Found Game Object Manager at 0x{addr.ToString("X")}");
                return true;
            }
            catch (DMAShutdown) { throw; }
            catch (Exception ex)
            {
                throw new GameNotRunningException($"ERROR getting Game Object Manager, game may not be running: {ex}");
            }
        }

        /// <summary>
        /// Gets Local Game World address.
        /// </summary>
        private bool GetLGW()
        {
            try
            {
                ulong activeNodes = Memory.ReadPtr(_gom.ActiveNodes);
                ulong lastActiveNode = Memory.ReadPtr(_gom.LastActiveNode);
                var gameWorld = GetObjectFromList(activeNodes, lastActiveNode, "GameWorld");
                if (gameWorld == 0) throw new Exception("Unable to find GameWorld Object, likely not in raid.");
                _localGameWorld = Memory.ReadPtrChain(gameWorld, Offsets.GameWorld.To_LocalGameWorld); // Game world >> Local Game World
                var rgtPlayers = new RegisteredPlayers(Memory.ReadPtr(_localGameWorld + Offsets.LocalGameWorld.RegisteredPlayers));
                if (rgtPlayers.PlayerCount > 1) // Make sure not in hideout,etc.
                {
                    _rgtPlayers = rgtPlayers; // update ref
                    return true;
                }
                else
                {
                    Program.Log("ERROR - Local Game World does not contain players (hideout?)");
                    return false;
                }
            }
            catch (DMAShutdown) { throw; }
            catch (Exception ex)
            {
                Program.Log($"ERROR getting Local Game World: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Loot, grenades, exfils,etc.
        /// </summary>
        private void UpdateMisc()
        {
            if (_lootManager is null || _refreshLoot)
            {
                _loadingLoot = true;
                try
                {
                    var loot = new LootManager(_localGameWorld);
                    _lootManager = loot; // update ref
                    _refreshLoot = false;
                }
                catch (Exception ex)
                {
                    Program.Log($"ERROR loading LootEngine: {ex}");
                }
                _loadingLoot = false;
            }
            if (_grenadeManager is null)
            {
                try
                {
                    var grenadeManager = new GrenadeManager(_localGameWorld);
                    _grenadeManager = grenadeManager; // update ref
                }
                catch (Exception ex)
                {
                    Program.Log($"ERROR loading GrenadeManager: {ex}");
                }
            }
            else _grenadeManager.Refresh(); // refresh via internal stopwatch
            if (_exfilManager is null)
            {
                try
                {
                    var exfils = new ExfilManager(_localGameWorld);
                    _exfilManager = exfils; // update ref
                }
                catch (Exception ex)
                {
                    Program.Log($"ERROR loading ExfilController: {ex}");
                }
            }
            else _exfilManager.Refresh(); // periodically refreshes (internal stopwatch)
        }
        public void RefreshLoot()
        {
            if (!_refreshLoot)
            {
                _refreshLoot = true;
            }
        }
        #endregion
    }

    #region Exceptions
    public class GameNotRunningException : Exception
    {
        public GameNotRunningException()
        {
        }

        public GameNotRunningException(string message)
            : base(message)
        {
        }

        public GameNotRunningException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class RaidEnded : Exception
    {
        public RaidEnded()
        {
        }

        public RaidEnded(string message)
            : base(message)
        {
        }

        public RaidEnded(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    #endregion
}
