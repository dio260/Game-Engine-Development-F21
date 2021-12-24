using CPI311.GameEngine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPI311.GameEngine
{
    public class Component
    {
            public GameObject GameObject { get; set; }
            public Transform Transform { get; set; } 
        public virtual void Draw()
        {

        }
    }
}

