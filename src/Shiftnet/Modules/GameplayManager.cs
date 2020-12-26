using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.SaveGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlkalineThunder.Pandemic.Debugging;
using AlkalineThunder.Pandemic.Scenes;
using Newtonsoft.Json;
using Shiftnet.Data;
using Shiftnet.Saves;

namespace Shiftnet.Modules
{
    [RequiresModule(typeof(SceneSystem))]
    [RequiresModule(typeof(SaveSystem))]
    [CriticalModule]
    public class GameplayManager : EngineModule
    {
        private List<CodeShopCategory> _categories = new List<CodeShopCategory>();
        private List<IShiftOS> _activeOSes = new List<IShiftOS>();
        private string _playerName;
        private List<CodeShopUpgrade> _upgrades = new List<CodeShopUpgrade>();
        private Npc[] _npcs;
        private static readonly string ContactsColumn = "contacts";
        private static readonly string UpgradesColumn = "codeshop";
        private List<ConversationInfo> _conversationEncounters = null;
        private Npc _playerNpc;
        private PlayerState _playerState;
        
        
        public event EventHandler DoNotDisturbChanged;
        
        private SceneSystem SceneSystem
            => GetModule<SceneSystem>();
        
        private SaveSystem SaveSystem
            => GetModule<SaveSystem>();

        public IEnumerable<CodeShopCategory> GetCodeShopCategories
            => _categories.OrderBy(x => x.Name);

        public bool IsGameActive
            => SaveSystem.IsGameLoaded;

        public void EndGame()
        {
            SaveSystem.UnloadGame();
            _playerState = null;
        }
        
        public event EventHandler UpgradeUnlocked;
        
        public IEnumerable<CodeShopUpgrade> GetUpgradesInCategory(CodeShopCategory category)
        {
            return _upgrades.Where(x => x.Category == category.Name).OrderBy(x => x.Name);
        }
        
        public Npc GetPlayerCharacter()
        {
            if (_playerNpc == null)
            {
                _playerNpc = new Npc
                {
                    Avatar = "",
                    Id = -1,
                    FullName = _playerName,
                    Username = "player"
                };
            }

            return _playerNpc;
        }
        
