// #define NOSAVE

//#define ONLINEMODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;
using static System.Net.Mime.MediaTypeNames;
using static Whoa.Whoa;

namespace Plex.Engine
{
    /// <summary>
    /// Management class for the Shiftnet save system.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// Boolean representing whether the system is shutting down.
        /// </summary>
        public static bool ShuttingDown = false;

        /// <summary>
        /// Boolean representing whether the save system is ready to be used.
        /// </summary>
        public static AutoResetEvent Ready = new AutoResetEvent(false);
        public static bool IsSandbox = false;

        /// <summary>
        /// Occurs before the save system connects to the Shiftnet Digital Society.
        /// </summary>
        public static event Action PreDigitalSocietyConnection;

        /// <summary>
        /// Start the entire Shiftnet engine.
        /// </summary>
        /// <param name="useDefaultUI">Whether Shiftnet should initiate it's Windows Forms front-end.</param>
        public static void Begin(bool useDefaultUI = true)
        {
            FSUtils.CreateMountIfNotExists();

            Paths.Init();
            TerminalBackend.PopulateTerminalCommands();
            Ready.Reset();

            if (PreDigitalSocietyConnection != null)
            {
                PreDigitalSocietyConnection?.Invoke();
                Ready.WaitOne();
            }

            FinishBootstrap();

            //Nothing happens past this point - but the client IS connected! It shouldn't be stuck in that while loop above.

        }

        public static string GetUsername()
        {
            using(var sstr = new ServerStream(ServerMessageType.USR_GETUSERNAME))
            {
                var result = sstr.Send();
                if(result.Message == 0x00)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadString();
                    }
                }
            }
            return null;
        }
        
        public static ulong GetExperience()
        {
            using (var sstr = new ServerStream(ServerMessageType.USR_GETXP))
            {
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadUInt64();
                    }
                }
            }
            return 0;
        }

        public static long GetCash()
        {
            using (var sstr = new ServerStream(ServerMessageType.USR_GETCASH))
            {
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadInt64();
                    }
                }
            }
            return 0;
        }

        public static void CompleteStory(string id)
        {
            using (var sstr = new ServerStream(ServerMessageType.SP_COMPLETESTORY))
            {
                sstr.Write(id);
                sstr.Send();
            }
        }

        public static void AddExperience(ulong value)
        {
            using (var sstr = new ServerStream(ServerMessageType.USR_ADDXP))
            {
                sstr.Write(value);
                sstr.Send();
            }
        }

        public static void SetStoryPickup(string id)
        {
            using (var sstr = new ServerStream(ServerMessageType.SP_SETPICKUP))
            {
                sstr.Write(id);
                sstr.Send();
            }
        }

        public static string GetSystemName()
        {
            using (var sstr = new ServerStream(ServerMessageType.USR_GETSYSNAME))
            {
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadString();
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Finish bootstrapping the engine.
        /// </summary>
        private static void FinishBootstrap()
        {
            Desktop.InvokeOnWorkerThread(new Action(() => Desktop.PopulateAppLauncher()));
            GameReady?.Invoke();
        }

        /// <summary>
        /// Delegate type for events with no caller objects or event arguments. You can use the () => {...} (C#) lambda expression with this delegate 
        /// </summary>
        public delegate void EmptyEventHandler();

        /// <summary>
        /// Occurs when the engine is loaded and the game can take over.
        /// </summary>
        public static event EmptyEventHandler GameReady;
    }

    /// <summary>
    /// Delegate for handling Terminal text input.
    /// </summary>
    /// <param name="text">The text inputted by the user (including prompt text).</param>
    public delegate void TextSentEventHandler(string text);
}
