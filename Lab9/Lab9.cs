using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Physics;

namespace Lab9
{
    public class Lab9 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Camera cam;

        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;

        int size = 10;

        Random random = new Random();

        public Lab9()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            InputManager.Initialize();
            Time.Initialize();

            cam = new Camera();
            cam.Transform = new Transform();
            cam.Transform.LocalPosition = Vector3.Right * 5 + Vector3.Backward * 5 + Vector3.Up * 10;
            cam.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);

            search = new AStarSearch(size, size); // size of grid 
            
            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            //start to find the path
            search.Search(); // A search is made here.

            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("cube");
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            // TODO: Add your update logic here
            if(InputManager.IsKeyPressed(Keys.Space))
            {
                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                while (!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                search.Start.Passable = true; // assign a random start node (passable)
                //search.End = ???; // assign a random end node (passable)
                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                       Matrix.CreateTranslation(node.Position), cam.View, cam.Projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) *
                     Matrix.CreateTranslation(position), cam.View, cam.Projection);


            base.Draw(gameTime);
        }
    }
}
