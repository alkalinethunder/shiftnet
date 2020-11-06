﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Objects;
using static Plex.Engine.FSUtils;

namespace Shiftnet.Apps
{
    [AppInformation("File Manager", "browse the files and folders of your computer.", Command = "browse")]
    public class FileSkimmer : ShiftApp
    {
        protected override void Main()
        {
            Title = "File Browser";
        }
    }
}
