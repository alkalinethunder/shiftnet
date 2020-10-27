﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;

namespace Plex.Engine.Theming
{
    public abstract class Theme
    {
        public abstract void LoadThemeData(GraphicsDevice device);

        public abstract Color GetAccentColor();
        
        //Rendering
        public abstract void DrawButtonBackground(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state);
        public abstract void DrawButtonImage(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state, Texture2D image);
        public abstract void DrawButtonText(GraphicsContext gfx, string text, int x, int y, int width, int height, ButtonState state);
        public abstract void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state, ArrowDirection direction);
        public abstract void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height);
        public abstract void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style);
        public abstract void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style, ButtonState state);
        public abstract void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style);
        public abstract void DrawWindowBorder(GraphicsContext graphics, int x, int y, int width, int height, bool focused, bool maximized, ButtonState close, ButtonState maximize, ButtonState minimize, bool dialog);
        public abstract Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight);
        public abstract void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver);

        //Measurement
        public abstract Vector2 MeasureString(TextControlFontStyle style, string text, TextAlignment alignment = TextAlignment.TopLeft, int maxwidth = int.MaxValue);
    }

    public enum ButtonState
    {
        Idle,
        MouseHover,
        MouseDown
    }

    public enum ArrowDirection
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public enum TitleButton
    {
        Close,
        Minimize,
        Maximize
    }

}
