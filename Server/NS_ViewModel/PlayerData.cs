using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NS_ViewModel
{
    public struct PlayerData
    {
        public int ID { get; set; }
        public int Health { get; set; }
        public int Armour { get; set; }
        public int TeamID { get; set; }
        public string Username { get; set; }
        public bool Crouching { get; set; }

        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        public struct Vector3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }
    }
}
