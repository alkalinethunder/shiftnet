﻿using System;

namespace Plex.Objects
        public string SystemName { get; set; }
        public string FriendlyName { get; set; }
        public string WelcomeMessage { get; set; }
        public string Description { get; set; }
        public SystemType SystemType { get; set; }
        public string OnHackCompleteStoryEvent { get; set; }
        public string OnHackFailedStoryEvent { get; set; }

        [Order]
        public string Dependencies { get; set; }
        public int Rank { get; set; }

        [Order]
        public float X { get; set; }
        [Order]
        public float Y { get; set; }

        public override string ToString()

    [Flags]
        Computer = 1,
        Mobile = 2,
        WebServer = 4,
        MailServer = 8,
        NAS = 16,
        Database = 32,
        Router = 64,
        UpgradeDB = 128,
        Bank = 256
    }