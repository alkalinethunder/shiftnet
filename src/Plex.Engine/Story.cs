using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Engine
{
    public class StoryContext
    {
        public string Id { get; set; }
        public MethodInfo Method { get; set; }
        public bool AutoComplete = false;

        public StoryContext()
        {
            AutoComplete = true;
        }

        public void MarkComplete()
        {
            SaveSystem.CompleteStory(Id);
            OnComplete?.Invoke();
        }

        public event Action OnComplete;
    }

    public class Objective
    {
        private Func<bool> _completeFunc = null;

        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsComplete
        {
            get
            {
                return (bool)_completeFunc?.Invoke();
            }
        }

        public Objective(string name, string desc, Func<bool> completeFunc, Action onComplete)
        {
            _completeFunc = completeFunc;
            Name = name;
            Description = desc;
            this.onComplete = onComplete;
        }

        private Action onComplete = null;

        public void Complete()
        {
            Thread.Sleep(20);
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Storyline management class.
    /// </summary>
    public static class Story
    {
        public static event Action ObjectiveStarted;
        public static event Action ObjectiveComplete;

        public static StoryContext Context { get; private set; }
        public static event Action<string> StoryComplete;
        public static List<Objective> CurrentObjectives { get; private set; }
        public static event Action<string> FailureRequested;
        public static event Action<MissionAttribute> MissionComplete;

        public static void DisplayFailure(string message)
        {
            FailureRequested?.Invoke(message);
        }



        public static void PushObjective(string name, string desc, Func<bool> completeFunc, Action onComplete)
        {
            if (CurrentObjectives == null)
                CurrentObjectives = new List<Objective>();

            var currentObjective = new Objective(name, desc, completeFunc, onComplete);
            CurrentObjectives.Add(currentObjective);
            var t = new Thread(() =>
            {
                ObjectiveStarted?.Invoke();
                var obj = currentObjective;
                while (!obj.IsComplete)
                {
                    Thread.Sleep(5000);
                }
                Thread.Sleep(500);
                CurrentObjectives.Remove(obj);
                obj.Complete();
                ObjectiveComplete?.Invoke();
            });
            t.IsBackground = true;
            t.Start();
        }
        
        
        /// <summary>
        /// Starts the storyline with the specified Storyline ID.
        /// </summary>
        /// <param name="stid">The storyline ID to start.</param>
        public static void Start(string stid)
        {
            if (SaveSystem.IsSandbox)
                return;

            foreach (var type in ReflectMan.Types)
            {
                foreach (var mth in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    foreach (var attrib in Array.FindAll(mth.GetCustomAttributes(false), a => a is StoryAttribute || a is MissionAttribute))
                    {
                        var story = attrib as StoryAttribute;
                        if (story.StoryID == stid)
                        {
                            new Thread(() =>
                            {
                                Context = new Engine.StoryContext
                                {
                                    Id = stid,
                                    Method = mth,
                                    AutoComplete = true,
                                };
                                SaveSystem.SetStoryPickup(Context.Id);
                                Context.OnComplete += () =>
                                {
                                    if (story is MissionAttribute)
                                    {
                                        var mission = story as MissionAttribute;
                                        SaveSystem.AddExperience(mission.CodepointAward);
                                        TerminalBackend.PrefixEnabled = true;
                                        TerminalBackend.InStory = false;
                                        MissionComplete?.Invoke(mission);
                                    }
                                    StoryComplete?.Invoke(stid);
                                    SaveSystem.SetStoryPickup("");
                                };
                                mth.Invoke(null, null);
                                if (Context.AutoComplete)
                                {
                                    Context.MarkComplete();
                                }
                            }).Start();
                            return;
                        }
                    }
                }

            }
#if DEBUG
            throw new ArgumentException("Story ID not found: " + stid + " - Talk to Michael. NOW.");
#else
            Debug.Print("No such story: " + stid);
#endif
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StoryAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StoryAttribute"/> attribute. 
        /// </summary>
        /// <param name="id">The ID of this story plot.</param>
        /// <remarks>
        ///     <para>
        ///         The <see cref="StoryAttribute"/> is used to turn a static, public method into a story element. Using the specified <paramref name="id"/> argument, the Shiftnet Engine can determine whether this plot has already been experienced, and using the <see cref="Upgrades"/> classes, the ID is treated as a special Shiftorium upgrade, and you can use the <see cref="RequiresUpgradeAttribute"/> attribute as well as the various other ways of determining whether a Shiftorium upgrade is installed to determine if this plot has been experienced.        
        /// </para>
        /// </remarks>
        public StoryAttribute(string id)
        {
            StoryID = id;
        }

        /// <summary>
        /// Gets the storyline ID stored in this attribute.
        /// </summary>
        public string StoryID { get; private set; }

        public ulong CodepointAward { get; protected set; }
    }
}
