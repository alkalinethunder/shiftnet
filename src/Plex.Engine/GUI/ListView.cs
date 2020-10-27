﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;

namespace Plex.Engine.GUI
{
    public class ListView : TextControl
    {
        private List<ListViewItem> _items = null;
        private Dictionary<string, Texture2D> _images = null;
        private const int _itemimagemargin = 15;
        private const int _initialmargin = 10;
        private const int _itemgap = 5;
        private int scroll = 0;
        private int defaulttexturesize = 42;
        private int _selected = -1;

        protected override void RenderText(GraphicsContext gfx)
        {
            int _itemx = _initialmargin;
            int _itemy = _initialmargin - scroll;
            int yhelper = 0;
            foreach (var item in _items)
            {
                Texture2D image = null;
                int texwidth = defaulttexturesize;
                int texheight = defaulttexturesize;
                if (!string.IsNullOrWhiteSpace(item.ImageKey))
                {
                    if (_images.ContainsKey(item.ImageKey))
                    {
                        image = _images[item.ImageKey];
                    }
                }
                int textwidth = texwidth + (_itemimagemargin * 2);
                var textmeasure = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, item.Text, TextAlignment.Top, textwidth);
                yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);

                int texty = _itemy + texheight;
                int textx = _itemx + ((textwidth - (int)textmeasure.X) / 2);

