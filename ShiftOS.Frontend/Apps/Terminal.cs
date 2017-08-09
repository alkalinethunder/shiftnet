﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;
using static ShiftOS.Engine.SkinEngine;

namespace ShiftOS.Frontend.Apps
{
    [FileHandler("Shell script", ".trm", "fileicontrm")]
    [Launcher("{TITLE_TERMINAL}", false, null, "{AL_UTILITIES}")]
    [WinOpen("{WO_TERMINAL}")]
    [DefaultTitle("{TITLE_TERMINAL}")]
    [DefaultIcon("iconTerminal")]
    public class Terminal : GUI.Control, IShiftOSWindow
    {
        private TerminalControl _terminal = null;
        
        public TerminalControl TerminalControl
        {
            get
            {
                return _terminal;
            }
        }

        public Terminal()
        {
            Width = 493;
            Height = 295;
        }

        public void OnLoad()
        {
            _terminal = new Apps.TerminalControl();
            _terminal.Dock = GUI.DockStyle.Fill;
            AddControl(_terminal);
            AppearanceManager.ConsoleOut = _terminal;
            AppearanceManager.StartConsoleOut();
            TerminalBackend.PrintPrompt();
            SaveSystem.GameReady += () =>
            {
                Console.WriteLine("[sessionmgr] Starting system UI...");
                AppearanceManager.SetupWindow(new SystemStatus());
                TerminalBackend.PrintPrompt();
            };
        }

        protected override void OnLayout(GameTime gameTime)
        {
            if (ContainsFocusedControl || IsFocusedControl)
                AppearanceManager.ConsoleOut = _terminal;
        }

        public void OnSkinLoad()
        {
            
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }

    public class TerminalControl : GUI.TextInput, ITerminalWidget
    {
        private int _zoomFactor = 1;

        public TerminalControl()
        {
            Dock = GUI.DockStyle.Fill;
            
        }
        
        private static readonly string[] delimiters = { Environment.NewLine };

        public string[] Lines
        {
            get
            {
				return Text.Split(delimiters, StringSplitOptions.None);
            }
        }

        public void Clear()
        {
            Text = "";
            Index = 0;
            _vertOffset = 0;
            Invalidate();
        }

        public void SelectBottom()
        {
           Index = Text.Length - 1;
            RecalculateLayout();
            InvalidateTopLevel();
        }

        

        public void Write(string text)
        {
            Engine.Desktop.InvokeOnWorkerThread(() =>
            {
                Text += Localization.Parse(text);
                SelectBottom();
                Index += text.Length;
                RecalculateLayout();
                InvalidateTopLevel();
            });
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        
        private static readonly Regex regexNl = new Regex(Regex.Escape(Environment.NewLine));
        
        public int GetCurrentLine()
        {
            return regexNl.Matches(Text.Substring(0, Index)).Count;
        }

        float _vertOffset = 0.0f;

        protected void RecalculateLayout()
        {
            var cloc = GetPointAtIndex();
            var csize = GraphicsContext.MeasureString("#", new Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style));
            if (cloc.Y - _vertOffset < 0)
            {
                _vertOffset += cloc.Y - _vertOffset;
            }
            while ((cloc.Y + csize.Y) - _vertOffset > Height)
            {
                _vertOffset += csize.Y;
            }
        }

        private bool blinkStatus = false;
        private double blinkTime = 0.0;

        protected override void OnLayout(GameTime gameTime)
        {
            blinkTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (blinkTime > 500.0)
                blinkTime = 0;
            bool prev = blinkStatus;
            blinkStatus = blinkTime > 250.0;
            if (prev != blinkStatus)
                Invalidate();
        }

        public bool ReadOnly = false;

        /// <summary>
        /// Gets the X and Y coordinates (in pixels) of the caret.
        /// </summary>
        /// <param name="gfx">A <see cref="System.Drawing.Graphics"/> object used for font measurements</param>
        /// <returns>the correct position of the d*ng caret. yw</returns>
        public System.Drawing.Point GetPointAtIndex()
        {
			if (string.IsNullOrEmpty(Text))
                return new System.Drawing.Point(2, 2);
            var font = new Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style);
            int currline = GetCurrentLine();
            string substring = String.Join(Environment.NewLine, Lines.Take(currline + 1));
			int h = (int)Math.Round(GraphicsContext.MeasureString(substring, font, Width).Y - font.Height);

