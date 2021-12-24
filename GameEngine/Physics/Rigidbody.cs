using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Managers;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine.Physics
{
    public class Rigidbody : Component, IUpdateable
    {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }
        public Vector3 CurrentPos { get; private set; }
        public Vector3 NextPos { get; private set; }
        public void Update()
        {
            Velocity += Acceleration * Time.ElapsedGameTime + Impulse / Mass;
            Transform.LocalPosition += Velocity * Time.ElapsedGameTime;
            Impulse = Vector3.Zero;
            //CurrentPos = 
        }

    }
}
