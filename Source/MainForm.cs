using System;
using SkiaSharp;
using System.Diagnostics;
using System.Numerics;
using SkiaSharp.Views.Desktop;
using System.Text;
using System.Collections.ObjectModel;

namespace eft_dma_radar
{
    public partial class MainForm : Form
    {
        private readonly SKGLControl _mapCanvas;
        private readonly SKGLControl _aimView;
        private readonly Stopwatch _fpsWatch = new Stopwatch();
        private readonly object _renderLock = new object();
        private readonly System.Timers.Timer _mapChangeTimer = new System.Timers.Timer(900);
        private readonly List<Map> _allMaps = new(); // Contains all maps from \\Maps folder
        private readonly Config _config;

        private int _fps = 0;
        private int _mapIndex = 0;
        private Map _currentMap; // Current Selected Map
        private SKBitmap[] _loadedMap;
        private MapPosition _mapPanPosition = new();
        #region Getters
        /// <summary>
        /// Radar has found Escape From Tarkov process and is ready.
        /// </summary>
        private bool Ready
        {
            get => Memory.Ready;
        }
        /// <summary>
        /// Radar has found Local Game World and LocalPlayer is 'In Game'.
        /// </summary>
        private bool InGame
        {
            get => Memory.InGame;
        }
        /// <summary>
        /// LocalPlayer (who is running Radar) 'Player' object.
        /// </summary>
        private Player CurrentPlayer
        {
            get => Memory.Players?.FirstOrDefault(x => x.Value.Type is PlayerType.CurrentPlayer).Value;
        }
        /// <summary>
        /// All Players in Local Game World (including dead/exfil'd) 'Player' collection.
        /// </summary>
        private ReadOnlyDictionary<string, Player> AllPlayers
        {
            get => Memory.Players;
        }
        /// <summary>
        /// Contains all loot in Local Game World that matches the filter.
        /// </summary>
        private ReadOnlyCollection<LootItem> Loot
        {
            get => Memory.Loot;
        }
        /// <summary>
        /// Contains all 'Hot' grenades in Local Game World, and their position(s).
        /// </summary>
        private ReadOnlyCollection<Grenade> Grenades
        {
            get => Memory.Grenades;
        }
        /// <summary>
        /// Radar is in the process of loading loot. Radar may be paused during this operation.
        /// </summary>
        private bool LoadingLoot
        {
            get => Memory.LoadingLoot;
        }
        /// <summary>
        /// Contains all 'Exfils' in Local Game World, and their status/position(s).
        /// </summary>
        private ReadOnlyCollection<Exfil> Exfils
        {
            get => Memory.Exfils;
        }
        #endregion

