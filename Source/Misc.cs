using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using SkiaSharp;

namespace eft_dma_radar
{
    #region Misc
    public static class Misc
    {

        public static readonly string[] CONTAINERS = new string[] { "body", "XXXcap", "Ammo_crate_Cap", "Grenade_box_Door", "Medical_Door", "Toolbox_Door", "card_file_box", "cover_", "lootable", "scontainer_Blue_Barrel_Base_Cap", "scontainer_wood_CAP", "suitcase_plastic_lootable_open", "weapon_box_cover", "container_crete_04_COLLIDER(1)" };

        public static readonly Dictionary<int, int> EXP_TABLE = new Dictionary<int, int>
        {
            {0, 1},
            {1000, 2},
            {4017, 3},
            {8432, 4},
            {14256, 5},
            {21477, 6},
            {30023, 7},
            {39936, 8},
            {51204, 9},
            {63723, 10},
            {77563, 11},
            {92713, 12},
            {111881, 13},
            {134674, 14},
            {161139, 15},
            {191417, 16},
            {225194, 17},
            {262366, 18},
            {302484, 19},
            {345751, 20},
            {391649, 21},
            {440444, 22},
            {492366, 23},
            {547896, 24},
            {609066, 25},
            {675913, 26},
            {748474, 27},
            {826786, 28},
            {910885, 29},
            {1000809, 30},
            {1096593, 31},
            {1198275, 32},
            {1309251, 33},
            {1429580, 34},
            {1559321, 35},
            {1698532, 36},
            {1847272, 37},
            {2005600, 38},
            {2173575, 39},
            {2351255, 40},
            {2538699, 41},
            {2735966, 42},
            {2946585, 43},
            {3170637, 44},
            {3408202, 45},
            {3659361, 46},
            {3924195, 47},
            {4202784, 48},
            {4495210, 49},
            {4801553, 50},
            {5121894, 51},
            {5456314, 52},
            {5809667, 53},
            {6182063, 54},
            {6573613, 55},
            {6984426, 56},
            {7414613, 57},
            {7864284, 58},
            {8333549, 59},
            {8831052, 60},
            {9360623, 61},
            {9928578, 62},
            {10541848, 63},
            {11206300, 64},
            {11946977, 65},
            {12789143, 66},
            {13820522, 67},
            {15229487, 68},
            {17206065, 69},
            {19706065, 70},
            {22706065, 71},
            {26206065, 72},
            {30206065, 73},
            {34706065, 74},
            {39706065, 75},
        };
    }
    #endregion

