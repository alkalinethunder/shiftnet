using System;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Settings;
using Community.CsharpSqlite;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Xna.Framework;

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
            GameUtils.Log("discord rpc init...");

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
            _rpc.Logger = new PandemicConsoleDiscordLogger();
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
            GameUtils.Log("discord rpc connected.");
            
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
        public void Trace(string message, params object[] args)
        {
            GameUtils.Log(string.Format(message, args));
        }

        public void Info(string message, params object[] args)
        {
            GameUtils.Log(string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            GameUtils.Log(string.Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            GameUtils.Log(string.Format(message, args));
        }

        public LogLevel Level { get; set; }
    }
}