﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EitherMouse
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UInt32 Sensitivity { get; set; }
        public UInt32 DoubleClickSpeed { get; set; }
        public UInt32 ScrollSpeed { get; set; }

        public Device(string name, UInt32 sensitivity, UInt32 doubleClickSpeed, UInt32 scrollSpeed)
        {
            this.Id = -1;
            this.Name = name;
            this.Sensitivity = sensitivity;
            this.DoubleClickSpeed = doubleClickSpeed;
            this.ScrollSpeed = scrollSpeed;
        }

        public Device()
        {
            this.Name = "Default";
            this.Sensitivity = 10;
            this.DoubleClickSpeed = 500;
            this.ScrollSpeed = 3;
        }

        public Device(string name)
        {
            this.Name = name;
            this.Sensitivity = 10;
            this.DoubleClickSpeed = 500;
            this.ScrollSpeed = 3;
        }
    }
}