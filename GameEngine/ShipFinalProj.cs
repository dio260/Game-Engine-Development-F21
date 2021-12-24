using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;

namespace CPI311.GameEngine
{
    public class ShipFinalProj : GameObject
    {
        Random rand;
        public float hp;
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        FirstPersonPlayer player;

        public float shootTimer;
        public ShipFinalProj(TerrainRenderer terrain, ContentManager Content,
                    Camera camera, GraphicsDevice g, Light light,
                    string shading, Texture2D texture, FirstPersonPlayer player) : base()
        {
            rand = new Random();
            hp = 100;
            shootTimer = 0;

            Transform.Scale = Vector3.One * 0.0025f;
            Terrain = terrain;
            float gridW = Terrain.Size.X / gridSize;
            float gridH = Terrain.Size.Y / gridSize;

            this.player = player;

            Transform.Scale = Vector3.One * 0.005f;
            Transform.LocalPosition += Vector3.Up;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            //Add renderer
            
            Renderer renderer = new Renderer(Content.Load<Model>("ship"), Transform,
                                camera, Content, g, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            //add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            switch (rand.Next(0, 4))
            {
                case 0:
                    Transform.LocalPosition = new Vector3(rand.Next(0, 41), 0, rand.Next(0, 41));
                    break;
                case 1:
                    Transform.LocalPosition = new Vector3(rand.Next(-41, 0), 0, rand.Next(0, 41));
                    break;
                case 2:
                    Transform.LocalPosition = new Vector3(rand.Next(0, 41), 0, rand.Next(-41, 0));
                    break;
                case 3:
                    Transform.LocalPosition = new Vector3(rand.Next(-41, 0), 0, rand.Next(-41, 0));
                    break;
            }
            Transform.LocalPosition = new Vector3(
               Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               Transform.LocalPosition.Z) + Vector3.Up;
        }
        public override void Update()
        {
            Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (shootTimer < 3)
            {
                shootTimer += 0.01f;
            }
            base.Update();

        }
    }
}
