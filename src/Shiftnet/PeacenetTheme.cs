﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.GUI;
using Plex.Engine.Theming;

namespace Shiftnet
{
    public class PeacenetTheme : Theme
    {
        private System.Drawing.Font _systemsans = null;
        private System.Drawing.Font _head1 = null;
        private System.Drawing.Font _head2 = null;
        private System.Drawing.Font _head3 = null;
        private SpriteFont _mono = null;
        private Texture2D _close = null;
        private Texture2D _maximize = null;
        private Texture2D _minimize = null;
        private Texture2D _restore = null;

        private Texture2D _arrow_left = null;
        private Texture2D _arrow_top = null;
        private Texture2D _arrow_right = null;
        private Texture2D _arrow_bottom = null;


        private int _titleheight = 30;
        private int _borderright = 2;

        private int _titlebuttonsize = 24;

        private Color _buttonIdle = Color.Black;
        private Color _buttonHover = Color.Black;
        private Color _buttonDown = Color.Black;

        private Color _buttonTextIdle = Color.Black;
        private Color _buttonTextHover = Color.Black;
        private Color _buttonTextDown = Color.Black;
        private Texture2D _check = null;

        private Color _controlBG = Color.Black;
        private Color _controlBGDark = Color.Black;
        private Color _controlBGLight = Color.Black;

        private Color _borderBG = Color.Black;
        private Color _borderBGInactive = Color.Black;

        public override void DrawCheckbox(GraphicsContext gfx, int x, int y, int width, int height, bool isChecked, bool isMouseOver)
        {
            Color _checkFG = (isMouseOver) ? _buttonTextHover : _buttonTextIdle;
            Color _checkBG = (isMouseOver) ? _controlBGLight : _controlBGDark;

            gfx.DrawRectangle(x, y, width, height, _checkFG);
            gfx.DrawRectangle(x + 2, y + 2, width - 4, height - 4, _checkBG);

            if (isChecked)
            {
                gfx.DrawRectangle(x + 2, y + 2, width - 4, height - 4, _check, _checkFG);
            }
        }

        public override void DrawArrow(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state, ArrowDirection direction)
        {
            var arrow = _arrow_left;
            switch(direction)
            {
                case ArrowDirection.Top:
                    arrow = _arrow_top;
                    break;
                case ArrowDirection.Right:
                    arrow = _arrow_right;
                    break;
                case ArrowDirection.Bottom:
                    arrow = _arrow_bottom;
                    break;
            }
            gfx.DrawRectangle(x, y, width, height, arrow, Color.White, ImageLayout.Zoom);
        }

        public override void DrawButtonBackground(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state)
        {
            Color c = _buttonIdle;
            if (state == ButtonState.MouseHover)
                c = _buttonHover;
            if (state == ButtonState.MouseDown)
                c = _buttonDown;
            gfx.DrawRectangle(x, y, width, height, c);
        }

