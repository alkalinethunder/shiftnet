﻿namespace Shiftnet.Saves
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RootDirectoryId { get; set; }
        public int NetworkId { get; set; }
        public ComputerType ComputerType { get; set; } = ComputerType.Personal;
    }

    public enum ComputerType
    {
        Player,
        Personal
    }
}    