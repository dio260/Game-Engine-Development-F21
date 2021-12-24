using System;
using System.Collections.Generic;
using System.Threading;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace CPI311.GameEngine
{

    public class AlienFinalProj : GameObject
    {
        //A star doesnt work
        public AStarSearch search;
        public List<Vector3> path;
        Random rand;
        public float hp;
        public float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        FirstPersonPlayer player;
        GameObject hitbox;


        bool running;
        float attackCD;
        SoundEffect hit;
        public bool mute;

        public AlienFinalProj(TerrainRenderer terrain, ContentManager Content,
                    Camera camera, GraphicsDevice g, Light light,
                    string shading, Texture2D texture, FirstPersonPlayer player) : base()
        {
            this.player = player;
            rand = new Random();
            hp = 30;
            attackCD = 0;
            mute = false;

            Transform.Scale = Vector3.One * 0.0025f;
            Terrain = terrain;
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
            //Renderer renderer = new Renderer(Content.Load<Model>("sphere"), Transform, camera, Content, g, light, 1, null, 20f, texture);
            Renderer renderer = new Renderer(Content.Load<Model>("alienfbx"), Transform, camera, Content, g, light, 1, null, 20f, texture);
            foreach (ModelMesh mesh in renderer.ObjectModel.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DiffuseColor = Color.DarkGreen.ToVector3();
                }

            Add<Renderer>(renderer);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = Content.Load<Model>("sphere").Meshes[0].BoundingSphere.Radius * 0.375f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            hit = Content.Load<SoundEffect>("alienhit");
            
            switch(rand.Next(0, 4))
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
            /*
            running = true;
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(UpdatePath));
            */
            //PlayerPathFinding();

        }
        public override void Update()
        {
            /*
            if (Vector3.Distance(Transform.Position, player.Transform.Position) > 5f) //move
            {
                if (path != null && path.Count > 0)
                {

                    int directionX = (int)-((Transform.LocalPosition.X - GetGridPosition(path[0]).X) / Math.Abs(Transform.LocalPosition.X - GetGridPosition(path[0]).X));
                    int directionZ = (int)-((Transform.LocalPosition.Z - GetGridPosition(path[0]).Z) / Math.Abs(Transform.LocalPosition.Z - GetGridPosition(path[0]).Z));
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
                    PlayerPathFinding();
                }
            }
            else //attack
            {
                if (attackCD >= 5f)
                {
                    player.hp -= 10;
                    attackCD = 0;
                }
                else
                {
                    attackCD += 0.1f;
                }
            }
            */
            if (Vector3.Distance(Transform.Position, player.Transform.Position) > 1f) //move
            {
                if (Transform.Position.X > player.Transform.Position.X)
                    Transform.Position += -1 * Vector3.UnitX * Time.ElapsedGameTime * speed;
                else
                    Transform.Position += Vector3.UnitX * Time.ElapsedGameTime * speed;

                if (Transform.Position.Z > player.Transform.Position.Z)
                    Transform.Position += -1 * Vector3.UnitZ * Time.ElapsedGameTime * speed;
                else
                    Transform.Position += Vector3.UnitZ * Time.ElapsedGameTime * speed;
            }
            else //attack
            {
                if (attackCD >= 5f)
                {
                    player.hp -= 10;
                    if(!mute)
                    {
                        SoundEffectInstance hitsound = hit.CreateInstance();
                        hitsound.Volume = 0.5f;
                        hitsound.Play();
                    }
                    
                    attackCD = 0;
                }
                else
                {
                    attackCD += 0.1f;
                }
            }
            

            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up * 1.01f;
            Transform.Update();
            
            base.Update();
        }
        public override void Draw()
        {
            //hitbox.Draw();
            base.Draw();
        }

        public Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.Size.X / search.Cols;
            float gridH = Terrain.Size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.Size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.Size.Y / 2);
            //return Vector3.Zero;
        }

        public void PlayerPathFinding()
        {
            AStarNode closestBombNode = search.Nodes[search.Rows / 2, search.Cols / 2];
            float minDistance = float.MaxValue;
            for (int i = 0; i < search.Cols; i++)
            {
                for (int j = 0; j < search.Rows; j++)
                {
                    if (search.Nodes[i, j].Passable &&
                        Vector3.Distance(GetGridPosition(search.Nodes[i, j].Position), Transform.Position) < minDistance)
                    {
                        closestBombNode = search.Nodes[i, j];
                        minDistance = Vector3.Distance(GetGridPosition(closestBombNode.Position), Transform.Position);
                    }
                }
            }
            search.Start = closestBombNode;

            AStarNode closestPlayerNode = search.Nodes[search.Rows / 2, search.Cols / 2];
            
            minDistance = float.MaxValue;


            for (int i = 0; i < search.Cols; i++)
            {
                for (int j = 0; j < search.Rows; j++)
                {
                    if (search.Nodes[i, j].Passable &&
                        Vector3.Distance(GetGridPosition(search.Nodes[i, j].Position), player.Transform.Position) < minDistance)
                    {
                        closestPlayerNode = search.Nodes[i, j];
                        minDistance = Vector3.Distance(GetGridPosition(closestPlayerNode.Position), player.Transform.Position);
                    }
                }
            }

            search.End = closestPlayerNode;
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);

                current = current.Parent;

            }
            Transform.LocalPosition = GetGridPosition(path[0]);
        }

        void UpdatePath(Object obj)
        {
            while (running)
            {
                AStarNode closestBombNode = search.Nodes[search.Rows / 2, search.Cols / 2];
                float minDistance = float.MaxValue;
                for (int i = 0; i < search.Cols; i++)
                {
                    for (int j = 0; j < search.Rows; j++)
                    {
                        if (search.Nodes[i, j].Passable &&
                            Vector3.Distance(GetGridPosition(search.Nodes[i, j].Position), Transform.Position) < minDistance)
                        {
                            closestBombNode = search.Nodes[i, j];
                            minDistance = Vector3.Distance(GetGridPosition(closestBombNode.Position), Transform.Position);
                        }
                    }
                }
                search.Start = closestBombNode;

                AStarNode closestPlayerNode = search.Nodes[search.Rows / 2, search.Cols / 2];
                minDistance = float.MaxValue;


                for (int i = 0; i < search.Cols; i++)
                {
                    for (int j = 0; j < search.Rows; j++)
                    {
                        if (search.Nodes[i, j].Passable &&
                            Vector3.Distance(GetGridPosition(search.Nodes[i, j].Position), player.Transform.Position) < minDistance)
                        {
                            closestPlayerNode = search.Nodes[i, j];
                            minDistance = Vector3.Distance(GetGridPosition(closestPlayerNode.Position), player.Transform.Position);
                        }
                    }
                }

                search.End = closestPlayerNode;
                search.Search();
                path = new List<Vector3>();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);

                    current = current.Parent;

                }
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
