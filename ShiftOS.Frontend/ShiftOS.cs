﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Frontend.GraphicsSubsystem;

namespace ShiftOS.Frontend
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShiftOS : Game
    {
        internal GraphicsDeviceManager graphicsDevice;
        SpriteBatch spriteBatch;

        private bool isFailing = false;
        private double failFadeInMS = 0;
        private const double failFadeMaxMS = 500;
        private string failMessage = "";
        private string failRealMessage = "";
        private double failFadeOutMS = 0;
        private bool failEnded = false;
        private double failCharAddMS = 0;

        private bool DisplayDebugInfo = false;

        private KeyboardListener keyboardListener = new KeyboardListener ();

        public ShiftOS()
        {
            Story.FailureRequested += (message) =>
            {
                failMessage = "";
                failRealMessage = message;
                isFailing = true;
                failFadeInMS = 0;
                failFadeOutMS = 0;
                failEnded = false;
            };
            graphicsDevice = new GraphicsDeviceManager(this);
            var uconf = Objects.UserConfig.Get();
            graphicsDevice.PreferredBackBufferHeight = uconf.ScreenHeight;
            graphicsDevice.PreferredBackBufferWidth = uconf.ScreenWidth;
            SkinEngine.SkinLoaded += () =>
            {
                UIManager.ResetSkinTextures(GraphicsDevice);
                UIManager.InvalidateAll();
            };
            UIManager.Viewport = new System.Drawing.Size(
                    graphicsDevice.PreferredBackBufferWidth,
                    graphicsDevice.PreferredBackBufferHeight
                );

            Content.RootDirectory = "Content";


            //Make window borderless
            Window.IsBorderless = false;

            //Set the title
            Window.Title = "ShiftOS";



            //Fullscreen
            graphicsDevice.IsFullScreen = uconf.Fullscreen;

            // keyboard events
            keyboardListener.KeyPressed += KeyboardListener_KeyPressed;

            UIManager.Init(this);
        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {

            if (e.Key == Keys.F11)
            {
                UIManager.Fullscreen = !UIManager.Fullscreen;
            }
            else if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.D)
            {
                DisplayDebugInfo = !DisplayDebugInfo;
            }
            else if (e.Modifiers.HasFlag(KeyboardModifiers.Control) && e.Key == Keys.E)
            {
                UIManager.ExperimentalEffects = !UIManager.ExperimentalEffects;
            }
            else
            {
                // Notice: I would personally recommend just using KeyboardEventArgs instead of KeyEvent
                // from now on, but what ever. -phath0m
                UIManager.ProcessKeyEvent(new KeyEvent(e));
            }
        }
        

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            OutOfBoxExperience.Init(new MonoGameOOBE());

            //Before we do ANYTHING, we've got to initiate the ShiftOS engine.
            UIManager.GraphicsDevice = GraphicsDevice;

            //Let's get localization going.
            Localization.RegisterProvider(new MonoGameLanguageProvider());

            //First things first, let's initiate the window manager.
            AppearanceManager.Initiate(new Desktop.WindowManager());
            //Cool. Now the engine's window management system talks to us.

            //Also initiate the desktop
            Engine.Desktop.Init(new Desktop.Desktop());

            //While we're having a damn initiation fuckfest, let's get the hacking engine running.
            Hacking.Initiate();

            //Now we can initiate the Infobox subsystem
            Engine.Infobox.Init(new Infobox());

            

            //Let's initiate the engine just for a ha.

            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Apps.Terminal());
            };

            FileSkimmerBackend.Init(new MGFSLayer());



            //Create a main menu
            var mm = new MainMenu();
            UIManager.AddTopLevel(mm);

            base.Initialize();

        }

        private double timeSinceLastPurge = 0;

        private Texture2D MouseTexture = null;

         /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(base.GraphicsDevice);

            UIManager.ResetSkinTextures(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            var bmp = Properties.Resources.cursor_9x_pointer;
            var _lock = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(_lock.Stride) * _lock.Height];
            Marshal.Copy(_lock.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(_lock);
            MouseTexture = new Texture2D(GraphicsDevice, bmp.Width, bmp.Height);
            MouseTexture.SetData<byte>(rgb);
            rgb = null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            MouseTexture = null;
            // TODO: Unload any non ContentManager content here
        }
        
        private double mouseMS = 0;

        private MouseState LastMouseState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (isFailing)
            {
                if (failFadeInMS < failFadeMaxMS)
                    failFadeInMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                if(failEnded == false)
                {
                    shroudOpacity = (float)GUI.ProgressBar.linear(failFadeInMS, 0, failFadeMaxMS, 0, 1);
                    if(shroudOpacity >= 1)
                    {
                        if (failMessage == failRealMessage + "|")
                        {
                            var keydata = Keyboard.GetState();

                            if (keydata.GetPressedKeys().FirstOrDefault(x => x != Keys.None) != Keys.None)
                            {
                                failEnded = true;
                            }
                        }
                        else
                        {
                            failCharAddMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                            if (failCharAddMS >= 75)
                            {
                                failMessage = failRealMessage.Substring(0, failMessage.Length) + "|";
                                failCharAddMS = 0;
                            }
                        }
                    }
                }
                else
                {
                    if(failFadeOutMS < failFadeMaxMS)
                    {
                        failFadeOutMS += gameTime.ElapsedGameTime.TotalMilliseconds;
                    }

                    shroudOpacity = 1 - (float)GUI.ProgressBar.linear(failFadeOutMS, 0, failFadeMaxMS, 0, 1);

                    if(shroudOpacity <= 0)
                    {
                        isFailing = false;
                    }
                }
            }
            else
            {
                if (UIManager.CrossThreadOperations.Count > 0)
                {
                    var action = UIManager.CrossThreadOperations.Dequeue();
                    action?.Invoke();
                }

                //Let's get the mouse state
                var mouseState = Mouse.GetState(this.Window);
                LastMouseState = mouseState;

                UIManager.ProcessMouseState(LastMouseState, mouseMS);
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseMS = 0;
                }
                else
                {
                    mouseMS += gameTime.ElapsedGameTime.TotalMilliseconds;

                }
                //So we have mouse input, and the UI layout system working...

                //But an OS isn't useful without the keyboard!

                //Let's see how keyboard input works.

                keyboardListener.Update(gameTime);


                //Cause layout update on all elements
                UIManager.LayoutUpdate(gameTime);

                timeSinceLastPurge += gameTime.ElapsedGameTime.TotalSeconds;

                if (timeSinceLastPurge > 2)
                {
                    GraphicsContext.StringCaches.Clear();
                    timeSinceLastPurge = 0;
                    GC.Collect();
                }


                //Some hackables have a connection timeout applied to them.
                //We must update timeout values here, and disconnect if the timeout
                //hits zero.

                if (Hacking.CurrentHackable != null)
                {
                    if (Hacking.CurrentHackable.DoConnectionTimeout)
                    {
                        Hacking.CurrentHackable.MillisecondsCountdown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                        shroudOpacity = (float)GUI.ProgressBar.linear(Hacking.CurrentHackable.MillisecondsCountdown, Hacking.CurrentHackable.TotalConnectionTimeMS, 0, 0, 1);
                        if (Hacking.CurrentHackable.MillisecondsCountdown <= 0)
                        {
                            Hacking.FailHack();
                        }
                    }
                }
                else
                {
                    shroudOpacity = 0;
                }
            }

            base.Update(gameTime);
        }

        float shroudOpacity = 0.0f;

        private GUI.TextControl framerate = new GUI.TextControl();

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            UIManager.DrawControlsToTargets(GraphicsDevice, spriteBatch, 0, 0);

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            rasterizerState.MultiSampleAntiAlias = true;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,
                            SamplerState.LinearClamp, DepthStencilState.Default,
                            rasterizerState);
            //Draw the desktop BG.
            UIManager.DrawBackgroundLayer(GraphicsDevice, spriteBatch, 640, 480);

            //The desktop is drawn, now we can draw the UI.
            UIManager.DrawTArgets(spriteBatch);

            //Draw a mouse cursor



            var mousepos = Mouse.GetState(this.Window).Position;
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X+1, mousepos.Y+1, MouseTexture.Width, MouseTexture.Height), Color.Black * 0.5f);
            spriteBatch.Draw(MouseTexture, new Rectangle(mousepos.X, mousepos.Y, MouseTexture.Width, MouseTexture.Height), Color.White);

            spriteBatch.Draw(UIManager.SkinTextures["PureWhite"], new Rectangle(0, 0, UIManager.Viewport.Width, UIManager.Viewport.Height), Color.Red * shroudOpacity);

            if(isFailing && failFadeInMS >= failFadeMaxMS)
            {
                var gfx = new GraphicsContext(graphicsDevice.GraphicsDevice, spriteBatch, 0,0, UIManager.Viewport.Width, UIManager.Viewport.Height);
                string objectiveFailed = "- OBJECTIVE FAILURE -";
                string prompt = "[press any key to dismiss this message and return to your sentience]";
                int textMaxWidth = UIManager.Viewport.Width / 3;
                var topMeasure = gfx.MeasureString(objectiveFailed, SkinEngine.LoadedSkin.HeaderFont, textMaxWidth);
                var msgMeasure = gfx.MeasureString(failMessage, SkinEngine.LoadedSkin.Header3Font, textMaxWidth);
                var pMeasure = gfx.MeasureString(prompt, SkinEngine.LoadedSkin.MainFont, textMaxWidth);

                gfx.DrawString(objectiveFailed, (UIManager.Viewport.Width - (int)topMeasure.X) / 2, UIManager.Viewport.Height / 3, Color.White, SkinEngine.LoadedSkin.HeaderFont, textMaxWidth);
                gfx.DrawString(failMessage, (UIManager.Viewport.Width - (int)msgMeasure.X) / 2, (UIManager.Viewport.Height - (int)msgMeasure.Y) / 2, Color.White, SkinEngine.LoadedSkin.Header3Font, textMaxWidth);
                gfx.DrawString(prompt, (UIManager.Viewport.Width - (int)pMeasure.X) / 2, UIManager.Viewport.Height - (UIManager.Viewport.Height / 3), Color.White, SkinEngine.LoadedSkin.MainFont, textMaxWidth);
            }

            if(Hacking.CurrentHackable != null)
            {
                if (Hacking.CurrentHackable.DoConnectionTimeout)
                {
                    string str = $"Timeout in {(Hacking.CurrentHackable.MillisecondsCountdown / 1000).ToString("#.##")} seconds.";
                    var gfx = new GraphicsContext(GraphicsDevice, spriteBatch, 0, 0, UIManager.Viewport.Width, UIManager.Viewport.Height);
                    var measure = gfx.MeasureString(str, SkinEngine.LoadedSkin.HeaderFont);
                    gfx.DrawString(str, 5, (gfx.Height - ((int)measure.Y) - 5), Color.Red, SkinEngine.LoadedSkin.HeaderFont);
                }
            }

            if (DisplayDebugInfo)
            {
                var gfxContext = new GraphicsContext(GraphicsDevice, spriteBatch, 0, 0, graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight);
                var color = Color.White;
                double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
                if (fps <= 20)
                    color = Color.Red;
                gfxContext.DrawString($@"ShiftOS
=======================

Copyright (c) 2017 ShiftOS Developers

Debug information - {fps} FPS

CTRL+D: toggle debug menu
CTRL+E: toggle experimental effects (experimental effects enabled: {UIManager.ExperimentalEffects})
Use the ""debug"" Terminal Command for engine debug commands.

Red text means low framerate, a low framerate could be a sign of CPU hogging code or a memory leak.


Text cache: {GraphicsContext.StringCaches.Count}", 0, 0, color, new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Bold));
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    [ShiftoriumProvider]
    public class MonoGameShiftoriumProvider : IShiftoriumProvider
    {
        public List<ShiftoriumUpgrade> GetDefaults()
        {
            return JsonConvert.DeserializeObject<List<ShiftoriumUpgrade>>(Properties.Resources.Shiftorium);
        }
    }

    public class MGFSLayer : IFileSkimmer
    {
        public string GetFileExtension(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.CommandFormat:
                    return ".cf";
                case FileType.Executable:
                    return ".saa";
                case FileType.Filesystem:
                    return ".mfs";
                case FileType.Image:
                    return ".png";
                case FileType.JSON:
                    return ".json";
                case FileType.Lua:
                    return ".lua";
                case FileType.Python:
                    return ".py";
                case FileType.Skin:
                    return ".skn";
                case FileType.TextFile:
                    return ".txt";
                default:
                    return ".scrtm";
            }
        }

        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            throw new NotImplementedException();
        }

        public void OpenDirectory(string path)
        {
            throw new NotImplementedException();
        }
    }

}
