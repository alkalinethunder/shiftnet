using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.SaveGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AlkalineThunder.Pandemic.Scenes;
using Community.CsharpSqlite;
using Shiftnet.Saves;
using Network = Plex.Objects.Network;

namespace Shiftnet.Modules
{
    [RequiresModule(typeof(SceneSystem))]
    [RequiresModule(typeof(SaveSystem))]
    public class GameplayManager : EngineModule
    {
        private List<IShiftOS> _activeOSes = new List<IShiftOS>();
        private string _playerName;
        
        private SceneSystem SceneSystem
            => GetModule<SceneSystem>();
        
        private SaveSystem SaveSystem
            => GetModule<SaveSystem>();

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

        private void StartInternal()
        {
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

            public PlayerFileSystem(GameplayManager gameplayManager)
            {
                _gameplayManager = gameplayManager;
            }

            public bool DirectoryExists(string path)
            {
                throw new NotImplementedException();
            }

            public bool FileExists(string path)
            {
                throw new NotImplementedException();
            }

            public void CreateDirectory(string directory)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public IEnumerable<string> GetFiles(string path)
            {
                throw new NotImplementedException();
            }
        }
        
        private class PlayerOS : IShiftOS
        {
            private GameplayManager _gameplayManager;
            private PlayerFileSystem _fs;
            
            public string HostName => _gameplayManager.GetPlayerComputer().Name;
            public IFileSystem FileSystem => _fs;

            public PlayerOS(GameplayManager manager)
            {
                _gameplayManager = manager;
                _fs = new PlayerFileSystem(_gameplayManager);
            }
        }

    }
}