            int linestart = String.Join(Environment.NewLine, Lines.Take(GetCurrentLine())).Length;

            var lineMeasure = GraphicsContext.MeasureString(Text.Substring(linestart, Index - linestart), font);
            int w = (int)Math.Floor(lineMeasure.X);
            while (w > Width)
            {
                w -= Width;
                h += (int)lineMeasure.Y;
            }
            return new System.Drawing.Point(w, h);
        }

        private PointF CaretPosition = new PointF(2, 2);
        private Size CaretSize = new Size(2, 15);
        private bool doEnter = true;

        protected override void OnKeyEvent(KeyEvent a)
        {
            if (a.Key != Keys.Enter)
                doEnter = true;
            if (a.ControlDown && (a.Key == Keys.OemPlus || a.Key == Keys.Add))
            {
                _zoomFactor *= 2;
                RecalculateLayout();
                Invalidate();
                return;
            }

            if (a.ControlDown && (a.Key == Keys.OemMinus || a.Key == Keys.Subtract))
            {
                _zoomFactor = Math.Max(1, _zoomFactor / 2);
                RecalculateLayout();
                Invalidate();
                return;
            }


            if (a.Key == Keys.Enter && !ReadOnly)
            {
                if (doEnter == true)
                {
                    if (!PerformTerminalBehaviours)
                    {
                        Text = Text.Insert(Index, Environment.NewLine);
                        Index += 2;
                        RecalculateLayout();
                        Invalidate();
                        return;
                    }

                    try
                    {
                        if (!TerminalBackend.PrefixEnabled)
                        {
                            string textraw = Lines[Lines.Length - 1];
                            TerminalBackend.SendText(textraw);
                            return;
                        }
                        var text = Lines;
                        var text2 = text[text.Length - 1];
                        var text3 = "";
                        var text4 = Regex.Replace(text2, @"\t|\n|\r", "");
                        WriteLine("");

                        if (TerminalBackend.PrefixEnabled)
                        {
                            text3 = text4.Remove(0, TerminalBackend.ShellOverride.Length);
                        }
                        if (!string.IsNullOrWhiteSpace(text3))
                        {
                            TerminalBackend.LastCommand = text3;
                            TerminalBackend.SendText(text4);
                            if (TerminalBackend.InStory == false)
                            {
                                {
                                    var result = SkinEngine.LoadedSkin.CurrentParser.ParseCommand(text3);

                                    if (result.Equals(default(KeyValuePair<string, Dictionary<string, string>>)))
                                    {
                                        Console.WriteLine("{ERR_SYNTAXERROR}");
                                    }
                                    else
                                    {
                                        TerminalBackend.InvokeCommand(result.Key, result.Value);
                                    }

                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (TerminalBackend.PrefixEnabled)
                        {
                            TerminalBackend.PrintPrompt();
                        }
                        AppearanceManager.CurrentPosition = 0;

                    }
                    doEnter = false;
                }
            }
            else if (a.Key == Keys.Back && !ReadOnly)
            {
                try
                {
                    if (PerformTerminalBehaviours)
                    {
                        var tostring3 = Lines[Lines.Length - 1];
                        var tostringlen = tostring3.Length + 1;
                        var workaround = TerminalBackend.ShellOverride;
                        var derp = workaround.Length + 1;
                        if (tostringlen != derp)
                        {
                            AppearanceManager.CurrentPosition--;
                            base.OnKeyEvent(a);
                            RecalculateLayout();
                            InvalidateTopLevel();
                        }
                    }
                    else
                    {
                        base.OnKeyEvent(a);
                        RecalculateLayout();
                        InvalidateTopLevel();
                    }
                }
                catch
                {
                    Debug.WriteLine("Drunky alert in terminal.");
                }
            }
            else if (a.Key == Keys.Right)
            {
                if (Index < Text.Length)
                {
                    Index++;
                    AppearanceManager.CurrentPosition++;
                    RecalculateLayout();
                    InvalidateTopLevel();
                }
            }
            else if (a.Key == Keys.Left)
            {
                if (SaveSystem.CurrentSave != null)
                {
                    var getstring = Lines[Lines.Length - 1];
                    var stringlen = getstring.Length + 1;
                    var header = TerminalBackend.ShellOverride;
                    var headerlen = header.Length + 1;
                    var selstart = Index;
                    var remstrlen = Text.Length - stringlen;
                    var finalnum = selstart - remstrlen;
                    if (!PerformTerminalBehaviours)
                        headerlen = 0;
                    if (finalnum > headerlen)
                    {
                        AppearanceManager.CurrentPosition--;
                        base.OnKeyEvent(a);
                    }
                }
            }
            else if (a.Key == Keys.Up && PerformTerminalBehaviours)
            {
                var tostring3 = Lines[Lines.Length - 1];
                if (tostring3 == TerminalBackend.ShellOverride)
                    Console.Write(TerminalBackend.LastCommand);
                ConsoleEx.OnFlush?.Invoke();
                return;

            }
            else
            {
                if ((PerformTerminalBehaviours && TerminalBackend.InStory) || ReadOnly)
                {
                    return;
                }
                if (a.KeyChar != '\0')
                {
                    Text = Text.Insert(Index, a.KeyChar.ToString());
                    Index++;
                    AppearanceManager.CurrentPosition++;
                    //                    RecalculateLayout();
                    InvalidateTopLevel();
                }
            }
            blinkStatus = true;
            blinkTime = 250;
        }

        public bool PerformTerminalBehaviours = true;

        protected override void OnPaint(GraphicsContext gfx)
        {
            var font = new System.Drawing.Font(LoadedSkin.TerminalFont.Name, LoadedSkin.TerminalFont.Size * _zoomFactor, LoadedSkin.TerminalFont.Style); 
            gfx.Clear(LoadedSkin.TerminalBackColorCC.ToColor().ToMonoColor());
            if (!string.IsNullOrEmpty(Text))
            {
                //Draw the caret.
                if (blinkStatus == true)
                {
                    PointF cursorPos = GetPointAtIndex();
                    string caret = (Index < Text.Length) ? Text[Index].ToString() : " ";
                    var cursorSize = GraphicsContext.MeasureString(caret, font);

                    var lineMeasure = GraphicsContext.MeasureString(Lines[GetCurrentLine()], font);
                    if (cursorPos.X > lineMeasure.X)
                    {
                        cursorPos.X = lineMeasure.X;
                    }

                    gfx.DrawRectangle((int)cursorPos.X, (int)(cursorPos.Y - _vertOffset), (int)cursorSize.X, (int)cursorSize.Y, LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor());
                }
                //Draw the text

                int textloc = 0 - (int)_vertOffset;
                foreach (var line in Lines)
                {
                    if(!(textloc < 0 || textloc - font.Height >= Height))
                        gfx.DrawString(line, 0, textloc, LoadedSkin.TerminalForeColorCC.ToColor().ToMonoColor(), font, Width - 4);
                    textloc += font.Height;
                }

            }
        }

    }

    public static class ConsoleColorExtensions
    {
        public static System.Drawing.Color ToColor(this ConsoleColor cc)
        {
            switch (cc)
            {
                case ConsoleColor.Black:
                    return System.Drawing.Color.Black;
                case ConsoleColor.Blue:
                    return System.Drawing.Color.Blue;
                case ConsoleColor.Cyan:
                    return System.Drawing.Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return System.Drawing.Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return System.Drawing.Color.DarkCyan;
                case ConsoleColor.DarkGray:
                    return System.Drawing.Color.DarkGray;
                case ConsoleColor.DarkGreen:
                    return System.Drawing.Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return System.Drawing.Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return System.Drawing.Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return System.Drawing.Color.Orange;
                case ConsoleColor.Gray:
                    return System.Drawing.Color.Gray;
                case ConsoleColor.Green:
                    return System.Drawing.Color.Green;
                case ConsoleColor.Magenta:
                    return System.Drawing.Color.Magenta;
                case ConsoleColor.Red:
                    return System.Drawing.Color.Red;
                case ConsoleColor.White:
                    return System.Drawing.Color.White;
                case ConsoleColor.Yellow:
                    return System.Drawing.Color.Yellow;
            }
            return System.Drawing.Color.Empty;
        }
    }

    public static class GraphicsExtensions
    {

        [Obsolete("Use GraphicsContext.MeasureString instead")]
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font, int width)
        {
            var measure = System.Windows.Forms.TextRenderer.MeasureText(s, font, new Size(width, int.MaxValue));
            return measure;
        }

        [Obsolete("Use GraphicsContext.MeasureString instead")]
        public static SizeF SmartMeasureString(this Graphics gfx, string s, Font font)
        {
            return SmartMeasureString(gfx, s, font, int.MaxValue);
        }

    }
}