                Theming.ThemeManager.Theme.DrawString(gfx, item.Text, textx, texty, (int)textmeasure.X, (int)textmeasure.Y, TextControlFontStyle.System);
                _itemx += textwidth + _itemgap;
                if (_itemx >= (Width - (_initialmargin * 2)))
                {
                    _itemx = _initialmargin;
                    _itemy += yhelper;
                }
            }

        }

        public ListView()
        {
            _items = new List<ListViewItem>();
            _images = new Dictionary<string, Texture2D>();
            Click += () =>
            {
                    int _itemx = _initialmargin;
                    int _itemy = _initialmargin - scroll;
                    int yhelper = 0;
                foreach (var item in _items)
                {
                    Texture2D image = null;
                    int texwidth = defaulttexturesize;
                    int texheight = defaulttexturesize;
                    if (!string.IsNullOrWhiteSpace(item.ImageKey))
                    {
                        if (_images.ContainsKey(item.ImageKey))
                        {
                            image = _images[item.ImageKey];
                        }
                    }
                    int textwidth = texwidth + (_itemimagemargin * 2);
                    var textmeasure = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, item.Text, TextAlignment.Top, textwidth);
                    yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);


                    if (MouseX >= _itemx && MouseX <= _itemx + textwidth)
                    {
                        if (MouseY >= _itemy && MouseY <= _itemy + texheight + (int)textmeasure.Y)
                        {
                            _selected = _items.IndexOf(item);
                            Invalidate();
                            return;
                        }
                    }

                    _itemx += textwidth + _itemgap;
                    if (_itemx >= (Width - (_initialmargin * 2)))
                    {
                        _itemx = _initialmargin;
                        _itemy += yhelper;
                    }
                }
                
                _selected = -1;
                Invalidate();
            };
        }

        public int SelectedIndex
        {
            get
            {
                return _selected;
            }
            set
            {
                if (value == _selected)
                    return;
                _selected = MathHelper.Clamp(value, -1, _items.Count - 1);
                Invalidate();
            }
        }

        public ListViewItem SelectedItem
        {
            get
            {
                if (_selected == -1)
                    return null;
                return _items[_selected];
            }
        }

        protected override void OnTextChanged()
        {
            DontRequireTextRerender();
        }

        public void ClearItems()
        {
            _items.Clear();
            scroll = 0;
            _selected = -1;
            RequireTextRerender();

            Invalidate();
        }

        public void RemoveItem(ListViewItem item)
        {
            if (!_items.Contains(item))
                throw new ArgumentException("This list view doesn't contain that item.");
            if (_selected == _items.IndexOf(item))
                _selected = -1;
            _items.Remove(item);
            RequireTextRerender();
            Invalidate();
        }

        public void AddItem(ListViewItem item)
        {
            if (_items.Contains(item))
                throw new ArgumentException("Item already exists in this listview.");
            _items.Add(item);
            RequireTextRerender();
            Invalidate();
        }

        public void SetImage(string key, Texture2D value)
        {
            if (_images.ContainsKey(key))
                _images[key] = value;
            else
                _images.Add(key, value);
            Invalidate();
        }

        public Texture2D GetImage(string key)
        {
            if (_images.ContainsKey(key))
                return _images[key];
            return null;
        }

        public void ClearImages()
        {
            _images.Clear();
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            Theming.ThemeManager.Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            int _itemx = _initialmargin;
            int _itemy = _initialmargin - scroll;
            int yhelper = 0;
            foreach (var item in _items)
            {
                Texture2D image = null;
                int texwidth = defaulttexturesize;
                int texheight = defaulttexturesize;
                if (!string.IsNullOrWhiteSpace(item.ImageKey))
                {
                    if (_images.ContainsKey(item.ImageKey))
                    {
                        image = _images[item.ImageKey];
                    }
                }
                int textwidth = texwidth + (_itemimagemargin * 2);
                var textmeasure = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, item.Text, TextAlignment.Top, textwidth);
                yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);

                if(image != null)
                {
                    int imageDrawX = _itemx + ((textwidth - texwidth) / 2);
                    Color tint = Color.White;
                    if (_items.IndexOf(item) == _selected && (IsFocusedControl || ContainsFocusedControl))
                        tint = Theming.ThemeManager.Theme.GetAccentColor();
                    gfx.DrawRectangle(imageDrawX, _itemy, texwidth, texheight, image, tint);
                }

                int texty = _itemy + texheight;
                int textx = _itemx + ((textwidth - (int)textmeasure.X) / 2);
                if(_items.IndexOf(item) == _selected && (IsFocusedControl || ContainsFocusedControl))
                {
                    gfx.DrawRectangle(textx, texty, (int)textmeasure.X, (int)textmeasure.Y, Theming.ThemeManager.Theme.GetAccentColor());
                }
                _itemx += textwidth + _itemgap;
                if(_itemx >= (Width - (_initialmargin * 2)))
                {
                    _itemx = _initialmargin;
                    _itemy += yhelper;
                }
            }
            base.OnPaint(gfx, target);
        }

        protected override void OnLayout(GameTime gameTime)
        {
            FontStyle = TextControlFontStyle.Custom;
            TextColor = Color.White;
                        if (AutoSize)
            {
                int end_width = MinWidth;
                int end_height = MinHeight;
                int _itemx = _initialmargin;
                int _itemy = _initialmargin - scroll;
                int yhelper = 0;
                foreach (var item in _items)
                {
                    Texture2D image = null;
                    int texwidth = defaulttexturesize;
                    int texheight = defaulttexturesize;
                    if (_images.ContainsKey(item.ImageKey))
                    {
                        image = _images[item.ImageKey];
                    }
                    int textwidth = texwidth + (_itemimagemargin * 2);
                    var textmeasure = GraphicsContext.MeasureString(item.Text, Font, Engine.GUI.TextAlignment.Top, textwidth);
                    yhelper = Math.Max(yhelper, _itemy + texheight + (int)textmeasure.Y);

                    _itemx += textwidth + _itemgap;
                    if (_itemx >= (MaxWidth - (_initialmargin * 2)))
                    {
                        _itemx = _initialmargin;
                        _itemy += yhelper;
                    }
                    end_width = Math.Max(end_width, _itemx);
                    end_height = Math.Max(end_height, _itemy +yhelper);
                }
                Width = end_width;
                Height = end_height;

            }
        }
    }

    public class ListViewItem
    {
        public string Text { get; set; }
        public string Tag { get; set; }
        public string ImageKey { get; set; }

    }
}
