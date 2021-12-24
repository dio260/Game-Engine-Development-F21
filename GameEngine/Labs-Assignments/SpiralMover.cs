using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;

namespace CPI311.GameEngine
{
    public class SpiralMover
    {
        public Sprite sprite { get; set; }

        public Vector2 Position { get; set; }

        public float Radius { get; set; }
        public float Phase { get; set; }
        public float Frequency { get; set; }
        public float Amplitude { get; set; }
        public float Speed { get; set; }

        public SpiralMover(Texture2D texture, Vector2 position, float radius = 150, 
                            float speed = 1, float frequency = 20, float amplitude = 10, 
                            float phase = 0) 
        { 
            sprite = new Sprite(texture); 
            Position = position; 
            Radius = radius; 
            Speed = speed; 
            Frequency = frequency; 
            Amplitude = amplitude; 
            Phase = 0; 
            sprite.Position = Position + new Vector2(Radius, 0); 
        }
        public void Update()
        {
            Phase += Speed * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Up)) { Radius += 10 * Time.ElapsedGameTime; }
            if (InputManager.IsKeyDown(Keys.Down)) { Radius -= 10 * Time.ElapsedGameTime; }

            if (InputManager.IsKeyDown(Keys.Left)) 
            { 
                if(InputManager.IsKeyDown(Keys.LeftShift))
                {
                    Frequency += Time.ElapsedGameTime;
                }
                else
                {
                    Amplitude += Time.ElapsedGameTime;
                }
                
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    Frequency -= Time.ElapsedGameTime;
                }
                else
                {
                    Amplitude -= Time.ElapsedGameTime;
                }

            }

            sprite.Position = Position + new Vector2(
                (float)((Radius + Amplitude * Math.Cos(Phase * Frequency)) * Math.Cos(Phase)),
                (float)((Radius + Amplitude * Math.Cos(Phase * Frequency)) * Math.Sin(Phase))
                );

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);

        }
    }
}
