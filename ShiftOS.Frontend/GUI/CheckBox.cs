﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Frontend.GraphicsSubsystem;
using ShiftOS.Engine;
using static ShiftOS.Engine.SkinEngine;
namespace ShiftOS.Frontend.GUI
{
    public class CheckBox : GUI.Control
    {
        public CheckBox()
        {
            Width = 24;
            Height = 24;
            Click += () =>
            {
                Checked = !_checked;
            };
        }

        private bool _checked = false;

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (value == _checked)
                    return;
                _checked = value;
                CheckedChanged?.Invoke();
                Invalidate();
            }
        }

        protected override void OnPaint(GraphicsContext gfx)
        {
            gfx.DrawRectangle(0, 0, Width, Height, UIManager.SkinTextures["ControlTextColor"]);
            if (_checked)
            {
                gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlTextColor"]);
            }
            else
            {
                gfx.DrawRectangle(1, 1, Width - 2, Height - 2, UIManager.SkinTextures["ControlColor"]);

            }
        }

        public event Action CheckedChanged;
    }
}