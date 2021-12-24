using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        public List<Vector3> path;

        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;


        
        public Agent(TerrainRenderer terrain, ContentManager Content,
                    Camera camera, GraphicsDevice graphicsDevice, Light light,
                    Model model, string shading, Texture2D texture) : base()
        {
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
            sphere.Radius = model.Meshes[0].BoundingSphere.Radius;//1f * Transform.LocalScale.Y;
            sphere.Transform = Transform;
            Renderer renderer = new Renderer(model, Transform, camera, Content,
                graphicsDevice, light, 1, shading, 20f, texture);
            Add<Collider>(sphere);
            Add<Renderer>(renderer);

            RandomPathFinding();
            
        }




        public override void Update()
        {
            if (path != null && path.Count > 0)
            {
                int directionX = (int) -((Transform.LocalPosition.X - GetGridPosition(path[0]).X) / Math.Abs(Transform.LocalPosition.X - GetGridPosition(path[0]).X));
                int directionZ = (int) -((Transform.LocalPosition.Z - GetGridPosition(path[0]).Z) / Math.Abs(Transform.LocalPosition.Z - GetGridPosition(path[0]).Z));
                // Move to the destination along the path
                if (Math.Abs(Transform.LocalPosition.X - GetGridPosition(path[0]).X) > 0.1f)
                {
                    Rigidbody.Velocity = Vector3.UnitX * speed * directionX;
                }
                else if (Math.Abs(Transform.LocalPosition.Z - GetGridPosition(path[0]).Z) > 0.1f)
                {
                    Rigidbody.Velocity = Vector3.UnitZ * speed * directionZ;
                }


                if ((Transform.LocalPosition - (GetGridPosition(path[0]) + Vector3.Up)).Length() <= 0.1f) // if it reaches to a point, go to the next in path
                {

                    path.RemoveAt(0);
                    

                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                // Search again to make a new path.
                RandomPathFinding();
            }

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

        public void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Rows),
            random.Next(search.Cols)]).Passable) ;
            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            var count = 0;
            while (current != null)
            {
                path.Insert(0, current.Position);

                current = current.Parent;

            }
            Transform.LocalPosition = GetGridPosition(path[0]);
        }

        
    }
}
