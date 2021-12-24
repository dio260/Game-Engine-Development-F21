using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine.Labs_Assignments
{
    public class enemy2D : AnimatedSprite
    {
        public Texture2D left, right;

        public bool Dead;
        public float respawnCooldown, moveSpeed;
        public int deadTimer;
        public GraphicsDevice g;

        int ai;

        public Sprite Player;
        Random rand = new Random();
        public enemy2D(Texture2D texture, GraphicsDevice graphicsDevice, Sprite player, int AI) : base(texture)
        {
            Player = player;
            Dead = false;
            respawnCooldown = 0;
            moveSpeed = 30;
            ai = AI;
            g = graphicsDevice;

        }

        public override void Update()
        {
            if(Dead)//respawn time.
            {
                
                respawnCooldown += 0.1f;
                if(respawnCooldown >= deadTimer)
                {

                    Dead = false;
                    respawnCooldown = 0;
                    Spawn();
                }
            }
            else //AI
            {
                switch(ai)
                {
                    case 0: // homing
                        if (Position.X > Player.Position.X)
                        {
                            Texture = left;
                            Position += new Vector2(-1 * Time.ElapsedGameTime, 0) * moveSpeed;
                        }
                        else
                        {
                            Texture = right;
                            Position += new Vector2(Time.ElapsedGameTime, 0) * moveSpeed;
                        }

                        if (Position.Y > Player.Position.Y)
                        {
                            Position += new Vector2(0, -1 * Time.ElapsedGameTime) * moveSpeed;
                        }
                        else
                        {
                            Position += new Vector2(0, Time.ElapsedGameTime) * moveSpeed;
                        }
                        break;
                    case 1: //one directional horizontal
                        if (Texture == left)
                        {
                            if(Position.X >= -16)
                            {
                                Position += new Vector2(-1 * Time.ElapsedGameTime, 0) * moveSpeed;
                            }
                            else
                            {
                                Texture = right;
                            }
                            
                        }
                        else
                        {
                            if (Position.X <= g.Viewport.Width)
                            {
                                Position += new Vector2(Time.ElapsedGameTime, 0) * moveSpeed;
                            }
                            else
                            {
                                Texture = left;
                            }
                            
                        }

                        break;
                    case 2: // one directional vertical
                        if (Texture == left)
                        {
                            if (Position.Y >= 0)
                            {
                                Position += new Vector2(0, -1 * Time.ElapsedGameTime) * moveSpeed;
                            }
                            else
                            {
                                Texture = right;
                            }

                        }
                        else
                        {
                            if (Position.Y <= g.Viewport.Height)
                            {
                                Position += new Vector2(0, Time.ElapsedGameTime) * moveSpeed;
                            }
                            else
                            {
                                Texture = left;
                            }

                        }
                        break;
                }

            }
            base.Update();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Dead)
                base.Draw(spriteBatch);
        }

        public void Spawn()
        {
            switch(ai)
            {
                case 0:
                    switch (rand.Next(1, 5))
                    {
                        case 1: //upper level of screen
                            Position = new Vector2(rand.Next(0, g.Viewport.Width), -16);
                            break;
                        case 2: //left side of screen
                            Position = new Vector2(-16, rand.Next(0, g.Viewport.Height));
                            break;
                        case 3: //right side of screen
                            Position = new Vector2(g.Viewport.Width + 16,
                                                    rand.Next(0, g.Viewport.Height));
                            break;
                        case 4: //bottom level of screen
                            Position = new Vector2(rand.Next(0, g.Viewport.Width),
                                                    g.Viewport.Height + 16);
                            break;
                    }
                    break;
                case 1:
                    if (rand.Next(0, 2) == 0)
                    {

                        Position = new Vector2(-16, Position.Y);
                        Texture = right;
                    }
                    else
                    {
                        Position = new Vector2(g.Viewport.Width, Position.Y);
                        Texture = left;
                    }
                    break;
                case 2:
                    if (rand.Next(0, 2) == 0)
                    {

                        Position = new Vector2(Position.X, 0);
                        Texture = right;
                    }
                    else
                    {
                        Position = new Vector2(Position.X, g.Viewport.Height);
                        Texture = left;
                    }
                    break;

            }
            
        }
    }
}
