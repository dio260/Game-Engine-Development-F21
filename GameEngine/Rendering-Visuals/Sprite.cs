using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine.Rendering
{
    public class Sprite
    {
        //Constructor
        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Position = Vector2.Zero; //or new Vector2(0,0)
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Color = Color.White;
            Rotation = 0;
            //Origin = Position; 
            //Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Origin = Vector2.Zero;
            Scale = new Vector2(1, 1);
            Effect = SpriteEffects.None;
            Layer = 1;
        }
        public Sprite(Texture2D texture, Vector2? position = null, Rectangle? source = null, Color? color = null, float rotation = 0,
                       Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, float layer = 1)
        {
            Texture = texture;

            if (position == null)
                Position = Vector2.Zero;
            else
                Position = (Vector2)position;
            if (source == null)
                Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
            else
                Source = (Rectangle)source;
            if (color == null)
                Color = Color.White;
            else
                Color = (Color)color;
            Rotation = rotation;

            if (origin == null)

                Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            else
                Origin = (Vector2)origin;
            if (scale == null)
                Scale = new Vector2(1, 1);
            else
                Scale = (Vector2)scale;
            Effect = effect;
            Layer = layer;
        }

        //Properties
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Effect { get; set; }
        public float Layer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
        }

    }
}