﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace eft_dma_radar
{
    static class Program
    {
        private static readonly Mutex _mutex;
        private static readonly bool _singleton;
        private static readonly Config _config;
        private static readonly object _logLock = new();
        private static readonly StreamWriter _log;

        /// <summary>
        /// Global Program Configuration.
        /// </summary>
        public static Config Config
        {
            get => _config;
        }

        #region Static_Constructor
        static Program()
        {
            _mutex = new Mutex(true, "9A19103F-16F7-4668-BE54-9A1E7A4F7556", out _singleton);
            if (Config.TryLoadConfig(out _config) is not true) _config = new Config();
            if (_config.LoggingEnabled)
            {
                _log = File.AppendText("log.txt");
                _log.AutoFlush = true;
            }
        }
        #endregion

        #region ProgramEntryPoint
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode; // allow russian chars
            try
            {
                if (_singleton)
                {
                    RuntimeHelpers.RunClassConstructor(typeof(TarkovMarketManager).TypeHandle); // invoke static constructor
                    ApplicationConfiguration.Initialize();
					Application.Run(new MainForm());
                }
                else
                {
                    throw new Exception("The Application Is Already Running!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "EFT Radar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Public logging method, writes to Debug Trace, and a Log File (if enabled in Config.Json)
        /// </summary>
        public static void Log(string msg)
        {
            Debug.WriteLine(msg);
            if (_config.LoggingEnabled)
            {
                lock (_logLock) // Sync access to File IO
                {
                    _log.WriteLine($"{DateTime.Now}: {msg}");
                }
            }
        }
        #endregion

        #region Pinvokes
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        #endregion
    }
}
