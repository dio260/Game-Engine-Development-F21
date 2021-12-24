using System;
using System.Collections.Generic;
using System.Text;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    public class FirstPersonPlayer : Player
    {
        public int hp;
        public float regenCD;
        Quaternion originalRot;
        public FirstPersonPlayer(TerrainRenderer terrain, ContentManager Content, Camera camera,
                    GraphicsDevice graphicsDevice, Light light,
                    Model model, string shading, Texture2D texture) 
                : base(terrain, Content, camera, graphicsDevice, light, model, shading, texture)
        {
            originalRot = Transform.Rotation;
            hp = 100;
            regenCD = 10;
        }

        public override void Update()
        {
            // Control the player

            if(Get<Camera>() != null)
            {
                //if (InputManager.IsKeyDown(Keys.Up)) { Transform.Rotate(Vector3.Right, Time.ElapsedGameTime); }
                //if (InputManager.IsKeyDown(Keys.Down)) { Transform.Rotate(Vector3.Left, Time.ElapsedGameTime); }
                if (InputManager.IsKeyDown(Keys.Left)) { Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 1); }
                if (InputManager.IsKeyDown(Keys.Right)) { Transform.Rotate(Vector3.Down, Time.ElapsedGameTime * 1); }
                //if (InputManager.IsKeyPressed(Keys.LeftControl)) { Transform.Rotation = originalRot; }
            }

            if(hp < 100)
            {
                if(regenCD >= 10)
                {
                    hp += 10;
                    regenCD = 0;
                }
                else
                {
                    regenCD += 0.1f;
                }
            }
            else
            {
                regenCD = 0;
            }

            base.Update();
        }
    }
}
