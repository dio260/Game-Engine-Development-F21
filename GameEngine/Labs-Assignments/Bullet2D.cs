using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Bullet2D : Sprite
    {
        public float Direction;
        public bool Active, Usable;
        float Speed, ActiveTime, Cooldown;
        public Bullet2D(Texture2D texture, float direction, float speed) : base(texture)
        {
            Active = false;
            Direction = direction;
            Speed = speed;
            ActiveTime = 0;
            Cooldown = 0;
            Usable = true;
          
        }

        public override void Update()
        {
            if (!Active)
            {
                if(!Usable)
                {
                    Cooldown += 0.01f;
                    if(Cooldown > 5f)
                    {
                        Usable = true;
                        Cooldown = 0;
                    }
                }
            }
            else
            {
                Position += (Vector2.UnitX * Speed) * Direction;
            }

            

            base.Update();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Active)
                base.Draw(spriteBatch);
        }
    }
}
