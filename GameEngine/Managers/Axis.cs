using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class Axis
    {
        public Keys Positive { get; set; }
        public Keys Negative { get; set; }
        public float Value;

        public Axis()
        {
            Value = 0;
        }

        public virtual void Update()
        {
            if (InputManager.IsKeyDown(Positive))
                Value += 0.05f;
            else if (InputManager.IsKeyDown(Negative))
                Value -= 0.05f;
            else
                Value = 0;
            Value = MathHelper.Clamp(Value, -1, 1);
        }
    }

    
}