        public bool DoNotDisturb
        {
            get => _playerState.DoNotDisturb;
            set
            {
                if (_playerState.DoNotDisturb != value)
                {
                    _playerState.DoNotDisturb = value;
                    DoNotDisturbChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        
        public event EventHandler ContactAdded;

        public IEnumerable<ContactInformation> Contacts
        {
            get
            {
                if (SaveSystem.IsGameLoaded)
                {
                    using var save = SaveSystem.OpenSaveFile();

                    var contacts = save.Database.GetCollection<Contact>(ContactsColumn);

                    foreach (var contact in contacts.FindAll())
                    {
                        yield return new ContactInformation(this, contact);
                    }
                }
            }
        }
        
        public void ContinueGame()
        {
            if (SaveSystem.GetSlots().Any())
            {
                var latestSlot = SaveSystem.GetSlots().OrderByDescending(x => x.LastPlayed).First();
                SaveSystem.LoadGame(latestSlot.Slot);

                _playerName = latestSlot.Name;
                StartInternal();
            }
        }

        public ConversationInfo GetConversationInfo(string id)
        {
            return _conversationEncounters.First(x => x.Id == id);
        }

        public Npc GetNpcById(int id)
        {
            return _npcs.First(x => x.Id == id);
        }
        
        public void StartNewGame(string hostname)
        {
            if (SaveSystem.IsGameLoaded)
                throw new InvalidOperationException("Game is already active.");

            if (SaveSystem.GetSlots().Any(x => x.Name == hostname))
                throw new InvalidOperationException("Save file already exists.");

            SaveSystem.NewGame(hostname);
            _playerName = hostname;
            
            StartInternal();
        }

        private bool UnlockUpgradeInternal(string upgradeId)
        {
            using var sf = SaveSystem.OpenSaveFile();
            var collection = sf.Database.GetCollection<CodeShopUnlock>(UpgradesColumn);

            var exists = collection.FindOne(x => x.UpgradeId == upgradeId) != null;
            if (exists) return false;

            var unlock = new CodeShopUnlock
            {
                UpgradeId = upgradeId
            };

            collection.Insert(unlock);

            UpgradeUnlocked?.Invoke(this, EventArgs.Empty);
            
            return true;
        }
        
        private void LoadNpcs()
        {
            using var resource = this.GetType().Assembly.GetManifestResourceStream("Shiftnet.Resources.npcs.json");
            using var convos = this.GetType().Assembly
                .GetManifestResourceStream("Shiftnet.Resources.conversations.json");
            using var reader = new StreamReader(resource);
            using var convoReader = new StreamReader(convos);
            
            var json = reader.ReadToEnd();

            _npcs = JsonConvert.DeserializeObject<Npc[]>(json);
            _conversationEncounters = JsonConvert.DeserializeObject<List<ConversationInfo>>(convoReader.ReadToEnd());
        }

        private void LoadUpgrades()
        {
            var asm = this.GetType().Assembly;
            using var stream = asm.GetManifestResourceStream("Shiftnet.Resources.codeshop.json");

            if (stream != null)
            {
                using var reader = new StreamReader(stream);

                var json = reader.ReadToEnd();

                var lists = JsonConvert.DeserializeObject<CodeShopUpgradeList[]>(json);

                foreach (var listInfo in lists)
                {
                    _categories.Add(new CodeShopCategory(listInfo));
                    foreach (var upgradeInfo in listInfo.Upgrades)
                    {
                        var upgrade = new CodeShopUpgrade(listInfo, upgradeInfo);
                        _upgrades.Add(upgrade);
                    }
                }
            }
        }

        public (int Unlocked, int Total) GetUpgradeProgress(CodeShopCategory category)
        {
            var upgrades = GetUpgradesInCategory(category);

            using var sf = SaveSystem.OpenSaveFile();
            var collection = sf.Database.GetCollection<CodeShopUnlock>(UpgradesColumn);

            var unlockedUpgrades = collection.FindAll();

            var total = upgrades.Count();
            var unlocked = upgrades.Count(x => unlockedUpgrades.Any(y => y.UpgradeId == x.Id));

            return (unlocked, total);
        }
        
        protected override void OnLoadContent()
        {
            LoadNpcs();
            LoadUpgrades();
            base.OnLoadContent();
        }

        private DirectoryEntry[] GetChildDirectories(DirectoryEntry entry)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var directories = transaction.Database.GetCollection<DirectoryEntry>(nameof(DirectoryEntry));
            return directories.Find(x => x.ParentId == entry.Id).ToArray();
        }

        private DirectoryEntry GetDirectoryEntry(int id)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var directories = transaction.Database.GetCollection<DirectoryEntry>(nameof(DirectoryEntry));
            return directories.FindOne(x => x.Id == id);
        }

        private FileEntry[] GetChildFiles(DirectoryEntry entry)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var directories = transaction.Database.GetCollection<FileEntry>(nameof(FileEntry));
            return directories.Find(x => x.ParentId == entry.Id).ToArray();
        }

        private void DownloadPlayerState()
        {
            using var sf = SaveSystem.OpenSaveFile();
            var stateId = "playerState";

            if (sf.Database.FileStorage.Exists(stateId))
            {
                using var stream = sf.Database.FileStorage.OpenRead(stateId);
                using var reader = new StreamReader(stream);

                var json = reader.ReadToEnd();

                _playerState = JsonConvert.DeserializeObject<PlayerState>(json);
            }
            else
            {
                _playerState = new PlayerState();
            }
        }
        
        private void StartInternal()
        {
            DownloadPlayerState();
            
            var hostname = _playerName;
            FindOrCreatePlayerComputer(hostname);

            var playerOS = new PlayerOS(this);

            _activeOSes.Add(playerOS);
            
            var args = new PropertySet();
            args.SetValue("os", playerOS);

            SceneSystem.GoToScene<Desktop>(args);

        }
        
        private Network SpawnNetwork(string name)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var networks = transaction.Database.GetCollection<Network>(nameof(Network));
            var network = new Network
            {
                Name = name
            };
            networks.Insert(network);
            return network;
        }

        private DirectoryEntry SpawnDirectoryEntry(DirectoryEntry parent, string filename)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var dirs = transaction.Database.GetCollection<DirectoryEntry>(nameof(DirectoryEntry));
            var parentId = parent != null ? parent.Id : -1;

            if (parentId > -1)
            {
                if (dirs.FindOne(x => x.ParentId == parentId && x.Name == filename) != null)
                {
                    throw new InvalidOperationException("Duplicate non-root directory entry.");
                }
            }

