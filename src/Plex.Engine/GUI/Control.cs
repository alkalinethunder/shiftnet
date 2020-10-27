﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine.GraphicsSubsystem;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Plex.Engine.Theming;

namespace Plex.Engine.GUI
{
    public abstract class Control : IDisposable
    {
        private int _x = 0;
        private int _y = 0;
        private int _w = 1;
        private int _h = 1;
        private Control _parent = null;
        private List<Control> _children = new List<Control>();
        private bool _wasMouseInControl = false;
        private bool _leftState = false;
        private bool _rightState = false;
        private bool _middleState = false;
        private bool _visible = true;
        private DockStyle _dock = DockStyle.None;
        private bool _autoSize = false;
        private double _opacity = 1.0;
        private bool _invalidated = true;
        private Anchor _anchor = null;
        private int _mouseX = 0;
        private int _mouseY = 0;
        private bool _captureMouse = false;
        private int _maxwidth = int.MaxValue;
        private int _minwidth = 1;
        private int _maxheight = int.MaxValue;
        private int _minheight = 1;

        protected virtual void OnDisposing()
        {

        }

        public void Dispose()
        {
            OnDisposing();
            while(_children.Count > 0)
            {
                var child = _children[0];
                child.Dispose();
                _children.Remove(child);
                child = null;
            }
            _children.Clear();
            
        }

        public int MinWidth
        {
            get
            {
                return _minwidth;
            }
            set
            {
                if (value == _minwidth)
                    return;
                if (value < 1)
                    value = 1;
                _minwidth = value;
                Width = Width;
            }
        }

        public int MaxWidth
        {
            get
            {
                return _maxwidth;
            }
            set
            {
                if (value == 0)
                    value = int.MaxValue;
                if (value == _maxwidth)
                    return;
                _maxwidth = value;
                Width = Width;
            }
        }

        public int MinHeight
        {
            get
            {
                return _minheight;
            }
            set
            {
                if (value == _minheight)
                    return;
                if (value < 1)
                    value = 1;
                _minheight = value;
                Height = Height;
            }
        }

        public int MaxHeight
        {
            get
            {
                return _maxheight;
            }
            set
            {
                if (value == 0)
                    value = int.MaxValue;
                if (value == _maxheight)
                    return;
                _maxheight = value;
                Height = Height;
            }
        }


        public void BringToFront()
        {
            if(_parent != null)
            {
                _parent._children.Remove(this);
                _parent.AddControl(this);
            }
            else
            {
                UIManager.BringToFront(this);
            }
        }

        public bool RequiresPaint
        {
            get
            {
                bool requires_child_repaint = false;
                foreach (var child in _children)
                {
                    requires_child_repaint = child.RequiresPaint;
                    if (requires_child_repaint)
                        break;
                }
                if (requires_child_repaint)
                    _invalidated = true;
                return _invalidated || requires_child_repaint;
            }
        }

        public bool CaptureMouse
        {
            get
            {
                return _captureMouse;
            }
            set
            {
                _captureMouse = value;
            }
        }

        public int MouseX
        {
            get
            {
                return _mouseX;
            }
        }

        public int MouseY
        {
            get
            {
                return _mouseY;
            }
        }


        public Anchor Anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                if (_anchor == value)
                    return;

