﻿using System;
namespace Plex.Engine.Discord
{
    [Serializable]
    public struct RichPresence
    {
        public string state; /* max 128 bytes */
        public string details; /* max 128 bytes */
        public long startTimestamp;
        public long endTimestamp;
        public string largeImageKey; /* max 32 bytes */
        public string largeImageText; /* max 128 bytes */
        public string smallImageKey; /* max 32 bytes */
        public string smallImageText; /* max 128 bytes */
        public string partyId; /* max 128 bytes */
        public int partySize;
        public int partyMax;
        public string matchSecret; /* max 128 bytes */
        public string joinSecret; /* max 128 bytes */
        public string spectateSecret; /* max 128 bytes */
        public bool instance;
    }
}