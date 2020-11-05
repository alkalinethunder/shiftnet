using System;
using Shiftnet.Apps;

namespace Shiftnet.Modules
{
    public class AppLauncher
    {
        private AppInformationAttribute _appInfo;
        private Type _app;

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
    }
}