        public override void DrawControlBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _controlBG);
        }

        public override void DrawControlDarkBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _controlBGDark);
        }

        public override void DrawControlLightBG(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _controlBGLight);
        }

        public override void DrawString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style)
        {
            drawStringWithColor(graphics, text, x, y, width, height, style, Color.White);
        }

        private void drawStringWithColor(GraphicsContext gfx, string text, int x, int y, int width, int height, TextControlFontStyle style, Color color)
        {
            var _f = _systemsans;
            if (style == TextControlFontStyle.Mono || style == TextControlFontStyle.Custom)
            {
                RenderTextInternal(gfx, text, x, y, width, height, _mono, color);
                return;
            }
            switch (style)
            {
                case TextControlFontStyle.System:
                    _f = _systemsans;
                    break;
                case TextControlFontStyle.Header1:
                    _f = _head1;
                    break;
                case TextControlFontStyle.Header2:
                    _f = _head2;
                    break;
                case TextControlFontStyle.Header3:
                    _f = _head3;
                    break;
            }
            gfx.DrawString(text, x, y, color, _f, TextAlignment.TopLeft, width, Plex.Engine.TextRenderers.WrapMode.Words);

        }

        public override void DrawTextCaret(GraphicsContext graphics, int x, int y, int width, int height)
        {
            graphics.DrawRectangle(x, y, width, height, _buttonTextIdle);
        }

        public override void DrawWindowBorder(GraphicsContext graphics, int x, int y, int width, int height, bool focused, bool maximized, ButtonState close, ButtonState maximize, ButtonState minimize, bool dialog)
        {
            float coloropacity = (focused) ? 1 : 0.75F;

            var bc = _borderBGInactive;
            if (focused)
                bc = _borderBG;
            graphics.Clear(bc);

            int _tby = (_titleheight - _titlebuttonsize) / 2;
            int _closex = (width - _titlebuttonsize) - _borderright;
            var _closetint = GetButtonTextColor(close) * coloropacity;
            graphics.DrawRectangle(_closex, _tby, _titlebuttonsize, _titlebuttonsize, _close, _closetint);

            if (!dialog)
            {
                int _maxx = (_closex - _titlebuttonsize) - 4;
                var _maxtint = GetButtonTextColor(maximize) * coloropacity;
                graphics.DrawRectangle(_maxx, _tby, _titlebuttonsize, _titlebuttonsize, (maximized) ? _restore : _maximize, _maxtint);

                int _minx = (_maxx - _titlebuttonsize) - 4;
                var _mintint = GetButtonTextColor(minimize) * coloropacity;
                graphics.DrawRectangle(_minx, _tby, _titlebuttonsize, _titlebuttonsize, _minimize, _mintint);
            }
        }

        private Color GetButtonTextColor(ButtonState state)
        {
            var c = _buttonTextIdle;
            if (state == ButtonState.MouseHover)
                c = _buttonTextHover;
            if (state == ButtonState.MouseDown)
                c = _buttonTextDown;
            return c;
        }

        public override void LoadThemeData(GraphicsDevice device)
        {
            bool isMondaInstalled = IsMondaInstalled();
            string fontName = "Tahoma";
            if (isMondaInstalled)
                fontName = "Monda";
            if (DateTime.Now.Day == 6 && DateTime.Now.Month == 7)
                if (IsComicSansInstalled())
                    fontName = "Comic Sans MS";
            _systemsans = new System.Drawing.Font(fontName, 10F);
            _head1 = new System.Drawing.Font(fontName, 25F);
            _head2 = new System.Drawing.Font(fontName, 20F);
            _head3 = new System.Drawing.Font(fontName, 15F);
            _mono = UIManager.ContentLoader.Load<SpriteFont>("UbuntuMono-B");

            _close = UIManager.ContentLoader.Load<Texture2D>("Window/Artwork/Close");
            _minimize = UIManager.ContentLoader.Load<Texture2D>("Window/Artwork/Minimize");
            _maximize = UIManager.ContentLoader.Load<Texture2D>("Window/Artwork/Maximize");
            _restore = UIManager.ContentLoader.Load<Texture2D>("Window/Artwork/Restore");

            _arrow_left = UIManager.ContentLoader.Load<Texture2D>("Arrows/Left");
            _arrow_top = UIManager.ContentLoader.Load<Texture2D>("Arrows/Up");
            _arrow_right = UIManager.ContentLoader.Load<Texture2D>("Arrows/Right");
            _arrow_bottom = UIManager.ContentLoader.Load<Texture2D>("Arrows/Down");

            _borderBG = new Color(64, 128, 255, 255);
            _borderBGInactive = _borderBG * 0.75F;

            _buttonIdle = new Color(90, 90, 90, 255);
            _buttonDown = Color.Black;
            _buttonHover = _borderBG;

            _buttonTextIdle = new Color(191, 191, 191);
            _buttonTextHover = Color.White;
            _buttonTextDown = Color.White;


            _controlBG = new Color(64, 64, 64);
            _controlBGDark = new Color(32, 32, 32);
            _controlBGLight = new Color(111, 111, 111);

            _check = UIManager.ContentLoader.Load<Texture2D>("Infobox/OK");
        }

        
        [Obsolete("Can we seriously stop fucking using GDI?")]
        private Texture2D TexFromImg(System.Drawing.Image img, GraphicsDevice device)
        {
            var bmp = (System.Drawing.Bitmap)img;
            var lck = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bytes = new byte[Math.Abs(lck.Stride) * lck.Height];
            Marshal.Copy(lck.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(lck);
            for (int i = 0; i < bytes.Length; i += 4)
            {
                //swap r and b bytes
                byte r = bytes[i];
                byte b = bytes[i + 2];
                bytes[i] = b;
                bytes[i + 2] = r;
            }
            var tex2 = new Texture2D(device, bmp.Width, bmp.Height);
            tex2.SetData<byte>(bytes);
            return tex2;
        }

        public override Vector2 MeasureString(TextControlFontStyle style, string text, TextAlignment alignment = TextAlignment.TopLeft, int maxwidth = int.MaxValue)
        {
            switch (style)
            {
                case TextControlFontStyle.Header1:
                    return TextRenderer.MeasureText(text, _head1, maxwidth, alignment, Plex.Engine.TextRenderers.WrapMode.Words);
                case TextControlFontStyle.Header2:
                    return TextRenderer.MeasureText(text, _head2, maxwidth, alignment, Plex.Engine.TextRenderers.WrapMode.Words);
                case TextControlFontStyle.Header3:
                    return TextRenderer.MeasureText(text, _head3, maxwidth, alignment, Plex.Engine.TextRenderers.WrapMode.Words);
                case TextControlFontStyle.System:
                    return TextRenderer.MeasureText(text, _systemsans, maxwidth, alignment, Plex.Engine.TextRenderers.WrapMode.Words);
            }
            return _mono.MeasureString(text);
        }

        private void RenderTextInternal(GraphicsContext gfx, string text, int x, int y, int w, int h, SpriteFont font, Color color)
        {
            //TODO: alignment
            int _cx = 0;
            int _cy = 0;
            int _lnheight = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\r':
                        _cx = 0;
                        break;
                    case '\n':
                        _cy += _lnheight;
                        break;
                    case '\v':
                    case '\t':
                        //TODO: tab support.
                        continue;
                    default:
                        var measure = font.MeasureString(c.ToString());
                        _lnheight = Math.Max(_lnheight, (int)measure.Y);
                        if(_cx + (int)measure.X > w)
                        {
                            _cx = 0;
                            _cy += _lnheight;
                        }
                        gfx.Batch.DrawString(font, c.ToString(), new Vector2(x+_cx, y+_cy), color);
                        _cx += (int)measure.X;
                        break;
                }
            }
        }

        private bool IsMondaInstalled()
        {
            var fCollection = new System.Drawing.Text.InstalledFontCollection();
            return fCollection.Families.FirstOrDefault(x => x.Name == "Monda") != null;
        }

        private bool IsComicSansInstalled()
        {
            var fCollection = new System.Drawing.Text.InstalledFontCollection();
            return fCollection.Families.FirstOrDefault(x => x.Name == "Comic Sans MS") != null;
        }


        public override void DrawButtonText(GraphicsContext gfx, string text, int x, int y, int width, int height, ButtonState state)
        {
            var c = _buttonTextIdle;
            if (state == ButtonState.MouseHover)
                c = _buttonTextHover;
            if (state == ButtonState.MouseDown)
                c = _buttonTextDown;
            gfx.DrawString(text, x, y, c, _systemsans, TextAlignment.Middle, width);
        }

        public override void DrawButtonImage(GraphicsContext gfx, int x, int y, int width, int height, ButtonState state, Texture2D image)
        {
            var c = _buttonTextIdle;
            if (state == ButtonState.MouseHover)
                c = _buttonTextHover;
            if (state == ButtonState.MouseDown)
                c = _buttonTextDown;
            gfx.DrawRectangle(x, y, width, height, image, c);
        }

        public override Rectangle GetTitleButtonRectangle(TitleButton button, int windowWidth, int windowHeight)
        {
            int _tby = (_titleheight - _titlebuttonsize) / 2;
            int _closex = (windowWidth - _titlebuttonsize) - _borderright;
            int _maxx = (_closex - _titlebuttonsize) - 4;
            int _minx = (_maxx - _titlebuttonsize) - 4;
            switch (button)
            {
                default:
                    return new Rectangle(_closex, _tby, _titlebuttonsize, _titlebuttonsize);
                case TitleButton.Minimize:
                    return new Rectangle(_minx, _tby, _titlebuttonsize, _titlebuttonsize);
                case TitleButton.Maximize:
                    return new Rectangle(_maxx, _tby, _titlebuttonsize, _titlebuttonsize);
            }
        }

        public override Color GetAccentColor()
        {
            return this._borderBG;
        }

        public override void DrawStatedString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style, ButtonState state)
        {
            var c = _buttonTextIdle;
            if (state == ButtonState.MouseHover)
                c = _buttonTextHover;
            if (state == ButtonState.MouseDown)
                c = _buttonTextDown;
            drawStringWithColor(graphics, text, x, y, width, height, style, c);
        }

        public override void DrawDisabledString(GraphicsContext graphics, string text, int x, int y, int width, int height, TextControlFontStyle style)
        {
            drawStringWithColor(graphics, text, x, y, width, height, style, Color.Gray);
        }
    }
}