        #region Paints
        private readonly SKPaint _mapPaint = new SKPaint()
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintGreen = new SKPaint()
        {
            Color = SKColors.Green,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintLtGreen = new SKPaint()
        {
            Color = SKColors.LimeGreen,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textLtGreen = new SKPaint()
        {
            Color = SKColors.LimeGreen,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintRed = new SKPaint()
        {
            Color = SKColors.Red,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textRed = new SKPaint()
        {
            Color = SKColors.Red,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintPink = new SKPaint()
        {
            Color = SKColors.HotPink,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textPink = new SKPaint()
        {
            Color = SKColors.HotPink,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintYellow = new SKPaint()
        {
            Color = SKColors.Yellow,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textYellow = new SKPaint()
        {
            Color = SKColors.Yellow,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintYellow2 = new SKPaint()
        {
            Color = SKColor.Parse("ffc70f"),
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textYellow2 = new SKPaint()
        {
            Color = SKColor.Parse("ffc70f"),
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintViolet = new SKPaint()
        {
            Color = SKColors.Fuchsia,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textViolet = new SKPaint()
        {
            Color = SKColors.Fuchsia,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintWhite = new SKPaint()
        {
            Color = SKColors.White,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _textWhite = new SKPaint()
        {
            Color = SKColors.White,
            IsStroke = false,
            TextSize = 12,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintBlack = new SKPaint()
        {
            Color = SKColors.Black,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintLoot = new SKPaint()
        {
            Color = SKColors.WhiteSmoke,
            StrokeWidth = 3,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintImportantLoot = new SKPaint()
        {
            Color = SKColors.Turquoise,
            StrokeWidth = 3,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _lootText = new SKPaint()
        {
            Color = SKColors.WhiteSmoke,
            IsStroke = false,
            TextSize = 13,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial"),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _importantLootText = new SKPaint()
        {
            Color = SKColors.Turquoise,
            IsStroke = false,
            TextSize = 13,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial"),
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintAimViewCrosshair = new SKPaint()
        {
            Color = SKColors.White,
            StrokeWidth = 1,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };
        private readonly SKPaint _paintRedAimView = new SKPaint()
        {
            Color = SKColors.Red,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintPinkAimView = new SKPaint()
        {
            Color = SKColors.HotPink,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintGreenAimView = new SKPaint()
        {
            Color = SKColors.LimeGreen,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintVioletAimView = new SKPaint()
        {
            Color = SKColors.Fuchsia,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintYellowAimView = new SKPaint()
        {
            Color = SKColors.Yellow,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintYellow2AimView = new SKPaint()
        {
            Color = SKColor.Parse("ffc70f"),
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintWhiteAimView = new SKPaint()
        {
            Color = SKColors.White,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
        };
        private readonly SKPaint _paintGrenade = new SKPaint()
        {
            Color = SKColors.OrangeRed,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintExfilOpen = new SKPaint()
        {
            Color = SKColors.LimeGreen,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintExfilPending = new SKPaint()
        {
            Color = SKColors.Yellow,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        private readonly SKPaint _paintExfilClosed = new SKPaint()
        {
            Color = SKColors.Red,
            StrokeWidth = 1,
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High,
        };
        private readonly SKPaint _statusText = new SKPaint()
        {
            Color = SKColors.Red,
            IsStroke = false,
            TextSize = 48,
            TextEncoding = SKTextEncoding.Utf8,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
            TextAlign = SKTextAlign.Center
        };
        #endregion

        #region Constructor
        public MainForm()
        {
            _config = Program.Config; // get ref to config
            InitializeComponent();
            // init skia
            _mapCanvas = new SKGLControl()
            {
                Size = new Size(50, 50),
                Dock = DockStyle.Fill,
                VSync = _config.Vsync // cap fps to refresh rate, reduce tearing
            };
            tabPage1.Controls.Add(_mapCanvas); // place Radar Map Canvas on top of TabPage1
            checkBox_MapFree.Parent = _mapCanvas; // change parent for checkBox_MapFree 'button'

            _aimView = new SKGLControl()
            {
                Size = new Size(200, 200),
                Location = new Point(0, tabPage1.Height - 200),
                VSync = _config.Vsync, // cap fps to refresh rate, reduce tearing
                Visible = false
            };
            _mapCanvas.Controls.Add(_aimView); // place AimView on top of Radar Map Canvas

            LoadConfig();
            LoadMaps();
            _mapChangeTimer.AutoReset = false;
            _mapChangeTimer.Elapsed += mapChangeTimer_Elapsed;

            this.DoubleBuffered = true; // Prevent flickering
            this.Resize += MainForm_Resize;
            this.Shown += MainForm_Shown;
            _mapCanvas.PaintSurface += mapCanvas_PaintSurface; // Radar Drawing Event
            _aimView.PaintSurface += aimView_PaintSurface; // Aimview Drawing Event
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            _mapCanvas.MouseClick += MapCanvas_MouseClick;
            listView_PmcHistory.MouseDoubleClick += ListView_PmcHistory_MouseDoubleClick;
            _fpsWatch.Start(); // fps counter
        }

        #endregion

        #region Events
        /// <summary>
        /// Event fires when MainForm becomes visible. Loops endlessly but is asynchronously non-blocking.
        /// </summary>
        private async void MainForm_Shown(object sender, EventArgs e)
        {
            while (_mapCanvas.GRContext is null) await Task.Delay(1);
            _mapCanvas.GRContext.SetResourceCacheLimit(503316480); // Fixes low FPS on big maps
            while (true)
            {
                await Task.Run(() => Thread.SpinWait(50000)); // High performance async delay
                _mapCanvas.Refresh(); // re-draw main window & child controls (aimview)
            }
        }
        /// <summary>
        /// Event fires when Window is resized.
        /// </summary>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            _aimView.Size = new Size(200, 200);
            _aimView.Location = new Point(0, tabPage1.Height - 200);
        }
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2) // PMC Loadouts Tab
            {
                richTextBox_PlayersInfo.Clear();
                var pmcs = this.AllPlayers?.Select(x => x.Value)
                    .Where(x => (x.Type is PlayerType.PMC || x.Type is PlayerType.WatchlistPMC)
                    && x.IsActive && x.IsAlive)
                        .ToList()
                        .OrderBy(x => x.GroupID)
                        .ThenBy(x => x.Name);
                if (this.InGame && pmcs is not null)
                {
                    var sb = new StringBuilder();
                    sb.Append(@"{\rtf1\ansi");
                    foreach (var pmc in pmcs)
                    {
                        string title = $"*** {pmc.Name}  L:{pmc.Lvl}";
                        if (pmc.GroupID != -1) title += $" G:{pmc.GroupID}";
                        sb.Append(@$"\b {title} \b0 ");
                        sb.Append(@" \line ");
                        var gear = pmc.Gear; // cache ref
                        if (gear is not null) foreach (var slot in gear)
                        {
                                sb.Append(@$"\b {slot.Key}: \b0 ");
                                sb.Append(slot.Value);
                                sb.Append(@" \line ");
                        }
                        sb.Append(@" \line ");
                    }
                    sb.Append(@"}");
                    richTextBox_PlayersInfo.Rtf = sb.ToString();
                }
            }
            else if (tabControl1.SelectedIndex == 3) // PMC History Tab
            {
                listView_PmcHistory.Items.Clear(); // Clear old view
                listView_PmcHistory.Items.AddRange(Player.History); // Obtain new view
                listView_PmcHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent); // resize PMC History columns automatically
            }
        }
        /// <summary>
        /// Event fires when the Aimview checkbox is toggled in settings.
        /// </summary>
        private void checkBox_Aimview_CheckedChanged(object sender, EventArgs e)
        {
            _aimView.Visible = checkBox_Aimview.Checked;
        }

        /// <summary>
        /// Event fires when the "Map Free" or "Map Follow" checkbox (button) is clicked on the Main Window.
        /// </summary>
        private void checkBox_MapFree_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_MapFree.Checked)
            {
                checkBox_MapFree.Text = "Map Follow";
                lock (_renderLock)
                {
                    var player = this.CurrentPlayer;
                    if (player is not null)
                    {
                        var pos = VectorToMapPos(player.Position);
                        _mapPanPosition = new MapPosition()
                        {
                            X = pos.X,
                            Y = pos.Y,
                            Height = pos.Height
                        };
                    }
                }
            }
            else checkBox_MapFree.Text = "Map Free";
        }

        /// <summary>
        /// Event fires when Map Setup box is checked/unchecked.
        /// </summary>
        private void checkBox_MapSetup_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_MapSetup.Checked)
            {
                groupBox_MapSetup.Visible = true;
                textBox_mapX.Text = _currentMap.ConfigFile.X.ToString();
                textBox_mapY.Text = _currentMap.ConfigFile.Y.ToString();
                textBox_mapScale.Text = _currentMap.ConfigFile.Scale.ToString();
            }
            else groupBox_MapSetup.Visible = false;
        }

        /// <summary>
        /// Event fires when Restart Game button is clicked in Settings.
        /// </summary>
        private void button_Restart_Click(object sender, EventArgs e)
        {
            Memory.Restart();
        }
        /// <summary>
        /// Event fires when Refresh Loot button is clicked in Settings.
        /// </summary>
        private void button_RefreshLoot_Click(object sender, EventArgs e)
        {
            Memory.RefreshLoot();
        }

        /// <summary>
        /// Event fires when Apply button is clicked in the "Map Setup Groupbox".
        /// </summary>
        private void button_MapSetupApply_Click(object sender, EventArgs e)
        {
            if (float.TryParse(textBox_mapX.Text, out float x) &&
                float.TryParse(textBox_mapY.Text, out float y) &&
                float.TryParse(textBox_mapScale.Text, out float scale))
            {
                lock (_renderLock)
                {
                    _currentMap.ConfigFile.X = x;
                    _currentMap.ConfigFile.Y = y;
                    _currentMap.ConfigFile.Scale = scale;
                    _currentMap.ConfigFile.Save(_currentMap);
                }
            }
            else
            {
                throw new Exception("INVALID float values in Map Setup.");
            }
        }

        /// <summary>
        /// Allows panning the map when in "Free" mode.
        /// </summary>
        private void MapCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox_MapFree.Checked)
            {
                var center = new SKPoint(_mapCanvas.Width / 2, _mapCanvas.Height / 2);
                lock (_renderLock)
                {
                    _mapPanPosition = new MapPosition()
                    {
                        X = _mapPanPosition.X + (e.X - center.X),
                        Y = _mapPanPosition.Y + (e.Y - center.Y)
                    };
                }
            }
        }
        /// <summary>
        /// Executes map change after a short delay, in case switching through maps quickly to reduce UI lag.
        /// </summary>
        private void mapChangeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                button_Map.Enabled = false;
                button_Map.Text = "Loading...";
            }));
            lock (_renderLock)
            {
                try
                {
                    _currentMap = _allMaps[_mapIndex]; // Swap map
                    if (_loadedMap is not null)
                    {
                        foreach (var map in _loadedMap) map?.Dispose();
                    }
                    _loadedMap = new SKBitmap[_currentMap.ConfigFile.Maps.Count];
                    for (int i = 0; i < _loadedMap.Length; i++)
                    {
                        using (var stream = File.Open(_currentMap.ConfigFile.Maps[i].Item2, FileMode.Open, FileAccess.Read))
                        {
                            _loadedMap[i] = SKBitmap.Decode(stream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"ERROR loading {_currentMap.ConfigFile.Maps[0].Item2}: {ex}");
                }
                finally
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        button_Map.Enabled = true;
                        button_Map.Text = "Toggle Map (F5)";
                    }));
                }
            }
        }

        /// <summary>
        /// Event fires when the Map button is clicked in Settings.
        /// </summary>
        private void button_Map_Click(object sender, EventArgs e)
        {
            ToggleMap();
        }
        /// <summary>
        /// Reloads watchlist in Player.cs
        /// </summary>
        private void button_watchlist_Click(object sender, EventArgs e)
        {
            try
            {
                Player.LoadWatchlist();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Copies Player "BSG ID" to Clipboard upon double clicking History Entry.
        /// </summary>
        private void ListView_PmcHistory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView_PmcHistory.HitTest(e.X, e.Y);
            ListViewItem view = info.Item;
            if (view is not null)
            {
                var entry = (PlayerHistoryEntry)view.Tag;
                if (entry is not null)
                {
                    var id = entry.ToString();
                    if (id is not null && id != string.Empty)
                    {
                        Clipboard.SetText(id); // Copy BSG ID to clipboard
                        MessageBox.Show($"Copied '{id}' to Clipboard!");
                    }
                }
            }
        }


        #endregion

        #region Methods
        /// <summary>
        /// Load previously set GUI Config values.
        /// </summary>
        private void LoadConfig()
        {
            trackBar_AimLength.Value = _config.PlayerAimLineLength;
            checkBox_Loot.Checked = _config.LootEnabled;
            checkBox_Aimview.Checked = _config.AimViewEnabled;
            trackBar_Zoom.Value = _config.DefaultZoom;
        }

        /// <summary>
        /// Load map files (.PNG) and Configs (.JSON) from \\Maps folder.
        /// </summary>
        private void LoadMaps()
        {
            var dir = new DirectoryInfo($"{Environment.CurrentDirectory}\\Maps");
            if (!dir.Exists) dir.Create();
            var configs = dir.GetFiles("*.json"); // Get all PNG Files
            if (configs.Length == 0) throw new IOException("Maps folder is empty!");
            foreach (var config in configs)
            {
                var name = Path.GetFileNameWithoutExtension(config.Name); // map name ex. 'CUSTOMS' w/o extension
                _allMaps.Add(new Map
                (
                    name.ToUpper(),
                    MapConfig.LoadFromFile(config.FullName),
                    config.FullName)
                );
            }
            try
            {
                _currentMap = _allMaps[0];
                _loadedMap = new SKBitmap[_currentMap.ConfigFile.Maps.Count];
                for (int i = 0; i < _loadedMap.Length; i++)
                {
                    using (var stream = File.Open(_currentMap.ConfigFile.Maps[i].Item2, FileMode.Open, FileAccess.Read))
                    {
                        _loadedMap[i] = SKBitmap.Decode(stream);
                    }
                }
                tabPage1.Text = $"Radar ({_currentMap.Name})";
            }
            catch (Exception ex)
            {
                throw new Exception($"ERROR loading {_currentMap.ConfigFile.Maps[0].Item2}: {ex}");
            }
        }
        /// <summary>
        /// Provides zoomed map bounds (centers on player).
        /// </summary>
        private MapParameters GetMapParameters(MapPosition currentPlayerPos)
        {
            int mapLayerIndex = 0;
            for (int i = _loadedMap.Length; i > 0; i--)
            {
                if (currentPlayerPos.Height > _currentMap.ConfigFile.Maps[i - 1].Item1)
                {
                    mapLayerIndex = i - 1;
                    break;
                }
            }
            var zoomWidth = _loadedMap[mapLayerIndex].Width * (.01f * trackBar_Zoom.Value);
            var zoomHeight = _loadedMap[mapLayerIndex].Height * (.01f * trackBar_Zoom.Value);

            var bounds = new SKRect(currentPlayerPos.X - zoomWidth / 2,
                currentPlayerPos.Y - zoomHeight / 2,
                currentPlayerPos.X + zoomWidth / 2,
                currentPlayerPos.Y + zoomHeight / 2)
                .AspectFill(new SKSize(_mapCanvas.Width, _mapCanvas.Height));

            return new MapParameters()
            {
                MapLayerIndex = mapLayerIndex,
                Bounds = bounds,
                XScale = (float)_mapCanvas.Width / (float)bounds.Width, // Set scale for this frame
                YScale = (float)_mapCanvas.Height / (float)bounds.Height // Set scale for this frame
            };
        }

        /// <summary>
        /// Checks if provided location is within current zoomed map bounds, and provides offset coordinates.
        /// </summary>
        private MapPosition GetZoomedPosOffset(MapPosition location, MapParameters mapParams)
        {
            return new MapPosition()
            {
                X = (location.X - mapParams.Bounds.Left) * mapParams.XScale,
                Y = (location.Y - mapParams.Bounds.Top) * mapParams.YScale,
                Height = location.Height
            };
        }

        /// <summary>
        /// Determines if an aggressor player is facing a friendly player.
        /// </summary>
        private static bool IsAggressorFacingTarget(SKPoint aggressor, double aggressorDegrees, SKPoint target)
        {
            const int maxDiff = 5; // Maximum degrees difference to yield 'True' result.
            var radians = Math.Atan2(target.Y - aggressor.Y, target.X - aggressor.X);
            var degs = radians * (180 / Math.PI);
            if (degs < 0) degs += 360f; // handle if negative
            var diff = Math.Max(degs, aggressorDegrees) - Math.Min(degs, aggressorDegrees);
            if (diff <= maxDiff) return true; // Aggressor is aiming at target.
            else return false; // Aggressor is ~NOT~ aiming at target.
        }

        /// <summary>
        /// Gets drawing paintbrush based on PlayerType.
        /// </summary>
        private SKPaint GetPaint(PlayerType playerType)
        {
            if (playerType is PlayerType.Teammate) return _paintLtGreen;
            else if (playerType is PlayerType.PMC) return _paintRed;
            else if (playerType is PlayerType.PlayerScav) return _paintWhite;
            else if (playerType is PlayerType.AIBoss) return _paintViolet;
            else if (playerType is PlayerType.AIScav) return _paintYellow;
            else if (playerType is PlayerType.AIRaider) return _paintYellow2;
            else if (playerType is PlayerType.WatchlistPMC) return _paintPink;
            else return _paintRed; // Default
        }
        /// <summary>
        /// Gets typing paintbrush based on PlayerType.
        /// </summary>
        private SKPaint GetText(PlayerType playerType)
        {
            if (playerType is PlayerType.Teammate) return _textLtGreen;
            else if (playerType is PlayerType.PMC) return _textRed;
            else if (playerType is PlayerType.PlayerScav) return _textWhite;
            else if (playerType is PlayerType.AIBoss) return _textViolet;
            else if (playerType is PlayerType.AIScav) return _textYellow;
            else if (playerType is PlayerType.AIRaider) return _textYellow2;
            else if (playerType is PlayerType.WatchlistPMC) return _textPink;
            else return _textRed; // Default
        }

        /// <summary>
        /// Convert degrees to radians in order to calculate drawing angles.
        /// </summary>
        private static double Deg2Rad(float deg)
        {
            return (Math.PI / 180) * deg;
        }

        /// <summary>
        /// Convert game positional values to UI Map Coordinates.
        /// </summary>
        private MapPosition VectorToMapPos(Vector3 vector)
        {
            var zeroX = _currentMap.ConfigFile.X;
            var zeroY = _currentMap.ConfigFile.Y;
            var scale = _currentMap.ConfigFile.Scale;

            var x = zeroX + (vector.X * scale);
            var y = zeroY - (vector.Y * scale); // Invert 'Y' unity 0,0 bottom left, C# top left
            return new MapPosition()
            {
                X = x,
                Y = y,
                Height = vector.Z // Keep as float, calculation done later
            };
        }

        /// <summary>
        /// Toggles currently selected map.
        /// </summary>
        private void ToggleMap()
        {
            if (!button_Map.Enabled) return;
            if (_mapIndex == _allMaps.Count - 1) _mapIndex = 0; // Start over when end of maps reached
            else _mapIndex++; // Move onto next map
            tabPage1.Text = $"Radar ({_allMaps[_mapIndex].Name})";
            _mapChangeTimer.Reset(); // Start delay
        }
        /// <summary>
        /// Returns paint color based on target player type.
        /// </summary>
        private SKPaint GetAimViewPaint(Player player)
        {
            switch (player.Type)
            {
                case PlayerType.PMC:
                    return _paintRedAimView;
                case PlayerType.Teammate:
                    return _paintGreenAimView;
                case PlayerType.AIBoss:
                    return _paintVioletAimView;
                case PlayerType.AIScav:
                    return _paintYellowAimView;
                case PlayerType.AIRaider:
                    return _paintYellow2AimView;
                case PlayerType.PlayerScav:
                    return _paintWhiteAimView;
                case PlayerType.WatchlistPMC:
                    return _paintPinkAimView;
                default:
                    return _paintRedAimView;
            }
        }
        #endregion

        #region MainRender
        private void mapCanvas_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            lock (_renderLock)
            {
                bool isReady = this.Ready; // cache bool
                bool inGame = this.InGame; // cache bool
                var currentPlayer = this.CurrentPlayer; // cache ref to current player
                if (_fpsWatch.ElapsedMilliseconds >= 1000)
                {
                    _mapCanvas.GRContext.PurgeResources(); // Seems to fix mem leak issue on increasing resource cache
                    string title = "EFT Radar";
                    if (inGame && currentPlayer is not null)
                    {
                        title += $" ({_fps} fps) ({Memory.Ticks} mem/s)";
                        if (this.LoadingLoot) title += " - LOADING LOOT";
                    }
                    this.Text = title; // Set window title
                    _fpsWatch.Restart();
                    _fps = 0;
                }
                else _fps++;
                SKSurface surface = e.Surface;
                SKCanvas canvas = surface.Canvas;
                canvas.Clear();
                try
                {
                    if (inGame && currentPlayer is not null)
                    {
                        // Get main player location
                        var currentPlayerRawPos = currentPlayer.Position;
                        var currentPlayerDirection = Deg2Rad(currentPlayer.Rotation.X);
                        var currentPlayerPos = VectorToMapPos(currentPlayerRawPos);
                        if (groupBox_MapSetup.Visible) // Print coordinates (to make it easy to setup JSON configs)
                        {
                            label_Pos.Text = $"Unity X,Y,Z: {currentPlayerRawPos.X},{currentPlayerRawPos.Y},{currentPlayerRawPos.Z}";
                        }

                        // Prepare to draw Game Map
                        MapParameters mapParams; // Drawing Source
                        if (checkBox_MapFree.Checked)
                        {
                            _mapPanPosition.Height = currentPlayerPos.Height;
                            mapParams = GetMapParameters(_mapPanPosition);
                        }
                        else mapParams = GetMapParameters(currentPlayerPos);
                        var windowRect = new SKRect() // Drawing Destination
                        {
                            Left = _mapCanvas.Left,
                            Right = _mapCanvas.Right,
                            Top = _mapCanvas.Top,
                            Bottom = _mapCanvas.Bottom
                        };
                        // Draw Game Map
                        canvas.DrawBitmap(_loadedMap[mapParams.MapLayerIndex], mapParams.Bounds, windowRect, _mapPaint);

                        // Draw LocalPlayer Scope
                        {
                            var zoomedCurrentPlayerPos = GetZoomedPosOffset(currentPlayerPos, mapParams); // always true
                            canvas.DrawCircle(zoomedCurrentPlayerPos.GetPoint(), 6, _paintGreen);
                            var point1 = new SKPoint(zoomedCurrentPlayerPos.X, zoomedCurrentPlayerPos.Y);
                            var point2 = new SKPoint((float)(zoomedCurrentPlayerPos.X + Math.Cos(currentPlayerDirection) * trackBar_AimLength.Value), (float)(zoomedCurrentPlayerPos.Y + Math.Sin(currentPlayerDirection) * trackBar_AimLength.Value));
                            canvas.DrawLine(point1, point2, _paintGreen);
                        }

                        // Draw other players
                        var allPlayers = this.AllPlayers?.Select(x => x.Value)
                            .Where(x => !(!x.IsActive && x.IsAlive)); // Skip exfil'd players
                        if (allPlayers is not null)
                        {
                            var friendlies = allPlayers.Where(x => (x.Type is PlayerType.CurrentPlayer
                                || x.Type is PlayerType.Teammate) &&
                                x.IsActive && x.IsAlive);
                            foreach (var player in allPlayers) // Draw PMCs
                            {
                                if (player.Type is PlayerType.CurrentPlayer) continue; // Already drawn current player, move on
                                var playerRawPos = player.Position;
                                var playerRotation = player.Rotation;
                                var playerPos = VectorToMapPos(playerRawPos);
                                var zoomedPlayerPos = GetZoomedPosOffset(playerPos, mapParams);
                                var playerDirection = Deg2Rad(playerRotation.X);
                                int aimlineLength = 15;
                                if (player.IsAlive is false)
                                { // Draw 'X' death marker
                                    canvas.DrawLine(new SKPoint(zoomedPlayerPos.X - 6, zoomedPlayerPos.Y + 6), new SKPoint(zoomedPlayerPos.X + 6, zoomedPlayerPos.Y - 6), _paintBlack);
                                    canvas.DrawLine(new SKPoint(zoomedPlayerPos.X - 6, zoomedPlayerPos.Y - 6), new SKPoint(zoomedPlayerPos.X + 6, zoomedPlayerPos.Y + 6), _paintBlack);
                                    continue;
                                }
                                else if (player.Type is not PlayerType.Teammate)
                                {
                                    foreach (var friendly in friendlies)
                                    {
                                        var friendlyRawPos = friendly.Position;
                                        var friendlyDist = Vector3.Distance(playerRawPos, friendlyRawPos);
                                        if (friendlyDist > _config.MaxDistance) continue; // max range, no lines across entire map
                                        var friendlyPos = VectorToMapPos(friendlyRawPos);
                                        if (IsAggressorFacingTarget(playerPos.GetPoint(),
                                            playerRotation.X,
                                            friendlyPos.GetPoint()))
                                        {
                                            aimlineLength = 1000; // Lengthen aimline
                                            break;
                                        }
                                    }
                                }
                                else if (player.Type is PlayerType.Teammate)
                                {
                                    aimlineLength = trackBar_AimLength.Value; // Allies use player's aim length
                                }
                                // Draw Player Scope
                                {
                                    string name = player.Name;
                                    if (player.ErrorCount > 10) name = "ERROR"; // In case POS stops updating, let us know!
                                    var plyrHeight = playerPos.Height - currentPlayerPos.Height;
                                    var plyrDist = Vector3.Distance(currentPlayerRawPos, playerRawPos);
                                    canvas.DrawText($"L{player.Lvl}:{name} ({player.Health})", zoomedPlayerPos.GetPoint(9, 3), GetText(player.Type));
                                    string stats = null;
                                    if (player.GroupID != -1
                                        && player.Type is not PlayerType.Teammate)
                                        stats = $"H: {(int)Math.Round(plyrHeight)} D: {(int)Math.Round(plyrDist)} G:{player.GroupID}";
                                    else stats = $"H: {(int)Math.Round(plyrHeight)} D: {(int)Math.Round(plyrDist)}";
                                    canvas.DrawText(stats, zoomedPlayerPos.GetPoint(9, 15), GetText(player.Type));
                                    canvas.DrawCircle(zoomedPlayerPos.GetPoint(), 6, GetPaint(player.Type)); // smaller circle
                                    var point1 = new SKPoint(zoomedPlayerPos.X, zoomedPlayerPos.Y);
                                    var point2 = new SKPoint((float)(zoomedPlayerPos.X + Math.Cos(playerDirection) * aimlineLength), (float)(zoomedPlayerPos.Y + Math.Sin(playerDirection) * aimlineLength));
                                    canvas.DrawLine(point1, point2, GetPaint(player.Type));
                                }
                            }
                            // Draw loot (if enabled)
                            if (checkBox_Loot.Checked)
                            {
                                var loot = this.Loot; // cache ref
                                if (loot is not null) foreach (var item in loot)
                                    {
                                        SKPaint paint;
                                        SKPaint text;
                                        if (item.Important)
                                        {
                                            paint = _paintImportantLoot;
                                            text = _importantLootText;
                                        }
                                        else
                                        {
                                            paint = _paintLoot;
                                            text = _lootText;
                                        }
                                        var mapPos = VectorToMapPos(item.Position);
                                        var mapPosZoom = GetZoomedPosOffset(mapPos, mapParams);
                                        var heightDiff = item.Position.Z - currentPlayerPos.Height;
                                        if (heightDiff > 1.45) // loot is above player
                                        {
                                            using var path = mapPosZoom.GetUpArrow();
                                            canvas.DrawPath(path, paint);
                                        }
                                        else if (heightDiff < -1.45) // loot is below player
                                        {
                                            using var path = mapPosZoom.GetDownArrow();
                                            canvas.DrawPath(path, paint);
                                        }
                                        else // loot is level with player
                                        {
                                            canvas.DrawCircle(mapPosZoom.GetPoint(), 5, paint);
                                        }
                                        canvas.DrawText(item.Label, mapPosZoom.GetPoint(7, 3), text);
                                    }
                            }
                            var grenades = this.Grenades; // cache ref
                            if (grenades is not null) // Draw grenades
                            {
                                foreach (var grenade in grenades)
                                {
                                    var pos = GetZoomedPosOffset(VectorToMapPos(grenade.Position), mapParams);
                                    canvas.DrawCircle(pos.GetPoint(), 5, _paintGrenade);
                                }
                            }
                            var exfils = this.Exfils; // cache ref
                            if (exfils is not null)
                            {
                                foreach (var exfil in exfils)
                                {
                                    var pos = GetZoomedPosOffset(VectorToMapPos(exfil.Position), mapParams);
                                    SKPaint paint = _paintExfilClosed; // default
                                    switch (exfil.Status)
                                    {
                                        case ExfilStatus.Open:
                                            paint = _paintExfilOpen;
                                            break;
                                        case ExfilStatus.Pending:
                                            paint = _paintExfilPending;
                                            break;
                                        case ExfilStatus.Closed:
                                            paint = _paintExfilClosed;
                                            break;
                                    }
                                    var heightDiff = pos.Height - currentPlayerPos.Height;
                                    if (heightDiff > 1.85) // exfil is above player
                                    {
                                        using var path = pos.GetUpArrow(5);
                                        canvas.DrawPath(path, paint);
                                    }
                                    else if (heightDiff < -1.85) // exfil is below player
                                    {
                                        using var path = pos.GetDownArrow(5);
                                        canvas.DrawPath(path, paint);
                                    }
                                    else // exfil is level with player
                                    {
                                        canvas.DrawCircle(pos.GetPoint(), 4, paint);
                                    }
                                }
                            }
                        }
                    }
                    else // Not rendering, display reason
                    {
                        if (!isReady)
                            canvas.DrawText("Game Process Not Running", _mapCanvas.Width / 2, _mapCanvas.Height / 2, _statusText);
                        else if (!inGame)
                            canvas.DrawText("Waiting for Raid Start...", _mapCanvas.Width / 2, _mapCanvas.Height / 2, _statusText);
                        else if (currentPlayer is null)
                            canvas.DrawText("Cannot find LocalPlayer", _mapCanvas.Width / 2, _mapCanvas.Height / 2, _statusText);
                    }
                } catch { }
                canvas.Flush(); // commit to GPU
            }
        }
        #endregion

        #region AimviewRender
        /// <summary>
        /// Renders 200x200 "AimView" Window in bottom left of Window.
        /// </summary>
        private void aimView_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(); // clear last frame
            try
            {
                var me = this.CurrentPlayer; // cache ref
                var allPlayers = this.AllPlayers?.Select(x => x.Value)
                    .Where(x => x.IsActive
                    && x.IsAlive
                    && x.Type is not PlayerType.CurrentPlayer); // get all active players except LocalPlayer
                if (this.InGame && me is not null && allPlayers is not null)
                {
                    var mePos = me.Position; // cache pos
                    var meRotation = me.Rotation; // cache rotation
                    var normalizedDirection = -meRotation.X;
                    if (normalizedDirection < 0) normalizedDirection = normalizedDirection + 360;

                    var pitch = meRotation.Y;
                    if (pitch >= 270)
                    {
                        pitch = 360 - pitch;
                    }
                    else
                    {
                        pitch = -pitch;
                    }

                    foreach (var player in allPlayers)
                    {
                        var playerPos = player.Position;
                        float heighDiff = playerPos.Z - mePos.Z;
                        float dist = Vector3.Distance(mePos, playerPos);
                        float angleY = (float)(180 / Math.PI * Math.Atan(heighDiff / dist)) - pitch;
                        float y = angleY / _config.AimViewFOV * _aimView.Height + _aimView.Height / 2;

                        float opposite = playerPos.Y - mePos.Y;
                        float adjacent = playerPos.X - mePos.X;
                        float angleX = (float)(180 / Math.PI * Math.Atan(opposite / adjacent));

                        if (adjacent < 0 && opposite > 0)
                        {
                            angleX = 180 + angleX;
                        }
                        else if (adjacent < 0 && opposite < 0)
                        {
                            angleX = 180 + angleX;
                        }
                        else if (adjacent > 0 && opposite < 0)
                        {
                            angleX = 360 + angleX;
                        }
                        angleX = angleX - normalizedDirection;
                        float x = angleX / _config.AimViewFOV * _aimView.Width + _aimView.Width / 2;

                        canvas.DrawCircle(_aimView.Width - x, _aimView.Height - y, 10 * (1 - dist / _config.MaxDistance), GetAimViewPaint(player));
                    }
                    // draw crosshair at end
                    canvas.DrawLine(0, _aimView.Height / 2, _aimView.Width, _aimView.Height / 2, _paintAimViewCrosshair);
                    canvas.DrawLine(_aimView.Width / 2, 0, _aimView.Width / 2, _aimView.Height, _paintAimViewCrosshair);
                }
            }
            catch { }
            canvas.Flush(); // commit current frame to gpu
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Form closing event.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e) // Raised on Close()
        {
            try
            {
                _config.PlayerAimLineLength = trackBar_AimLength.Value;
                _config.LootEnabled = checkBox_Loot.Checked;
                _config.AimViewEnabled = checkBox_Aimview.Checked;
                _config.DefaultZoom = trackBar_Zoom.Value;
                Config.SaveConfig(_config);
                Memory.Shutdown();
            }
            finally { base.OnFormClosing(e); }
        }

        /// <summary>
        /// Process hotkey presses.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                if (trackBar_Zoom.Value - 5 >= 1) trackBar_Zoom.Value -= 5;
                else trackBar_Zoom.Value = 1;
                return true;
            }
            else if (keyData == (Keys.F2))
            {
                if (trackBar_Zoom.Value + 5 <= 200) trackBar_Zoom.Value += 5;
                else trackBar_Zoom.Value = 200;
                return true;
            }
            else if (keyData == (Keys.F3))
            {
                this.checkBox_Loot.Checked = !this.checkBox_Loot.Checked; // Toggle loot
                return true;
            }
            else if (keyData == (Keys.F4))
            {
                this.checkBox_Aimview.Checked = !this.checkBox_Aimview.Checked; // Toggle aimview
                return true;
            }
            else if (keyData == (Keys.F5))
            {
                ToggleMap();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
    #endregion
}
