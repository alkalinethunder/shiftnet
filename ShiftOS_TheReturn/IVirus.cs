﻿using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;namespace Plex.Engine{    [Obsolete("This feature will be removed in Milestone 3.")]
    public interface IVirus    {        void Infect(int threatlevel);        void Disinfect();    }    [Obsolete("This feature will be removed in Milestone 3.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]    public class VirusAttribute : Attribute    {        public VirusAttribute(string id, string name, string desc)        {            Name = name;            ID = id;            Description = desc;        }        public string Name { get; set; }        public string Description { get; set; }        public string ID { get; set; }    }}