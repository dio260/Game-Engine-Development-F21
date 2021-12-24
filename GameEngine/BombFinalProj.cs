using System;
using System.Collections.Generic;
using System.Threading;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace CPI311.GameEngine
{
    public class BombFinalProj : GameObject
    {
        public AStarSearch search;
        public List<Vector3> path;

        public float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        FirstPersonPlayer player;


        bool running;

        public BombFinalProj(TerrainRenderer terrain, ContentManager Content,
                    Camera camera, GraphicsDevice graphicsDevice, Light light,
                    string shading, Texture2D texture, FirstPersonPlayer player) : base()
        {
            this.player = player;
            Transform.Scale = Vector3.One * 0.3f;

            Terrain = terrain;
            path = null;
            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.Size.X / gridSize;
            float gridH = Terrain.Size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.Size.X / 2,
                                                0, gridH * j + gridH / 2 - Terrain.Size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[j, i].Passable = false;
                }

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // Add other component required for Player
            SphereCollider sphere = new SphereCollider();
            sphere.Radius = Content.Load<Model>("sphere").Meshes[0].BoundingSphere.Radius * 0.3f;
            sphere.Transform = Transform;
            Renderer renderer = new Renderer(Content.Load<Model>("sphere"), Transform, camera, Content,
                graphicsDevice, light, 1, shading, 20f, texture);
            foreach (ModelMesh mesh in renderer.ObjectModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DiffuseColor = Color.Black.ToVector3();
                    effect.EnableDefaultLighting();
                }

            }
            Add<Collider>(sphere);
            Add<Renderer>(renderer);

            

            //PlayerPathFinding();

        }


        public override void Update()
        {
            if (Transform.Position.X > player.Transform.Position.X)
                Transform.Position += -1 * Vector3.UnitX * Time.ElapsedGameTime * speed;
            else
                Transform.Position += Vector3.UnitX * Time.ElapsedGameTime * speed;

            if (Transform.Position.Z > player.Transform.Position.Z)
                Transform.Position += -1 * Vector3.UnitZ * Time.ElapsedGameTime * speed;
            else
                Transform.Position += Vector3.UnitZ * Time.ElapsedGameTime * speed;

            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;
            Transform.Update();
            base.Update();
        }
        public Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.Size.X / search.Cols;
            float gridH = Terrain.Size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.Size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.Size.Y / 2);
            //return Vector3.Zero;
        }

    }
}
