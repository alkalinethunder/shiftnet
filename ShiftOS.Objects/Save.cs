﻿/*
 * Project: Shiftnet
 * 
 * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only.
 * 






 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Plex.Objects.Hacking;
using Whoa;

namespace Plex.Objects
{
    //Better to store this stuff server-side so we can do some neat stuff with hacking...
    public class Save
    {
        [Order]
        public string Username = "user";
        
        [Order]
        public bool IsSandbox = false;
        
        [Order]
        public ulong Experience { get; set; }
        
        [Order]
        public Dictionary<string, bool> Upgrades { get; set; }
        [Order]
        public string SystemName { get; set; }
        [Order]
        public List<string> StoriesExperienced { get; set; }

        public int CountUpgrades()
        {
            int count = 0;
            foreach (var upg in Upgrades)
            {
                if (upg.Value == true)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// DO NOT MODIFY THIS. EVER. YOU WILL BREAK THE STORYLINE. Let the engine do it's job. 
        /// </summary>
        [Order]
        public string PickupPoint { get; set; }

        [Order]
        public List<string> LoadedUpgrades { get; set; }

        [Order]
        public int MaxLoadedUpgrades { get; set; }

        [Order]
        public int Rank { get; set; }

        [Order]
        public long Cash { get; set; }

        [Order]
        public List<CashTransaction> Transactions { get; set; }

        [Order]
        public Dictionary<string, long> NetworkTasks { get; set; }
    }

    public class UsedCredential
    {
        [Order]
        public string Address { get; set; }
        [Order]
        public int Port { get; set; }
        [Order]
        public string Username { get; set; }
        [Order]
        public string Password { get; set; }
    }

    public class HackableSystem
    {
        [Order]
        public Save SystemDescriptor { get; set; }

        [Order]
        public SystemType SystemType { get; set; }

        [Order]
        [Obsolete("Hacking rework incoming.")]
        public bool IsPwn3d { get; set; }

        [Order]
        public float X { get; set; }

        [Order]
        public float Y { get; set; }

        [Order]
        public bool IsNPC { get; set; }

        [Order]
        public string NetName { get; set; }

        [Order]
        [Obsolete("Hacking rework incoming.")]
        public List<Puzzle> Puzzles = new List<Puzzle>();

        [Order]
        [Obsolete("Hacking rework incoming.")]
        public bool HasFirewall { get; set; }

        [Order]
        public List<MountInformation> Filesystems { get; set; }

    }


    public class CashTransaction
    {
        [Order]
        public long Amount { get; set; }
        [Order]
        public string To { get; set; }
        [Order]
        public string From { get; set; }
        [Order]
        public string Date { get; set; }
    }

    public class SettingsObject : DynamicObject
    {
        private Dictionary<string, object> _settings = null;

        public SettingsObject()
        {
            _settings = new Dictionary<string, object>();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _settings.Keys.ToArray();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_settings.ContainsKey(binder.Name))
            {
                result = _settings[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                if (_settings.ContainsKey(binder.Name))
                {
                    _settings[binder.Name] = value;
                }
                else
                {
                    _settings.Add(binder.Name, value);
                }
            }
            catch
            {

            }

            return true;
        }
    }

    public class MountInformation
    {
        [Order]
        public int DriveNumber { get; set; }

        [Order]
        public DriveSpec Specification { get; set; }

        [Order]
        public string VolumeLabel { get; set; }

        [Order]
        public string ImageFilePath { get; set; }
    }

    public enum DriveSpec
    {
        ShiftFS,
        PlexFAT
    }
    
}
