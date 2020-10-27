using System;
using AlkalineThunder.Pandemic;
using Community.CsharpSqlite;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Xna.Framework;

namespace Shiftnet.Integrations
{
    public class DiscordRichPresenceModule : EngineModule
    {
        private DiscordRpcClient _rpc;
        private RichPresence _presence;
        private Timestamps _timestamps;
        
        protected override void OnInitialize()
        {
            GameUtils.Log("discord rpc init...");

            _rpc = new DiscordRpcClient("770739106354429953");
            _rpc.Logger = new PandemicConsoleDiscordLogger();

            _rpc.Initialize();
        }

        protected override void OnLoadContent()
        {
            _timestamps = new Timestamps(DateTime.UtcNow);
            
            _presence = new RichPresence()
                .WithTimestamps(_timestamps);

            _rpc.SetPresence(_presence);
            
            GameUtils.Log("discord rpc connected.");
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _rpc.UpdateEndTime(DateTime.UtcNow);
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