﻿using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using Whoa;

namespace Plex.Objects{    public class Hackable    {        [Order]
        public string SystemName { get; set; }        [Order]
        public string FriendlyName { get; set; }        [Order]
        public string WelcomeMessage { get; set; }        [Order]
        public string Description { get; set; }        [Order]
        public SystemType SystemType { get; set; }        [Order]
        public string OnHackCompleteStoryEvent { get; set; }        [Order]
        public string OnHackFailedStoryEvent { get; set; }

        [Order]
        public string Dependencies { get; set; }        [Order]
        public int Rank { get; set; }        public string ID        {            get            {                return SystemName.ToLower().Replace(" ", "_");            }        }

        [Order]
        public float X { get; set; }
        [Order]
        public float Y { get; set; }

        public override string ToString()        {            return $"{FriendlyName} ({SystemName})";        }    }

    [Flags]    public enum SystemType    {
        Computer = 1,
        Mobile = 2,
        WebServer = 4,
        MailServer = 8,
        NAS = 16,
        Database = 32,
        Router = 64,
        UpgradeDB = 128,
        Bank = 256
    }    }