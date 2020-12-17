using System;
using System.Collections.Generic;
using System.Linq;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Rendering;
using AlkalineThunder.Pandemic.Skinning;
using AlkalineThunder.Pandemic.Windowing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace Shiftnet.Skinning
{
    public class CyberpunkSkinEngine : SkinEngine
    {
        private Dictionary<Type, CyberpunkPreset> _defaultPresets = new Dictionary<Type, CyberpunkPreset>();
        private int _cursorWidth = 10;
        
        protected override void OnLoad()
        {
            _defaultPresets.Add(typeof(Button), CyberpunkPreset.Highlight);
            _defaultPresets.Add(typeof(CheckBox), CyberpunkPreset.Primary);
            
            base.OnLoad();
        }

        private Color Highlight
            => GetColor("text.highlight");

        private Color PrimaryText
            => GetColor("text.primary");

        private Color SecondaryText
            => GetColor("text.secondary");

        private Color TertiaryText
            => GetColor("text.tertiary");

        private Color Container
            => GetColor("container");

        public override void DrawTextBoxBackground(Control control, SpriteRocket2D renderer, GuiButtonState state)
        {
        }

        public override void DrawTextBoxHint(Control control, SpriteRocket2D renderer, DynamicSpriteFont font, string hint)
        {
        }

        public override void DrawTextBoxText(Control control, SpriteRocket2D renderer, DynamicSpriteFont font, string text,
            float horizontalOffset)
        {
        }

        public override int SliderBarHeight => 4;
        public override int SelectListItemSpacing => 0;
        public override Padding SelectListPadding => 0;
        public override Padding CheckBoxPadding => 2;
        public override DynamicSpriteFont ConsoleFont => null;
        public override int CheckBoxSize => 14;
        public override void DrawConsoleBackground(Control control, SpriteRocket2D renderer)
        {
        }

        public override void DrawConsoleText(Control control, SpriteRocket2D renderer, DynamicSpriteFont font, string text, Vector2 position)
        {
        }

        public override void DrawTextCursor(Control control, SpriteRocket2D renderer, Vector2 position, float height)
        {
            var style = GetControlStyle(control);
            
            renderer.Begin();

            renderer.FillRectangle(new Rectangle((int) position.X, (int) position.Y, _cursorWidth, (int) height), style.TextColor);

            renderer.End();
        }

        public override void DrawSliderBar(SliderBar control, SpriteRocket2D renderer, Rectangle safeArea, GuiButtonState state)
        {
        }

        public override void DrawButtonBackground(Control control, SpriteRocket2D renderer, GuiButtonState state, bool isActive)
        {
            var style = GetControlStyle(control);

            var bg = style.Background;
            var fg = bg;

            if (state != GuiButtonState.Idle)
                fg = style.TextColor;
            if (state == GuiButtonState.Pressed)
                bg = bg.Darken(0.4f);
            
            renderer.Begin();
            renderer.FillRectangle(control.BoundingBox, bg);
            renderer.DrawRectangle(control.BoundingBox, fg, style.BorderSize);
            renderer.End();
        }

        public override void DrawBox(Control control, SpriteRocket2D renderer)
        {
        }

        public override void DrawSelectListBackground(Control control, SpriteRocket2D renderer)
        {
        }

        public override void DrawSelectListItem(Control control, SpriteRocket2D renderer, string item, Rectangle rect, bool isSelected,
            bool isHot)
        {
        }

        public override Vector2 MeasureTextBlock(TextBlock textBlock, Vector2 alottedSize)
        {
            return Vector2.Zero;
        }

        public override void DrawTextBlock(TextBlock textBlock, SpriteRocket2D renderer)
        {
        }

        public override void DrawCheck(Control control, SpriteRocket2D renderer, Rectangle safeArea, CheckState state, bool isHovered)
        {
            renderer.Begin();

            safeArea.Height = CheckBoxSize;
            
            
            var inner = safeArea;
            inner.Inflate(-4, -4);

            var style = GetControlStyle(control);

            renderer.FillRectangle(safeArea, style.Background);
            renderer.DrawRectangle(safeArea, style.TextColor, 2);

            if (state == CheckState.Checked)
                renderer.FillRectangle(inner, style.TextColor);
            else
                renderer.FillRectangle(inner, style.Border);
            
            if (isHovered)
                renderer.FillRectangle(safeArea, style.Border * 0.4f);
            
            renderer.End();
        }

        public override Vector2 MeasureListItem(Control control, string text)
        {
            return Vector2.Zero;
        }

        public override DynamicSpriteFont EditorFont => null;
        public override Vector2 MeasureWindowButton(WindowButton button)
        {
            return Vector2.Zero;
        }

        public override void DrawWindowButton(WindowButton button, SpriteRocket2D renderer, GuiButtonState state)
        {
        }

        public override void DrawIcon(Control control, SpriteRocket2D renderer, Texture2D image)
        {
        }

        public override void DrawEditorText(Control control, SpriteRocket2D renderer, DynamicSpriteFont font, string text, Vector2 position)
        {
        }

        public override void DrawEditorLineNumber(Control control, SpriteRocket2D renderer, DynamicSpriteFont font, string text,
            Vector2 position)
        {
        }

        public override void DrawEditorLineNumberArea(Control control, SpriteRocket2D renderer, Rectangle bounds)
        {
        }

        public override void DrawEditorHighlight(Control control, SpriteRocket2D renderer, Rectangle bounds)
        {
        }

        private CyberpunkPreset GetDefaultPreset(Control control)
        {
            var type = control.GetType();
            if (_defaultPresets.ContainsKey(type))
                return _defaultPresets[type];
            return CyberpunkPreset.Default;
        }

        public override ControlStyle GetControlStyle(Control control)
        {
            var cyberpunkPreset =
                control.StyleProperties.GetValue<CyberpunkPreset>("cyberpunk", GetDefaultPreset(control));
            
            var style = new ControlStyle();

            switch (cyberpunkPreset)
            {
                case CyberpunkPreset.Default:
                    style.TextColor = Highlight;
                    style.Background = Container;
                    style.Highlight = PrimaryText;
                    break;
                case CyberpunkPreset.Highlight:
                    style.Background = PrimaryText;
                    style.TextColor = Highlight;
                    
                    break;
                case CyberpunkPreset.Primary:
                    style.Background = Container;
                    style.TextColor = PrimaryText;
                    style.Border = Highlight;
                    break;
            }

            style.BorderSize = 2;
            
            return style;
        }
    }

    public enum CyberpunkPreset
    {
        Default,
        Primary,
        Secondary,
        Tertiary,
        Highlight
    }
}