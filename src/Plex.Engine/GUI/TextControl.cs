﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class TextControl : Control
    {
        private string _text = "";
        private System.Drawing.Font _font = new System.Drawing.Font("Tahoma", 9f);
        private RenderTarget2D _textBuffer = null;
        bool requiresTextRerender = true;
        private TextAlignment alignment = TextAlignment.TopLeft;
        private Microsoft.Xna.Framework.Color _foreground = Microsoft.Xna.Framework.Color.Black;
        private TextControlFontStyle _fs = TextControlFontStyle.System;

        public TextAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (alignment == value)
                    return;
                alignment = value;
                RequireTextRerender();
                Invalidate();
            }
        }

        public Microsoft.Xna.Framework.Color TextColor
        {
            get
            {
                return _foreground;
            }
            set
            {
                if (_foreground == value)
                    return;
                _foreground = value;
                Invalidate();
                RequireTextRerender();
            }
        }

        public TextControlFontStyle FontStyle
        {
            get
            {
                return _fs;
            }
            set
            {
                if (_fs == value)
                    return;
                _fs = value;
                Invalidate();
                RequireTextRerender();
            }
        }

        protected override void OnDisposing()
        {
            if (_textBuffer != null)
            {
                _textBuffer.Dispose();
                _textBuffer = null;
            }
            base.OnDisposing();
        }

        public void RequireTextRerender()
        {
            requiresTextRerender = true;
        }

        public void DontRequireTextRerender()
        {
            requiresTextRerender = false;
        }

        private Vector2 measure(int maxwidth = int.MaxValue)
        {
            if(_fs == TextControlFontStyle.Custom)
            {
                return TextRenderer.MeasureText(Text, Font, maxwidth, Alignment, TextRenderers.WrapMode.Words);
            }
            else
            {
                return Theming.ThemeManager.Theme.MeasureString(_fs, Text, Alignment, maxwidth);
            }
        }

        protected virtual void RenderText(GraphicsContext gfx)
        {
            var sMeasure = measure(Width);
            Vector2 loc = new Vector2(2, 2);
            float centerH = (Width - sMeasure.X) / 2;
            float centerV = (Height - sMeasure.Y) / 2;
            switch (this.alignment)
            {
                case TextAlignment.Top:
                    loc.X = centerH;
                    break;
                case TextAlignment.TopRight:
                    loc.X = Width - sMeasure.X;
                    break;
                case TextAlignment.Left:
                    loc.Y = centerV;
                    break;
                case TextAlignment.Middle:
                    loc.Y = centerV;
                    loc.X = centerH;
                    break;
                case TextAlignment.Right:
                    loc.Y = centerV;
                    loc.X = (Width - sMeasure.Y);
                    break;
                case TextAlignment.BottomLeft:
                    loc.Y = (Height - sMeasure.Y);
                    break;
                case TextAlignment.Bottom:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = centerH;
                    break;
                case TextAlignment.BottomRight:
                    loc.Y = (Height - sMeasure.Y);
                    loc.X = (Width - sMeasure.X);
                    break;

            }

            draw(gfx, (int)loc.X, (int)loc.Y, Width);
        }

        private void draw(GraphicsContext gfx, int x, int y, int maxwidth = int.MaxValue)
        {
            if (_fs == TextControlFontStyle.Custom)
            {
                gfx.DrawString(Text, x, y, TextColor, Font, Alignment, MaxWidth, TextRenderers.WrapMode.Words);
            }
            else
            {
                Theming.ThemeManager.Theme.DrawString(gfx, Text, x, y, maxwidth, Height, _fs);
            }

        }

        public bool TextRerenderRequired
        {
            get
            {
                return this.requiresTextRerender;
            }
        }

        private bool _cleareveryredraw = true;

        /// <summary>
        /// Gets or sets a value indicating whether the text renderer should clear the text framebuffer before rerendering any text. Default value is true.
        /// </summary>
        public bool ClearTextBufferEveryRerender
        {
            get
            {
                return _cleareveryredraw;
            }
            set
            {
                if (_cleareveryredraw == value)
                    return;
                _cleareveryredraw = value;
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (AutoSize)
            {
                if (requiresTextRerender)
                {
                    var measure = this.measure(MaxWidth);
                    Width = (int)measure.X;
                    Height = (int)measure.Y;
                }
                else
                {
                    Width = _textBuffer.Width;
                    Height = _textBuffer.Height;
                }
            }
            if (requiresTextRerender)
            {
                Invalidate();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;
                _text = value;
                requiresTextRerender = true;
                OnTextChanged();
                Invalidate();
            }
        }

        public event Action TextChanged;
        protected virtual void OnTextChanged()
        {
            TextChanged?.Invoke();
        }

        public System.Drawing.Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                if (_font == value)
                    return;
                _font = value;
                requiresTextRerender = true;
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            if(_textBuffer != null)
            {
                if(_textBuffer.Width != Math.Max(1, Width) || _textBuffer.Height != Math.Max(1, Height))
                {
                    _textBuffer.Dispose();
                    _textBuffer = null;
                    RequireTextRerender();
                }
            }
            if (requiresTextRerender)
            {
                requiresTextRerender = false;
                if(_textBuffer == null) { 
                    _textBuffer = new RenderTarget2D(gfx.Device, Math.Max(1, Width), Math.Max(1, Height), false, gfx.Device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);

                }
                gfx.Batch.End();
                int x = gfx.X;
                int y = gfx.Y;
                int w = gfx.Width;
                int h = gfx.Height;
                gfx.X = 0;
                gfx.Y = 0;
                gfx.Width = Width;
                gfx.Height = Height;
                gfx.Device.SetRenderTarget(_textBuffer);
                if(_cleareveryredraw)
                    gfx.Device.Clear(Microsoft.Xna.Framework.Color.Transparent);
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);
                
                RenderText(gfx);
                gfx.Batch.End();
                gfx.Device.SetRenderTarget(target);
                gfx.Device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                gfx.X = x;
                gfx.Y = y;
                gfx.Width = w;
                gfx.Height = h;
                gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);

            }
            gfx.DrawRectangle(0, 0, Width, Height, _textBuffer, Color.White * (float)Opacity, System.Windows.Forms.ImageLayout.None, false);
        }
    }

    public enum TextControlFontStyle
    {
        System,
        Header1,
        Header2,
        Header3,
        Mono,
        Custom
    }
}