            var directory = new DirectoryEntry
            {
                ParentId = parentId,
                Name = filename
            };

            dirs.Insert(directory);
            return directory;
        }
        
        private Computer SpawnComputer(Network network, string hostname, ComputerType type)
        {
            var rootDirectory = SpawnDirectoryEntry(null, string.Empty);
            
            using var transaction = SaveSystem.OpenSaveFile();
            var computers = transaction.Database.GetCollection<Computer>(nameof(Computer));
            var computer = new Computer
            {
                Name = hostname,
                RootDirectoryId = rootDirectory.Id,
                ComputerType = type
            };

            computers.Insert(computer);
            return computer;
        }

        private Computer GetPlayerComputer()
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var computers = transaction.Database.GetCollection<Computer>(nameof(Computer));
            return computers.FindOne(x => x.ComputerType == ComputerType.Player);
        }

        private Computer CreateInitialData(string playerName)
        {
            var playerNetwork = SpawnNetwork("Local Area Network");
            var playerComputer = SpawnComputer(playerNetwork, playerName, ComputerType.Player);

            return playerComputer;
        }

        private Computer FindOrCreatePlayerComputer(string playerName)
        {
            var existingComputer = GetPlayerComputer();
            if (existingComputer != null)
                return existingComputer;

            return CreateInitialData(playerName);
        }

        private class PlayerFileSystem : IFileSystem
        {
            private GameplayManager _gameplayManager;
            private DiskNode _rootNode;
            
            public PlayerFileSystem(GameplayManager gameplayManager)
            {
                _gameplayManager = gameplayManager;
                
                var computer = _gameplayManager.GetPlayerComputer();
                var root = _gameplayManager.GetDirectoryEntry(computer.RootDirectoryId);
                _rootNode = new DiskNode
                {
                    DirectoryEntry = root.Id,
                    Name = "/"
                };

                BuildDiskTree(_rootNode);
            }

            private void BuildDiskTree(DiskNode node)
            {
                var directory = _gameplayManager.GetDirectoryEntry(node.DirectoryEntry);
                var children = _gameplayManager.GetChildDirectories(directory);

                node.Children.Clear();
                
                foreach (var subdir in children)
                {
                    var subNode = new DiskNode
                    {
                        DirectoryEntry = subdir.Id,
                        Name = subdir.Name,
                        Parent = node
                    };

                    BuildDiskTree(subNode);

                    node.Children.Add(subNode);
                }

                foreach (var file in _gameplayManager.GetChildFiles(directory))
                {
                    var fileNode = new DiskFileNode
                    {
                        FileEntry = file.Id,
                        FileName = file.Name,
                        Parent = node
                    };

                    node.Files.Add(fileNode);
                }
            }

            private DiskNode GetDiskNodeInternal(string[] parts)
            {
                var node = _rootNode;

                foreach (var word in parts)
                {
                    if (word == ".")
                        continue;

                    if (word == "..")
                    {
                        node = node.Parent ?? node;
                        continue;
                    }

                    node = node.Children.FirstOrDefault(x => x.Name == word);

                    if (node == null)
                        break;
                }

                return node;
            }

            private DiskFileNode GetFileNodeInternal(string[] parts)
            {
                var dirExists = GetDiskNodeInternal(parts) != null;

                if (dirExists)
                    return null;

                var parentNode = GetDiskNodeInternal(parts.Take(parts.Length - 1).ToArray());

                if (parentNode == null)
                    return null;

                var fileNode = parentNode.Files.FirstOrDefault(x => x.FileName == parts.Last());
                return fileNode;
            }
            
            public bool DirectoryExists(string path)
            {
                var parts = Paths.Split(Paths.GetAbsolute(path));
                var node = GetDiskNodeInternal(parts);

                return node != null;
            }

            public bool FileExists(string path)
            {
                var parts = Paths.Split(Paths.GetAbsolute(path));
                var fileNode = GetFileNodeInternal(parts);

                return fileNode != null;
            }

            public void CreateDirectory(string directory)
            {
                var parts = Paths.Split(Paths.GetAbsolute(directory));
                var existing = GetDiskNodeInternal(parts);

                if (existing == null)
                {
                    var file = GetFileNodeInternal(parts);
                    if (file != null)
                        return;
                    
                    var parent = GetDiskNodeInternal(parts.Take(parts.Length - 1).ToArray());

                    if (parent != null)
                    {
                        var existingDirectory = _gameplayManager.GetDirectoryEntry(parent.DirectoryEntry);
                        var spawn = _gameplayManager.SpawnDirectoryEntry(existingDirectory, parts.Last());

                        var node = new DiskNode
                        {
                            DirectoryEntry = spawn.Id,
                            Name = spawn.Name,
                            Parent = parent
                        };
                        
                        parent.Children.Add(node);
                    }
                }
            }

            public void DeleteFile(string path)
            {
                throw new NotImplementedException();
            }

            public void DeleteDirectory(string path, bool recursive)
            {
                throw new NotImplementedException();
            }

            public string ReadAllText(string path)
            {
                throw new NotImplementedException();
            }

            public void WriteAllText(string path, string text)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<string> GetDirectories(string path)
            {
                var parts = Paths.Split(Paths.GetAbsolute(path));
                var dir = GetDiskNodeInternal(parts);
                
                if (dir == null)
                    throw new IOException($"The directory '{path}' does not exist.");

                foreach (var subnode in dir.Children)
                {
                    yield return Paths.Combine(path, subnode.Name);
                }
            }

            public IEnumerable<string> GetFiles(string path)
            {
                var parts = Paths.Split(Paths.GetAbsolute(path));
                var dir = GetDiskNodeInternal(parts);
                
                if (dir == null)
                    throw new IOException($"The directory '{path}' does not exist.");

                foreach (var subnode in dir.Files)
                {
                    yield return Paths.Combine(path, subnode.FileName);
                }
            }
        }

        private void AddContact(ConversationInfo info)
        {
            using var transaction = SaveSystem.OpenSaveFile();
            var contacts = transaction.Database.GetCollection<Contact>(ContactsColumn);

            if (contacts.FindOne(x => x.ConversationId == info.Id) != null)
                return;

            var contact = new Contact
            {
                ConversationId = info.Id
            };

            contacts.Insert(contact);

            App.GameLoop.Invoke(() =>
            {
                ContactAdded?.Invoke(this, EventArgs.Empty);
            });
        }

        [Exec("addContact")]
        public void Exec_AddContact(string id)
        {
            AddContact(_conversationEncounters.First(x => x.Id == id));
        }

        [Exec("getUpgrades")]
        public void Exec_GetUpgrades()
        {
            foreach (var upgrade in _upgrades)
            {
                App.Logger.Info(upgrade.Id + ": " + upgrade.Name);
            }
        }

        [Exec("unlock")]
        public void Cheat_UnlockUpgrade(string upgradeId)
        {
            var upgrade = _upgrades.First(x => x.Id == upgradeId);

            if (UnlockUpgradeInternal(upgrade.Id))
            {
                App.Logger.Log("Unlocked.");
            }
            else
            {
                App.Logger.Log("Already unlocked.");
            }
        }
        
        [Exec("doNotDisturb")]
        public void Exec_DoNotDisturb()
        {
            DoNotDisturb = !DoNotDisturb;
        }
        
        [Exec("npcs")]
        public void PrintNpcs()
        {
            foreach (var npc in _npcs)
            {
                App.Logger.Trace($"{npc.Id}: {npc.FullName} (@{npc.Username})");
            }
        }
        
        private class PlayerOS : IShiftOS
        {
            private GameplayManager _gameplayManager;
            private PlayerFileSystem _fs;
            
            public string HostName => _gameplayManager.GetPlayerComputer().Name;
            public IFileSystem FileSystem => _fs;
            public bool IsPlayer => true;
            public string Home => "/";
            
            public PlayerOS(GameplayManager manager)
            {
                _gameplayManager = manager;
                _fs = new PlayerFileSystem(_gameplayManager);
            }
        }

        [Exec("getUpgradeCategories")]
        public void Exec_GetUpgradeCategories()
        {
            foreach (var cat in _categories)
            {
                App.Logger.Log($"{cat.Id}: {cat.Name}");
            }
        }
        
        [Exec("chatEncounters")]
        public void Exec_ChatEncounters()
        {
            foreach (var encounter in _conversationEncounters)
            {
                App.Logger.Info(encounter.Id);
            }
        }
    }

    public class PlayerState
    {
        public int Experience { get; set; }
        public int SkillPoints { get; set; }
        public bool DoNotDisturb { get; set; }
    }
}