using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public int Frames { get; set; }
        public float Frame { get; set; }
        public float Speed { get; set; }

        public int Clips { get; set; }
        public int Clip { get; set; }

        public int Animation { get; set; }

        public AnimatedSprite(Texture2D texture, int frames = 1, int frame = 0,
                        int clips = 1, int clip = 0, float speed = 1) : base(texture)
        {
            Frames = frames;
            Frame = frames;
            Speed = speed;
            Clip = clip;
            Clips = clips;

            Width = texture.Width / Frames;
            
            Height = texture.Height / clips;
        }

        //everything
        public AnimatedSprite(Texture2D texture, Vector2? position, Rectangle? source = null, Color? color = null, Single rotation = 0,
                       Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, Single layer = 0, int frames = 1)
                        : base(texture, position, source, color, rotation, origin, scale, effect, layer)
        {
            Frames = frames;
            Frame = 0;
            Speed = 1;
        }
        public override void Update()
        {
            base.Update();
            Frame = (Frame + Speed * Time.ElapsedGameTime) % Frames;
            Source = new Rectangle(Width * (int)Frame, Height * Clip, Width, Height);
        }
    }
}
