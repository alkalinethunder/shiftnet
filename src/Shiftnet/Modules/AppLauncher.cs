using System;
using AlkalineThunder.Pandemic;
using Shiftnet.Apps;

namespace Shiftnet.Modules
{
    public class AppLauncher
    {
        private AppInformationAttribute _appInfo;
        private Type _app;

        public bool Startup
            => _appInfo.Startup;
        
        public bool PlayerOnly
            => _appInfo.PlayerOnly;
        
        public string Command
            => _appInfo.Command;
        
        public bool HasCommand
            => !string.IsNullOrWhiteSpace(Command);
        
        public string TypeName
            => _app.FullName;

        public string Description
            => _appInfo.Description;

        public string Name
            => _appInfo.Name;
        
        public AppLauncher(Type type, AppInformationAttribute info)
        {
            _app = type;
            _appInfo = info;
        }

        public bool IsLauncherFor<T>() where T : ShiftApp, new()
        {
            return _app == typeof(T);
        }
        
        public ShiftApp Launch(PropertySet args, Desktop os, string cwd)
        {
            var app = (ShiftApp) Activator.CreateInstance(_app, null);
            var appHost = os.CreateAppHost(_appInfo);
            app.Initialize(appHost, args, cwd);
            return app;
        }
    }
}