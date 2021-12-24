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
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }

        public int moveSpeed;

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera,
                    GraphicsDevice graphicsDevice, Light light,
                    Model model, string shading, Texture2D texture) : base()
        {

            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // Add other component required for Player
            SphereCollider sphere = new SphereCollider();
            sphere.Radius = model.Meshes[0].BoundingSphere.Radius;//1f * Transform.LocalScale.Y;
            sphere.Transform = Transform;
            Renderer renderer = new Renderer(model, Transform, camera, Content,
                graphicsDevice, light, 1, shading, 20f, texture);
            Add<Collider>(sphere);
            Add<Renderer>(renderer);

            moveSpeed = 10;

        }

        public override void Update()
        {
            // Control the player

            

            if (InputManager.IsKeyDown(Keys.W)) // move forward
                if(Terrain.GetAltitude(this.Transform.LocalPosition + Transform.Forward * Time.ElapsedGameTime * moveSpeed) <= 1)
                    this.Transform.LocalPosition += Transform.Forward * Time.ElapsedGameTime * moveSpeed;      
            if (InputManager.IsKeyDown(Keys.S)) // move backwars
                if (Terrain.GetAltitude(this.Transform.LocalPosition + Transform.Back * Time.ElapsedGameTime * moveSpeed) <= 1)
                    this.Transform.LocalPosition += Transform.Back * Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.A)) // move forward
                if (Terrain.GetAltitude(this.Transform.LocalPosition + Transform.Left * Time.ElapsedGameTime * moveSpeed) <= 1)
                    this.Transform.LocalPosition += Transform.Left * Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.D)) // move backwars
                if (Terrain.GetAltitude(this.Transform.LocalPosition + Transform.Right * Time.ElapsedGameTime * moveSpeed) <= 1)
                    this.Transform.LocalPosition += Transform.Right * Time.ElapsedGameTime * moveSpeed;

            // change the Y position corresponding to the terrain (maze)
            this.Transform.LocalPosition = new Vector3(
                this.Transform.LocalPosition.X,
                Terrain.GetAltitude(this.Transform.LocalPosition),
                this.Transform.LocalPosition.Z) + Vector3.Up;

            base.Update();
        }

    }
}