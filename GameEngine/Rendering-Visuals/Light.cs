using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine.Physics;

namespace CPI311.GameEngine.Rendering
{
    public class Light : Component
    {
        public static Light Current { get; set; }
        public static Light Default { get; set; }

        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }

        public Light()
        {
            Ambient = Color.White;
            Diffuse = Color.White;
            Specular = Color.White;
        }

        static Light()
        {
            Default = new Light();
            Default.Transform = new Transform();
            Current = Default;
        }
    }

}