                _anchor = value;
                Invalidate();
            }
        }

        public void Invalidate()
        {
            _invalidated = true;
            foreach (var child in _children)
                child.Invalidate();
        }

        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                if (_opacity == value)
                    return;
                _opacity = value;
                Invalidate();
            }
        }

        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                _autoSize = value;
            }
        }

        public DockStyle Dock
        {
            get
            {
                return _dock;
            }
            set
            {
                _dock = value;
            }
        }

        public bool ContainsMouse
        {
            get { return _wasMouseInControl; }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (_visible == value)
                    return;

                _visible = value;
                Invalidate();
            }
        }

        public void AddControl(Control ctrl)
        {
            if (!_children.Contains(ctrl))
            {
                ctrl._parent = this;
                _children.Add(ctrl);
                Invalidate();
                UIManager.FocusedControl = ctrl;
            }
        }

        public bool MouseLeftDown
        {
            get
            {
                return _leftState;
            }
        }

        public bool MouseMiddleDown
        {
            get
            {
                return _middleState;
            }
        }

        public bool MouseRightDown
        {
            get
            {
                return _rightState;
            }
        }



        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x == value)
                    return;
                _x = value;
                Invalidate();
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y == value)
                    return;
                _y = value;
                Invalidate();
            }
        }

        public int Width
        {
            get
            {
                return _w;
            }
            set
            {
                value = MathHelper.Clamp(value, _minwidth, _maxwidth);
                if (_w == value)
                    return;
                _w = value;
                Invalidate();
            }
        }

        public int Height
        {
            get
            {
                return _h;
            }
            set
            {
                value = MathHelper.Clamp(value, _minheight, _maxheight);
                if (_h == value)
                    return;
                _h = value;
                Invalidate();
            }
        }

        public Control Parent
        {
            get
            {
                return _parent;
            }
        }

        public Control[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }

        public Point PointToParent(int x, int y)
        {
            return new Point(x + _x, y + _y);
        }

        public Point PointToScreen(int x, int y)
        {
            var parentCoords = new Point(X + x, Y + y);
            Control parent = this._parent;
            while(parent != null)
            {
                parentCoords = parent.PointToParent(parentCoords.X, parentCoords.Y);
                parent = parent.Parent;
            }
            return parentCoords;
        }

        public void ClearControls()
        {
            _children.Clear();
            Invalidate(); 
        }

        public void RemoveControl(Control ctrl)
        {
            if(_children.Contains(ctrl))
            {
                _children.Remove(ctrl);
                ctrl._parent = null;
                Invalidate();
            }
        }

        public Point PointToLocal(int x, int y)
        {
            return new GUI.Point(x - _x, y - _y);
        }

        public virtual void MouseStateChanged() { }

        protected virtual void OnPaint(GraphicsContext gfx, RenderTarget2D target)
        {
            ThemeManager.Theme.DrawControlBG(gfx, 0, 0, Width, Height);
        }

        public void SendToBack()
        {
            if(_parent != null)
            {
                _parent._children.Remove(this);
                _parent._children.Insert(0, this);
            }
            else
            {
                UIManager.SendToBack(this);
            }
        }

        public void InvalidateTopLevel()
        {
            var parent = this;
            while (parent.Parent != null)
                parent = parent.Parent;
            parent.Invalidate();
        }

        protected virtual void BeforePaint(GraphicsContext gfx, RenderTarget2D target)
        {

        }

        protected virtual void AfterPaint(GraphicsContext gfx, RenderTarget2D target)
        {

        }

        public readonly RasterizerState RasterizerState = new RasterizerState { ScissorTestEnable = true };

        public void Paint(GraphicsContext gfx, RenderTarget2D target)
        {
            gfx.Batch.End();
            var oldScissor = gfx.Device.ScissorRectangle;
            gfx.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                                    SamplerState.LinearClamp, UIManager.GraphicsDevice.DepthStencilState,
                                    RasterizerState);
            

            if (_visible == true)
            {
                BeforePaint(gfx, target);
                //In case we've got a different target...
                var rt = (RenderTarget2D)gfx.Device.GetRenderTargets().First().RenderTarget;

                OnPaint(gfx, rt);
                int draw_x = gfx.X;
                int draw_y = gfx.Y;
                int draw_width = gfx.Width;
                int draw_height = gfx.Height;
                foreach (var ctrl in _children)
                {
                    if (ctrl.Visible == true)
                    {
                        gfx.X = draw_x + ctrl.X;
                        gfx.Y = draw_y + ctrl.Y;
                        gfx.Width = ctrl.Width;
                        gfx.Height = ctrl.Height;
                        ctrl.Paint(gfx, rt);
                        gfx.X = draw_x;
                        gfx.Y = draw_y;
                    }
                    gfx.Width = draw_width;
                    gfx.Height = draw_height;
                }
                //this lets us reset the target.
                AfterPaint(gfx, target);
                _invalidated = false;
            }
        }

        public void Layout(GameTime gameTime)
        {
            //Dock style
            if(_parent != null)
            {
                if(_anchor != null)
                {
                    
                }

                switch (_dock)
                {
                    case DockStyle.Top:
                        X = 0;
                        Y = 0;
                        Width = _parent.Width;
                        break;
                    case DockStyle.Left:
                        X = 0;
                        Y = 0;
                        Height = _parent.Height;
                        break;
                    case DockStyle.Right:
                        Y = 0;
                        X = _parent.Width - Width;
                        Height = _parent.Height;
                        break;
                    case DockStyle.Bottom:
                        X = 0;
                        Y = _parent.Height - Height;
                        Width = _parent.Width;
                        break;
                    case DockStyle.Fill:
                        X = 0;
                        Y = 0;
                        Width = _parent.Width;
                        Height = _parent.Height;
                        break;
                }
            }
            OnLayout(gameTime);
            foreach (var child in _children)
                if(child.Visible)
                    child.Layout(gameTime);
        }

        protected virtual void OnLayout(GameTime gameTime)
        {
            //do nothing
        }

        public bool IsFocusedControl
        {
            get
            {
                return UIManager.FocusedControl == this;
            }
        }

        public bool ContainsFocusedControl
        {
            get
            {
                if (UIManager.FocusedControl == null)
                    return false;
                else
                {
                    bool contains = false;

                    var ctrl = UIManager.FocusedControl;
                    while(ctrl.Parent != null)
                    {
                        ctrl = ctrl.Parent;
                        if (ctrl == this)
                            contains = true;
                    }
                    return contains;
                }
            }
        }

        private int _lastScrollValue = 0;

        public virtual bool ProcessMouseState(MouseState state, double lastLeftClickMS, int width = 0, int height = 0)
        {
            //If we aren't rendering the control, we aren't accepting input.
            if (_visible == false)
                return false;
            int _bw = (width == 0) ? this._w : width;
            int _bh = (height == 0) ? this._h : height;
            //Firstly, we get the mouse coordinates in the local space
            var coords = PointToLocal(state.Position.X, state.Position.Y);
            bool doMove = false;
            if (_mouseX != coords.X || _mouseY != coords.Y)
                doMove = true;

            _mouseX = coords.X;
            _mouseY = coords.Y;
            //Now we check if the mouse is within the bounds of the control
            //We're in the local space. Let's fire the MouseMove event.

            if (coords.X >= 0 && coords.Y >= 0 && coords.X <= _bw && coords.Y <= _bh)
            {
                if (doMove)
                    MouseMove?.Invoke(coords);
                //Also, if the mouse hasn't been in the local space last time it moved, fire MouseEnter.
                if (_wasMouseInControl == false)
                {
                    _wasMouseInControl = true;
                    MouseEnter?.Invoke();
                    Invalidate();
                }


                //Things are going to get a bit complicated.
                //Firstly, we need to find out if we have any children.
                bool _requiresMoreWork = true;
                if (_children.Count > 0)
                {
                    //We do. We're going to iterate through them all and process the mouse state.
                    foreach (var control in _children.OrderByDescending(x => _children.IndexOf(x)))
                    {
                        //If the process method returns true, then we do not need to do anything else on our end.

                        //We need to first create a new mousestate object with the new coordinates

                        var nstate = new MouseState(coords.X, coords.Y, state.ScrollWheelValue, state.LeftButton, state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
                        //pass that state to the process method, and set the _requiresMoreWork value to the opposite of the return value
                        _requiresMoreWork = !control.ProcessMouseState(nstate, lastLeftClickMS);
                        //If it's false, break the loop.
                        if (_requiresMoreWork == false)
                            break;
                    }
                }

                //If we need to do more work...
                if (_requiresMoreWork == true)
                {
                    bool fire = false; //so we know to fire a MouseStateChanged method
                    //Let's get the state values of each button
                    bool ld = state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    bool md = state.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    bool rd = state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                    if (ld != _leftState || md != _middleState || rd != _rightState)
                    {
                        fire = true;
                    }
                    if (_leftState == true && ld == false)
                    {
                        Click?.Invoke();
                        Invalidate();
                        MouseUp?.Invoke();
                    }
                    if (_leftState == false && ld == true)
                    {
                        if (lastLeftClickMS <= 500 & lastLeftClickMS > 0)
                            DoubleClick?.Invoke();
                        var focused = UIManager.FocusedControl;
                        UIManager.FocusedControl = this;
                        focused?.InvalidateTopLevel();
                        InvalidateTopLevel();
                        MouseDown?.Invoke();

                    }
                    _leftState = ld;
                    _middleState = md;
                    _rightState = rd;
                    if (fire)
                        MouseStateChanged();
                    if (!IsFocusedControl)
                    {
                        _lastScrollValue = state.ScrollWheelValue;
                    }
                    if (state.ScrollWheelValue != _lastScrollValue && IsFocusedControl)
                    {
                        int delta = state.ScrollWheelValue - _lastScrollValue;
                        OnMouseScroll(delta);
                        MouseScroll?.Invoke(delta);
                        _lastScrollValue = state.ScrollWheelValue;
                    }

                }

                return true;
            }
            else
            {

                _leftState = false;
                _rightState = false;
                _middleState = false;
                MouseStateChanged();
                //If the mouse was in local space before, fire MouseLeave
                PropagateMouseLeave();

            }


            //Mouse is not in the local space, don't do anything.
            return false;
        }

        private void PropagateMouseLeave()
        {
            if (_wasMouseInControl == true)
            {
                MouseLeave?.Invoke();
                Invalidate();
                foreach (var ctrl in _children)
                    ctrl.PropagateMouseLeave();
            }
            _wasMouseInControl = false;
        }

        protected virtual void OnMouseScroll(int value)
        {

        }

        public event Action<int> MouseScroll;

        protected virtual void OnKeyEvent(KeyEvent e)
        {

        }

        public void ProcessKeyEvent(KeyEvent e)
        {
            OnKeyEvent(e);
            KeyEvent?.Invoke(e);
        }

        public event Action DoubleClick;
        public event Action<Point> MouseMove;
        public event Action MouseEnter;
        public event Action MouseLeave;
        public event Action Click;
        public event Action<KeyEvent> KeyEvent;
        public event Action MouseDown;
        public event Action MouseUp;
    }

    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public enum DockStyle
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Fill
    }

    [Flags]
    public enum AnchorStyle
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public class Anchor
    {
        public AnchorStyle Style { get; set; }
        public int Distance { get; set; }
    }
}
