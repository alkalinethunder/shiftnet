﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AlkalineThunder.Pandemic;
using Shiftnet.Apps;

namespace Shiftnet.Modules
{
    public class AppsModule : EngineModule
    {
        private List<AppLauncher> _apps = new List<AppLauncher>();

        public IEnumerable<HelpEntry> AvailableApps
            => _apps.Select(x => new HelpEntry(x.Name, x.Description));
        
        private void LocateApps()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes().Where(x => x.InheritsFrom(typeof(ShiftApp)))
                    .Where(x => x.GetConstructor(Type.EmptyTypes) != null))
                {
                    var attribute = type.GetCustomAttributes(false).OfType<AppInformationAttribute>().FirstOrDefault();

                    if (attribute != null)
                    {
                        _apps.Add(new AppLauncher(type, attribute));
                    }
                }
            }

        }

        [Exec("apps.list")]
        public void PrintAppsList()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Apps:");

            foreach (var app in _apps)
            {
                sb.AppendLine($"{app.TypeName} (\"{app.Name}\")");
            }
                
            GameUtils.Log(sb.ToString());
        }

        protected override void OnInitialize()
        {
            LocateApps();
        }
    }
}