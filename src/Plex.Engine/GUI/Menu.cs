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
    public class Menu : TextControl
    {
        private List<MenuItem> _childMenus = new List<MenuItem>();
        private string _emptyText = "<empty>";
        private int _textheight = 0;
        private int _textwidth = 0;
        private int _border = 0;
        private MenuItem _dropdown = null;
        private int _selectedIndex = -1;
        private int _selectedX = 0;
        private int _selectedY = 0;
        private int _selectedW = 0;
        private int _selectedH = 0;
        private int _imageMargin = 24;
        private int _textPaddingLeft = 3;

        public bool HasDropdown
        {
            get
            {
                return _childMenus.Count != 0;
            }
        }

        protected virtual void CalculateSelectedItem(int x, int y)
        {
            int _itemcount = (Height - _border) / _textheight;
            for (int i = 0; i < _itemcount; i++)
            {
                int i_y = (i * _textheight) + (_border / 2);
                int i_b = i_y + _textheight;
                if (y >= i_y && y <= i_b)
                {
                    _selectedIndex = i;
                    _selectedX = 0;
                    _selectedY = i_y;
                    _selectedW = Width;
                    _selectedH = _textheight;
                    RequireTextRerender();
                    Invalidate();
                    return;
                }
            }

        }

        protected virtual void OnClick()
        {
            if (_selectedIndex != -1 && _childMenus.Count != 0)
            {
                var item = _childMenus[_selectedIndex];
                if (item.Enabled == false)
                    return;
                if (item.HasDropdown == true)
                {
                    if (_dropdown != null)
                    {
                        _dropdown.Hide();
                        _dropdown = null;
                    }
                    _dropdown = item;
                    int _ddX = X + Width;
                    int _ddY = Y + (_border / 2) + (_textheight * _selectedIndex);
                    _dropdown.Layout(new GameTime());
                    int vpw = UIManager.Viewport.Width;
                    int vph = UIManager.Viewport.Height;
                    if (_ddY + _dropdown.Height >= vph)
                    {
                        int diff = (_ddY + _dropdown.Height) - vph;
                        _ddY -= diff;
                    }
                    if (_ddX + _dropdown.Width >= vpw)
                    {
                        _ddX = X - _dropdown.Width;
                    }
                    _dropdown.X = _ddX;
                    _dropdown.Y = _ddY;
                    _dropdown.Show();
                    UIManager.FocusedControl = this;
                }
                else
                {
                    item.Activate();
                    Hide();
                }
            }
            else
            {
                Hide();
            }

        }

        public void SetDropdown(int x, int y, MenuItem item)
        {
            if(_dropdown != null)
            {
                _dropdown.Hide();
                _dropdown = null;
            }
            _dropdown = item;
            _dropdown.X = x;
            _dropdown.Y = y;
            _dropdown.Layout(new GameTime());
            _dropdown.Show();
        }


        public void Select(int i, int x, int y, int w, int h)
        {
            _selectedH = h;
            _selectedW = w;
            _selectedX = x;
            _selectedY = y;
            _selectedIndex = i;
            Invalidate();
            RequireTextRerender();
        }

        public Menu()
        {
            AutoSize = true;
            Visible = false;
            MouseEnter += () =>
            {
                UIManager.FocusedControl = this;
            };
            MouseLeave += () =>
            {
                _selectedIndex = -1;
                _selectedX = 0;
                _selectedY = 0;
                _selectedH = 0;
                _selectedW = 0;
                RequireTextRerender();
                Invalidate();
            };
            MouseMove += (loc) =>
            {
                int x = loc.X;
                int y = loc.Y;
                CalculateSelectedItem(x, y);
            };
            Click += () =>
            {
                OnClick();
            };
            MenuItem.EnabledChanged += MenuItem_EnabledChanged;
        }

        private void MenuItem_EnabledChanged(MenuItem obj)
        {
            if(this._childMenus.Contains(obj)&&Visible)
            {
                Select(-1, 0, 0, 0, 0);
                RequireTextRerender();
                Invalidate();
            }
        }

        private bool _pbg = true;
        public bool PaintBG
        {
            get
            {
                return _pbg;
            }
            set
            {
                _pbg = value;
            }
        }

        public void ClearItems()
        {
            _childMenus.Clear();
            _selectedIndex = -1;
            _selectedX = 0;
            _selectedY = 0;
            _selectedW = 0;
            _selectedH = 0;
            RequireTextRerender();
            Invalidate();
        }

        
        public void AddItem(MenuItem item)
        {
            if (_childMenus.Contains(item))
                return;
            _childMenus.Add(item);
            _selectedIndex = -1;
            _selectedX = 0;
            _selectedY = 0;
            _selectedW = 0;
            _selectedH = 0;
            RequireTextRerender();
            Invalidate();

        }

        public void RemoveItem(MenuItem item)
        {
            if (!_childMenus.Contains(item))
                return;
            _childMenus.Remove(item);
            _selectedIndex = -1;
            _selectedX = 0;
            _selectedY = 0;
            _selectedW = 0;
            _selectedH = 0;
            RequireTextRerender();
            Invalidate();
        }

        protected override void RenderText(GraphicsContext gfx)
        {
            int text_x = (_border / 2) + _imageMargin + _textPaddingLeft;
            int text_y = (_border / 2);
            if(_childMenus.Count == 0)
            {
                Theming.ThemeManager.Theme.DrawString(gfx, _emptyText, text_x, text_y, int.MaxValue, int.MaxValue, TextControlFontStyle.System);
            }
            else
            {
                for(int i = 0; i < _childMenus.Count; i++)
                {
                    bool selected = i == _selectedIndex;
                    var state = Theming.ButtonState.Idle;
                    if (selected)
                        state = Theming.ButtonState.MouseHover;
                    if (_childMenus[i].Enabled == false)
                    {
                        Theming.ThemeManager.Theme.DrawDisabledString(gfx, _childMenus[i].Text, text_x, text_y, int.MaxValue, int.MaxValue, TextControlFontStyle.System);
                    }
                    else
                    {
                        Theming.ThemeManager.Theme.DrawStatedString(gfx, _childMenus[i].Text, text_x, text_y, int.MaxValue, int.MaxValue, TextControlFontStyle.System, state);
                    }
                    text_y += _textheight;
                }
            }
        }

        protected override void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            if (PaintBG)
            {
                Theming.ThemeManager.Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
                Theming.ThemeManager.Theme.DrawControlBG(gfx, 0, 0, _border + _imageMargin, Height);
                var accent = Theming.ThemeManager.Theme.GetAccentColor();
                gfx.DrawRectangle(_selectedX, _selectedY, _selectedW, _selectedH, accent);
                for (int i = 0; i < _childMenus.Count; i++)
                {
                    var dd = _childMenus[i];
                    if (dd.HasDropdown && dd.Enabled == true)
                    {
                        int ddy = (_border / 2) + (_textheight * i);
                        int ddh = _textheight;
                        int ddw = 16;
                        int ddx = Width - ddw;

                        Theming.ThemeManager.Theme.DrawArrow(gfx, ddx, ddy, ddw, ddh, (i == _selectedIndex) ? Theming.ButtonState.MouseHover : Theming.ButtonState.MouseDown, Theming.ArrowDirection.Right);
                    }
                }
            }
            base.OnPaint(gfx, target);
        }

        public int SelectedX
        {
            get
            {
                return _selectedX;
            }
        }

        public int SelectedY
        {
            get
            {
                return _selectedY;
            }
        }

        public int SelectedW
        {
            get
            {
                return _selectedW;
            }
        }

        public int SelectedH
        {
            get
            {
                return _selectedH;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
        }

        public int Border
        {
            get
            {
                return _border;
            }
        }

        public MenuItem[] MenuItems
        {
            get
            {
                return _childMenus.ToArray();
            }
        }

        protected override void OnLayout(GameTime gameTime)
        {
            FontStyle = TextControlFontStyle.Custom;
            TextColor = Color.White;
            
            //Ignore min/max width
            MaxWidth = int.MaxValue;
            MaxHeight = int.MaxValue;
            MinWidth = 1;
            MinHeight = 1;

            if (this.TextRerenderRequired)
            {
                string longest = (_childMenus.Count == 0) ? _emptyText : GetLongestString();
                var measure = Theming.ThemeManager.Theme.MeasureString(TextControlFontStyle.System, longest);
                _textwidth = (int)measure.X;
                _textheight = (int)measure.Y;
            }
            Width = _border + _imageMargin + _textPaddingLeft + _textwidth + 50;
            Height = _border + (Math.Max(_textheight, _textheight * _childMenus.Count));
        }

        public string GetLongestString()
        {
            string str = "";
            foreach (var item in _childMenus)
                if (Math.Max(item.Text.Length, str.Length) == item.Text.Length)
                    str = item.Text;
            return str;
            
        }

        public void Show()
        {
            RequireTextRerender();
            Visible = true;
            UIManager.AddTopLevel(this);
        }

        public void Hide()
        {
            Visible = false;
            if(_dropdown != null)
            {
                _dropdown.Hide();
                _dropdown = null;
            }
            UIManager.StopHandling(this);

        }
    }

    public class MenuItem : Menu
    {
        public object Tag { get; set; }
        public MenuItem()
        {
            Enabled = true;
        }

        public void Activate()
        {
            ItemActivated?.Invoke();
        }

        private bool _enabled = true;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                EnabledChanged?.Invoke(this);
            }
        }

        
        public event Action ItemActivated;
        public static event Action<MenuItem> EnabledChanged;
    }
}
