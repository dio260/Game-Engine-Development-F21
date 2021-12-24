using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {   
        public Color BarColor { get; set; }
        public float Value { get; set; }
        public float Speed { get; set; }
        public float MaxValue { get; set;  }
        private float clampedValue;

        public ProgressBar(Texture2D texture) : base(texture)
        {
            base.Texture = texture;
            base.Scale = new Vector2(1, 1);
            //base.Source = new Rectangle(0, 0, (int)(Texture.Width * MaxValue), Texture.Height);
            Color = Color.White;
            BarColor = Color.Green;

            MaxValue = 1;
            Value = MaxValue;
            clampedValue = Value / MaxValue;
            
            Speed = 0;

            
        }

        public ProgressBar(Texture2D texture, Color? fill = null, float initValue = 1f, 
                            float maxValue = 1f, float scale = 1f, float initSpeed = 0f ) : base(texture)
        {
            base.Texture = texture;
            base.Scale = new Vector2(scale, 1);
            //base.Source = new Rectangle(0, 0, (int)(Texture.Width * MaxValue), Texture.Height);
            if (fill == null)
                BarColor = Color.Green;
            else
                BarColor = (Color)fill;
            Color = Color.White;

            Value = initValue;
            MaxValue = maxValue;
            clampedValue = Value / maxValue;

            Speed = initSpeed;

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            spriteBatch.Draw(Texture, Position, 
                new Rectangle(Source.X, Source.Y, (int)(Source.Width * clampedValue), Source.Height), 
                BarColor, Rotation, Origin, Scale, Effect, Layer);
        }

        public override void Update()
        {
            base.Update();
            Value = MathHelper.Clamp(Value + Speed, 0, MaxValue);
            clampedValue = Value / MaxValue;
            clampedValue = MathHelper.Clamp(clampedValue, 0, 1);
            
        }
        
    }
}
