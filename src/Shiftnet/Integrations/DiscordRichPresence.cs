using System;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Debugging;
using AlkalineThunder.Pandemic.Settings;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Xna.Framework;
using ILogger = DiscordRPC.Logging.ILogger;
using LogLevel = DiscordRPC.Logging.LogLevel;

namespace Shiftnet.Integrations
{
    [RequiresModule(typeof(SettingsService))]
    public class DiscordRichPresenceModule : EngineModule
    {
        private DiscordRpcClient _rpc;
        private RichPresence _presence;
        private Timestamps _timestamps;

        private SettingsService Settings
            => GetModule<SettingsService>();

        public bool EnableRichPresence
        {
            get => Settings.GetValue("discord.presence", true);
            set => Settings.SetValue("discord.presence", value);
        }
        
        protected override void OnInitialize()
        {
            App.Logger.Info("discord rpc init...");

            _timestamps = new Timestamps(DateTime.UtcNow);

            _presence = new RichPresence()
                .WithTimestamps(_timestamps);

            if (EnableRichPresence)
            {
                InitializeRichPresence();
            }
        }

        private void InitializeRichPresence()
        {
            _rpc = new DiscordRpcClient("770739106354429953");
            _rpc.Logger = new PandemicConsoleDiscordLogger(App.Logger);
            _rpc.Initialize();
            UpdatePresence();
        }

        private void ShutdownRichPresence()
        {
            _rpc.Deinitialize();
            _rpc = null;
        }
        
        protected override void OnLoadContent()
        {
            App.Logger.Info("discord rpc connected.");
            
            Settings.SettingsUpdated += SettingsOnSettingsUpdated;
        }

        private void SettingsOnSettingsUpdated(object? sender, EventArgs e)
        {
            if (EnableRichPresence && _rpc == null)
            {
                InitializeRichPresence();
            }
            else if (_rpc != null && !EnableRichPresence)
            {
                ShutdownRichPresence();
            }
        }

        private void UpdatePresence()
        { 
            _rpc.SetPresence(_presence);
        }
        
        protected override void OnUpdate(GameTime gameTime)
        {
            if (_rpc != null)
            {
                _rpc.UpdateEndTime(DateTime.UtcNow);
            }
        }

        [Exec("discord.setPresenceEnabled")]
        public void Exec_SetPresenceEnabled(bool value)
        {
            EnableRichPresence = value;
        }
    }

    public class PandemicConsoleDiscordLogger : ILogger
    {
        private Logger _logger;
        
        internal PandemicConsoleDiscordLogger(Logger logger)
        {
            _logger = logger;
        }
        
        public void Trace(string message, params object[] args)
        {
            _logger.Trace(string.Format(message, args));
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            _logger.Warn(string.Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(string.Format(message, args));
        }

        public LogLevel Level { get; set; }
    }
}