    #region ExtensionMethods
    public static class Extensions
    {
        public static void Reset(this System.Timers.Timer t)
        {
            t.Stop();
            t.Start();
        }
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static AIRole GetRole(this WildSpawnType type)
        {
            switch (type)
            {
                case WildSpawnType.marksman:
                    return new AIRole()
                    {
                        Name = "Sniper",
                        Type = PlayerType.AIScav
                    };
                case WildSpawnType.assault:
                    return new AIRole()
                    {
                        Name = "Scav",
                        Type = PlayerType.AIScav
                    };
                case WildSpawnType.bossTest:
                    return new AIRole()
                    {
                        Name = "bossTest",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.bossBully:
                    return new AIRole()
                    {
                        Name = "Reshala",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.followerTest:
                    return new AIRole()
                    {
                        Name = "followerTest",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.followerBully:
                    return new AIRole()
                    {
                        Name = "Guard",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.bossKilla:
                    return new AIRole()
                    {
                        Name = "Killa",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.bossKojaniy:
                    return new AIRole()
                    {
                        Name = "Shturman",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.followerKojaniy:
                    return new AIRole()
                    {
                        Name = "Guard",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.pmcBot:
                    return new AIRole()
                    {
                        Name = "Raider",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.cursedAssault:
                    return new AIRole()
                    {
                        Name = "Scav",
                        Type = PlayerType.AIScav
                    };
                case WildSpawnType.bossGluhar:
                    return new AIRole()
                    {
                        Name = "Gluhar",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.followerGluharAssault:
                    return new AIRole()
                    {
                        Name = "Assault",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.followerGluharSecurity:
                    return new AIRole()
                    {
                        Name = "Security",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.followerGluharScout:
                    return new AIRole()
                    {
                        Name = "Scout",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.followerGluharSnipe:
                    return new AIRole()
                    {
                        Name = "Sniper",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.followerSanitar:
                    return new AIRole()
                    {
                        Name = "Guard",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.bossSanitar:
                    return new AIRole()
                    {
                        Name = "Sanitar",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.test:
                    return new AIRole()
                    {
                        Name = "test",
                        Type = PlayerType.AIScav
                    };
                case WildSpawnType.assaultGroup:
                    return new AIRole()
                    {
                        Name = "assaultGroup",
                        Type = PlayerType.AIScav
                    };
                case WildSpawnType.sectantWarrior:
                    return new AIRole()
                    {
                        Name = "Cultist",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.sectantPriest:
                    return new AIRole()
                    {
                        Name = "Priest",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.bossTagilla:
                    return new AIRole()
                    {
                        Name = "Tagilla",
                        Type = PlayerType.AIBoss
                    };
                case WildSpawnType.followerTagilla:
                    return new AIRole()
                    {
                        Name = "Guard",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.exUsec:
                    return new AIRole()
                    {
                        Name = "Rogue",
                        Type = PlayerType.AIRaider
                    };
                case WildSpawnType.gifter:
                    return new AIRole()
                    {
                        Name = "SANTA",
                        Type = PlayerType.AIScav
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endregion

    #region Misc_Classes_Enums
    /// <summary>
    /// Custom Debug Stopwatch class to measure performance.
    /// </summary>
    public class DebugStopwatch
    {
        private readonly Stopwatch _sw;
        private readonly string _name;

        /// <summary>
        /// Constructor. Starts stopwatch.
        /// </summary>
        /// <param name="name">(Optional) Name of stopwatch.</param>
        public DebugStopwatch(string name = null)
        {
            _name = name;
            _sw = new Stopwatch();
            _sw.Start();
        }

        /// <summary>
        /// End stopwatch and display result to Debug Output.
        /// </summary>
        public void Stop()
        {
            _sw.Stop();
            TimeSpan ts = _sw.Elapsed;
            Debug.WriteLine($"{_name} Stopwatch Runtime: {ts.Ticks} ticks");
        }
    }

    /// <summary>
    /// Defines Player Unit Type (Player,PMC,Scav,etc.)
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// Default value if a type cannot be established.
        /// </summary>
        Default,
        /// <summary>
        /// LocalPlayer (the person running the Radar)
        /// </summary>
        CurrentPlayer,
        /// <summary>
        /// Teammate of CurrentPlayer
        /// </summary>
        Teammate,
        /// <summary>
        /// Hostile/Enemy PMC.
        /// </summary>
        PMC,
        /// <summary>
        /// Normal AI Bot Scav.
        /// </summary>
        AIScav,
        /// <summary>
        /// Difficult AI Raider.
        /// </summary>
        AIRaider,
        /// <summary>
        /// Difficult AI Boss.
        /// </summary>
        AIBoss,
        /// <summary>
        /// Player controlled Scav.
        /// </summary>
        PlayerScav,
        /// <summary>
        /// Hostile/Enemy PMC that is on the watchlist.
        /// </summary>
        WatchlistPMC
    }

    /// <summary>
    /// Defines map position for the 2D Map.
    /// </summary>
    public struct MapPosition
    {
        public MapPosition()
        {
            X = 0;
            Y = 0;
            Height = 0;
        }

        /// <summary>
        /// X coordinate on Bitmap.
        /// </summary>
        public float X;
        /// <summary>
        /// Y coordinate on Bitmap.
        /// </summary>
        public float Y;
        /// <summary>
        /// Unit 'height' as determined by Vector3.Z
        /// </summary>
        public float Height;

        /// <summary>
        /// Get exact player location (with optional X,Y offsets).
        /// </summary>
        public SKPoint GetPoint(int xOff = 0, int yOff = 0)
        {
            return new SKPoint(X + xOff, Y + yOff);
        }

        /// <summary>
        /// Gets up arrow where loot is. IDisposable.
        /// </summary>
        public SKPath GetUpArrow(int size = 6)
        {
            SKPath path = new SKPath();
            path.MoveTo(X, Y);
            path.LineTo(X - size, Y + size);
            path.LineTo(X + size, Y + size);
            path.Close();

            return path;
        }

        /// <summary>
        /// Gets down arrow where loot is. IDisposable.
        /// </summary>
        public SKPath GetDownArrow(int size = 6)
        {
            SKPath path = new SKPath();
            path.MoveTo(X, Y);
            path.LineTo(X - size, Y - size);
            path.LineTo(X + size, Y - size);
            path.Close();

            return path;
        }
    }

    /// <summary>
    /// Defines a Map for use in the GUI.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Name of map (Ex: Customs)
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 'MapConfig' class instance
        /// </summary>
        public readonly MapConfig ConfigFile;
        /// <summary>
        /// File path to Map .JSON Config
        /// </summary>
        public readonly string ConfigFilePath;

        public Map(string name, MapConfig config, string configPath)
        {
            Name = name;
            ConfigFile = config;
            ConfigFilePath = configPath;
        }
    }

    /// <summary>
    /// Contains multiple map parameters used by the GUI.
    /// </summary>
    public struct MapParameters
    {
        /// <summary>
        /// Contains the 'index' of which map layer to display.
        /// For example: Labs has 3 floors, so there is a Bitmap image for 'each' floor.
        /// Index is dependent on LocalPlayer height.
        /// </summary>
        public int MapLayerIndex;
        /// <summary>
        /// Rectangular 'zoomed' bounds of the Bitmap to display.
        /// </summary>
        public SKRect Bounds;
        /// <summary>
        /// Regular -> Zoomed 'X' Scale correction.
        /// </summary>
        public float XScale;
        /// <summary>
        /// Regular -> Zoomed 'Y' Scale correction.
        /// </summary>
        public float YScale;
    }

    /// <summary>
    /// Defines a .JSON Map Config File
    /// </summary>
    public class MapConfig
    {
        [JsonIgnore]
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
        /// <summary>
        /// Bitmap 'X' Coordinate of map 'Origin Location' (where Unity X is 0).
        /// </summary>
        [JsonPropertyName("x")]
        public float X { get; set; }
        /// <summary>
        /// Bitmap 'Y' Coordinate of map 'Origin Location' (where Unity Y is 0).
        /// </summary>
        [JsonPropertyName("y")]
        public float Y { get; set; }
        /// <summary>
        /// Unused.
        /// </summary>
        [JsonPropertyName("z")]
        public float Z { get; set; }
        /// <summary>
        /// Arbitrary scale value to align map scale between the Bitmap and Game Coordinates.
        /// </summary>
        [JsonPropertyName("scale")]
        public float Scale { get; set; }
        /// <summary>
        /// * This List contains the path to the map file(s), and a minimum height (Z) value.
        /// * Each tuple consists of Item1: (float)MIN_HEIGHT, Item2: (string>MAP_PATH
        /// * This list will be iterated backwards, and if the current player height (Z) is above the float
        /// value, then that map layer will be drawn. This will allow having different bitmaps at different
        /// heights.
        /// * If using only a single map (no layers), set the float value to something very low like -100.
        /// </summary>
        [JsonPropertyName("maps")]
        public List<Tuple<float, string>> Maps { get; set; }

        /// <summary>
        /// Loads map.json config file.
        /// </summary>
        /// <param name="file">Map Config .JSON file (ex: customs.json)</param>
        /// <returns></returns>
        public static MapConfig LoadFromFile(string file)
        {
            var json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<MapConfig>(json);
        }

        /// <summary>
        /// Saves map.json config file.
        /// </summary>
        /// <param name="file">Map Config .JSON file (ex: customs.json)</param>
        /// <returns></returns>
        public void Save(Map map)
        {
            var json = JsonSerializer.Serialize<MapConfig>(this, _jsonOptions);
            File.WriteAllText(map.ConfigFilePath, json);
        }
    }

    /// <summary>
    /// Global Program Configuration (Config.json)
    /// </summary>
    public class Config
    {
        [JsonIgnore]
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
        /// <summary>
        /// Enables Vertical Sync in GUI Render.
        /// </summary>
        [JsonPropertyName("vsyncEnabled")]
        public bool Vsync { get; set; }
        /// <summary>
        /// Player/Teammates Aimline Length (Max: 1000)
        /// </summary>
        [JsonPropertyName("playerAimLine")]
        public int PlayerAimLineLength { get; set; }
        /// <summary>
        /// Last used 'Zoom' level.
        /// </summary>
        [JsonPropertyName("defaultZoom")]
        public int DefaultZoom { get; set; }
        /// <summary>
        /// Enables loot output on map.
        /// </summary>
        [JsonPropertyName("lootEnabled")]
        public bool LootEnabled { get; set; }
        /// <summary>
        /// Enables Aimview window in Main Window.
        /// </summary>
        [JsonPropertyName("aimviewEnabled")]
        public bool AimViewEnabled { get; set; }
        /// <summary>
        /// Enables logging output to 'log.txt'
        /// </summary>
        [JsonPropertyName("loggingEnabled")]
        public bool LoggingEnabled { get; set; }
        /// <summary>
        /// Max game distance to render targets in Aimview, 
        /// and to display dynamic aimlines between two players.
        /// </summary>
        [JsonPropertyName("maxDistance")]
        public float MaxDistance { get; set; }
        /// <summary>
        /// 'Field of View' in degrees to display targets in the Aimview window.
        /// </summary>
        [JsonPropertyName("aimviewFOV")]
        public float AimViewFOV { get; set; }
        /// <summary>
        /// Minimum loot value (rubles) to display 'normal loot' on map.
        /// </summary>
        [JsonPropertyName("minLootValue")]
        public int MinLootValue { get; set; }
        /// <summary>
        /// Minimum loot value (rubles) to display 'important loot' on map.
        /// </summary>
        [JsonPropertyName("minImportantLootValue")]
        public int MinImportantLootValue { get; set; }

        public Config()
        {
            Vsync = true;
            PlayerAimLineLength = 10;
            DefaultZoom = 100;
            LootEnabled = true;
            AimViewEnabled = false;
            LoggingEnabled = false;
            MaxDistance = 300;
            AimViewFOV = 30;
            MinLootValue = 50000;
            MinImportantLootValue = 300000;
        }

        /// <summary>
        /// Attempt to load Config.json
        /// </summary>
        /// <param name="config">'Config' instance to populate.</param>
        /// <returns></returns>
        public static bool TryLoadConfig(out Config config)
        {
            try
            {
                if (!File.Exists("Config.json")) throw new FileNotFoundException("Config.json does not exist!");
                var json = File.ReadAllText("Config.json");
                config = JsonSerializer.Deserialize<Config>(json);
                return true;
            }
            catch
            {
                config = null;
                return false;
            }
        }
        /// <summary>
        /// Save to Config.json
        /// </summary>
        /// <param name="config">'Config' instance</param>
        public static void SaveConfig(Config config)
        {
            var json = JsonSerializer.Serialize<Config>(config, _jsonOptions);
            File.WriteAllText("Config.json", json);
        }
    }

    /// <summary>
    /// Top level object defining a scatter read operation. Create one of these in a local context.
    /// </summary>
    public class ScatterReadMap
    {
        private readonly List<ScatterReadRound> Rounds = new();
        /// <summary>
        /// Contains results from Scatter Read after Execute() is performed. First key is Index, Second Key ID.
        /// </summary>
        public Dictionary<int, Dictionary<int, ScatterReadEntry>> Results = new();

        /// <summary>
        /// Executes Scatter Read operation as defined per the map.
        /// </summary>
        public void Execute(int indexCount)
        {
            for (int i = 0; i < indexCount; i++) // Add dict for each index
            {
                Results.Add(i, new Dictionary<int, ScatterReadEntry>());
            }
            foreach (var round in Rounds)
            {
                round.Run();
            }
        }
        /// <summary>
        /// Add scatter read rounds to the operation. Each round is a successive scatter read, you may need multiple
        /// rounds if you have reads dependent on earlier scatter reads result(s).
        /// </summary>
        /// <returns>ScatterReadRound object.</returns>
        public ScatterReadRound AddRound()
        {
            var round = new ScatterReadRound(this);
            Rounds.Add(round);
            return round;
        }
    }

    /// <summary>
    /// Defines a scatter read round. Each round will execute a single scatter read. If you have reads that
    /// are dependent on previous reads (chained pointers for example), you may need multiple rounds.
    /// </summary>
    public class ScatterReadRound
    {
        private readonly ScatterReadMap _map;
        private readonly List<ScatterReadEntry> Entries = new();
        public ScatterReadRound(ScatterReadMap map)
        {
            _map = map;
        }

        /// <summary>
        /// Adds a single Scatter Read Entry.
        /// </summary>
        /// <param name="index">For loop index this is associated with.</param>
        /// <param name="id">Random ID number to identify the entry's purpose.</param>
        /// <param name="addr">Address to read from (you can pass a ScatterReadEntry from an earlier round, 
        /// and it will use the result).</param>
        /// <param name="type">Type of object to read.</param>
        /// <param name="size">Size of oject to read (ONLY for reference types, value types get size from
        /// Type). You canc pass a ScatterReadEntry from an earlier round and it will use the Result.</param>
        /// <param name="offset">Optional offset to add to address (usually in the event that you pass a
        /// ScatterReadEntry to the Addr field).</param>
        /// <returns></returns>
        public ScatterReadEntry AddEntry(int index, int id, object addr, Type type, object size = null, uint offset = 0x0)
        {
            if (size is null) size = (int)0;
            var entry = new ScatterReadEntry()
            {
                Index = index,
                Id = id,
                Addr = addr,
                Type = type,
                Size = size,
                Offset = offset
            };
            Entries.Add(entry);
            return entry;
        }

        /// <summary>
        /// Internal use only do not use.
        /// </summary>
        public void Run()
        {
            Memory.ReadScatter(Entries.ToArray(), _map.Results);
        }
    }

    /// <summary>
    /// Single scatter read entry. Use ScatterReadRound.AddEntry() to construct this class.
    /// </summary>
    public class ScatterReadEntry
    {
        /// <summary>
        /// for loop index this is associated with
        /// </summary>
        public int Index;
        /// <summary>
        /// Random identifier code (1 = PlayerBase, 2 = PlayerProfile, etc.)
        /// </summary>
        public int Id;
        /// <summary>
        /// Can be an ulong or another ScatterReadEntry
        /// </summary>
        public object Addr = (ulong)0x0;
        /// <summary>
        /// Offset amount to be added to Address.
        /// </summary>
        public uint Offset = 0x0;
        /// <summary>
        /// Defines the type. For value types is also used to determine the size.
        /// </summary>
        public Type Type;
        /// <summary>
        /// Can be an int32 or another ScatterReadEntry
        /// </summary>
        public object Size;
        /// <summary>
        /// Multiplies size by this value (ex: unity strings *2). Default: 1
        /// </summary>
        public int SizeMult = 1;
        /// <summary>
        /// Result is stored here, must cast to unbox.
        /// </summary>
        public object Result;
    }

    /// <summary>
    /// Defines 'type' of AI Bot as determined by reading Offsets.PlayerSettings.Role
    /// </summary>
    public enum WildSpawnType : int // EFT.WildSpawnType
    {
        /// <summary>
        /// Sniper Scav.
        /// </summary>
        marksman = 1,

        /// <summary>
        /// Regular Scav.
        /// </summary>
        assault = 2,

        /// <summary>
        /// ???
        /// </summary>
        bossTest = 4,

        /// <summary>
        /// Reshala
        /// </summary>
        bossBully = 8,

        /// <summary>
        /// ???
        /// </summary>
        followerTest = 16,

        /// <summary>
        /// Reshala Guard.
        /// </summary>
        followerBully = 32,

        /// <summary>
        /// Killa
        /// </summary>
        bossKilla = 64,

        /// <summary>
        /// Shturman
        /// </summary>
        bossKojaniy = 128,

        /// <summary>
        /// Shturman Guard.
        /// </summary>
        followerKojaniy = 256,

        /// <summary>
        /// AI Raider
        /// </summary>
        pmcBot = 512,

        /// <summary>
        /// Normal Scav (cursed)
        /// </summary>
        cursedAssault = 1024,

        /// <summary>
        /// Gluhar
        /// </summary>
        bossGluhar = 2048,

        /// <summary>
        /// Gluhar Guard (Assault)
        /// </summary>
        followerGluharAssault = 4096,

        /// <summary>
        /// Gluhar Guard (Security)
        /// </summary>
        followerGluharSecurity = 8192,

        /// <summary>
        /// Gluhar Guard (Scout)
        /// </summary>
        followerGluharScout = 16384,

        /// <summary>
        /// Gluhar Guard (Sniper)
        /// </summary>
        followerGluharSnipe = 32768,

        /// <summary>
        /// Sanitar Guard
        /// </summary>
        followerSanitar = 65536,

        /// <summary>
        /// Sanitar
        /// </summary>
        bossSanitar = 131072,

        /// <summary>
        /// ???
        /// </summary>
        test = 262144,

        /// <summary>
        /// ???
        /// </summary>
        assaultGroup = 524288,

        /// <summary>
        /// Cultist
        /// </summary>
        sectantWarrior = 1048576,

        /// <summary>
        /// Cultist Priest (Boss)
        /// </summary>
        sectantPriest = 2097152,

        /// <summary>
        /// Tagilla
        /// </summary>
        bossTagilla = 4194304,

        /// <summary>
        /// Tagilla Guard?
        /// </summary>
        followerTagilla = 8388608,

        /// <summary>
        /// USEC Rogues
        /// </summary>
        exUsec = 16777216,

        /// <summary>
        /// Santa
        /// </summary>
        gifter = 33554432
    };

    /// <summary>
    /// Represents a PMC in the PMC History log.
    /// </summary>
    public class PlayerHistoryEntry
    {
        private readonly string _id;
        private readonly ListViewItem _view;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Player BSG ID.</param>
        /// <param name="entry">Full History log entry.</param>
        public PlayerHistoryEntry(string id, string entry)
        {
            _id = id;
            var view = new ListViewItem(
            new string[2]
            {
                entry,
                id
            });
            view.Tag = this; // Store ref to this object
            _view = view;
        }
        
        /// <summary>
        /// For insertion into a ListView control.
        /// </summary>
        public ListViewItem View
        {
            get => _view;
        }

        /// <summary>
        /// Returns player ID.
        /// </summary>
        public override string ToString()
        {
            return _id;
        }
    }

    /// <summary>
    /// Defines Role for an AI Bot Player.
    /// </summary>
    public struct AIRole
    {
        /// <summary>
        /// Name of Bot Player.
        /// </summary>
        public string Name;
        /// <summary>
        /// Type of Bot Player.
        /// </summary>
        public PlayerType Type;
    }
    #endregion
}
