using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;
        Model model;
        Random random;

        Transform cameraTransform;
        Camera camera;


        List<GameObject> gameobjects;
        int numberCollisions = 0;
        int lastSecondCollisions;
        bool running;
        Texture2D texture, blank;
        Light light;

        Renderer normal, noTexture, current;

        BoxCollider boxCollider;

        float speed;
        bool showInfo, showColors, showTextures;
        int frames, totalFrames;
        float avgFrames;

        bool testSwept;
        public Assignment3()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();


            random = new Random();
            gameobjects = new List<GameObject>();

            running = true;
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CollisionReset));

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(FPSReset));

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(AddFrames));

            speed = 1;
            showInfo = true;
            showColors = true;
            showTextures = false;

            testSwept = false;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            model = Content.Load<Model>("Sphere");
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();

            //(model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = Color.Blue.ToVector3();

            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            //lab7
            font = Content.Load<SpriteFont>("Font");
            //AddSphere();
            texture = Content.Load<Texture2D>("Square");
            blank = Content.Load<Texture2D>("Blank");
            frames = 0;

            

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            /*
            for (int i = 0; i < 5; i++)// Lab7
            {
                //AddSphere();
                AddGameObject();
            }*/
            AddGameObject();

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            // TODO: Add your update logic here


            if (InputManager.IsKeyPressed(Keys.Up)) { AddGameObject(); }
            if (InputManager.IsKeyPressed(Keys.Down) && gameobjects.Count > 0) { gameobjects.RemoveAt(gameobjects.Count - 1); }
            if (InputManager.IsKeyDown(Keys.Left)) { speed -= 0.0025f; }
            if (InputManager.IsKeyDown(Keys.Right)) { speed += 0.0025f; }

            //no negative speed.
            if(speed < 0)
            {
                speed = 0;
            }

            if (InputManager.IsKeyPressed(Keys.LeftShift)) { showInfo = !showInfo; }
            if (InputManager.IsKeyPressed(Keys.Space)) { showColors = !showColors; }
            if (InputManager.IsKeyPressed(Keys.LeftAlt)) { showTextures = !showTextures; }


            if(gameobjects.Count > 0)
            {
                //foreach (GameObject obj in gameobjects)
                for (int i = 0; i < gameobjects.Count; i++)
                {
                    gameobjects[i].Rigidbody.Velocity = gameobjects[i].Rigidbody.Velocity * speed;
                    if (gameobjects[i].Transform.Position.X > 15 || gameobjects[i].Transform.Position.X < -15f ||
                        gameobjects[i].Transform.Position.Y > 15 || gameobjects[i].Transform.Position.Y < -15f ||
                        gameobjects[i].Transform.Position.Z > 15 || gameobjects[i].Transform.Position.Z < -15f)
                        gameobjects.RemoveAt(i);
                    else
                        gameobjects[i].Update();
                }

                Vector3 normal; // it is updated if a collision happens
                for (int i = 0; i < gameobjects.Count; i++)
                {
                    if (boxCollider.Collides(gameobjects[i].Get<SphereCollider>(), out normal))
                    {
                        numberCollisions++;
                        if (Vector3.Dot(normal, gameobjects[i].Get<Rigidbody>().Velocity) < 0)
                            gameobjects[i].Get<Rigidbody>().Impulse +=
                               Vector3.Dot(normal, gameobjects[i].Get<Rigidbody>().Velocity) * -2 * normal;
                    }
                    for (int j = i + 1; j < gameobjects.Count; j++)
                    {
                        if (gameobjects[i].Get<SphereCollider>().Collides(gameobjects[j].Get<SphereCollider>(), out normal))
                        {
                            //testSwept = true;
                            numberCollisions++;
                            if (Vector3.Dot(normal, gameobjects[i].Get<Rigidbody>().Velocity) > 0 &&
                               Vector3.Dot(normal, gameobjects[j].Get<Rigidbody>().Velocity) < 0)
                                return;
                            Vector3 velocityNormal = Vector3.Dot(normal,
                            gameobjects[i].Get<Rigidbody>().Velocity - gameobjects[j].Get<Rigidbody>().Velocity) * -2
                               * normal * gameobjects[i].Get<Rigidbody>().Mass * gameobjects[j].Get<Rigidbody>().Mass;
                            gameobjects[i].Get<Rigidbody>().Impulse += velocityNormal / 2;
                            gameobjects[j].Get<Rigidbody>().Impulse += -velocityNormal / 2;
                        }

                    }
                }
            }

            frames++;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

                       

            foreach (GameObject obj in gameobjects)
            {
                
                /*
                if (showTextures)
                {
                    obj.Add<Renderer>(new Renderer(model, obj.Transform, camera, Content,
                GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
                }
                else
                {
                    obj.Add<Renderer>(new Renderer(model, obj.Transform, camera, Content,
                GraphicsDevice, light, 1, "SimpleShading", 20f, blank));
                }
                */

                obj.Draw();

                
                if (showColors)
                {
                    float speed = obj.Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                               new Vector3(speedValue, speedValue, 1);
                    //obj.Add<Renderer>(new Renderer(model, obj.Transform, camera, Content,
                    //GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
                    model.Draw(obj.Transform.World, camera.View, camera.Projection);

                }
            }

            _spriteBatch.Begin();

            if(showInfo)
            {
                _spriteBatch.DrawString(font, "Number of Spheres: " + gameobjects.Count, new Vector2(5, GraphicsDevice.Viewport.Height - 80), Color.Black);
                _spriteBatch.DrawString(font, "Average Collisions over 1 second: " + lastSecondCollisions, new Vector2(5, GraphicsDevice.Viewport.Height - 60), Color.Black);
                _spriteBatch.DrawString(font, "Current Animation Speed: " + speed, new Vector2(5, GraphicsDevice.Viewport.Height - 40), Color.Black);
                _spriteBatch.DrawString(font, "Average Frame Rate: " + avgFrames, new Vector2(5, GraphicsDevice.Viewport.Height - 20), Color.Black);
            }
            
                _spriteBatch.DrawString(font, "Press Left Shift to toggle diagnostic info.", new Vector2(5, 5), Color.Black);
                _spriteBatch.DrawString(font, "Press Up/Down to add/remove a sphere", new Vector2(5, 25), Color.Black);
                _spriteBatch.DrawString(font, "Press Left/Right to increase/decrease animation speed", new Vector2(5, 45), Color.Black);
                _spriteBatch.DrawString(font, "Press Space to show speed-based coloring: " + showColors, new Vector2(5, 65), Color.Black);
                _spriteBatch.DrawString(font, "Press Left Alt to show material textures: " + showTextures, new Vector2(5, 85), Color.Black);

            //_spriteBatch.DrawString(font, "Swept collide?: " + testSwept, new Vector2(5, 150), Color.Black);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();
            gameObject.Transform.LocalPosition += Vector3.One * 5 * (float)random.NextDouble();
            gameObject.Transform.GameObject = gameObject;
            gameObject.Add<Rigidbody>();
            gameObject.Rigidbody.Transform = gameObject.Transform;
            gameObject.Rigidbody.Mass = 1;
            //gameObject.Rigidbody.Mass = (float)random.NextDouble() * (1 - 0.5f) + 0.5f;


            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            gameObject.Rigidbody.Velocity =
               direction * ((float)random.NextDouble() * 5 + 5); // changed speed so its more bearable
            //rigidbody.Acceleration = Vector3.Down * 9.81f;
            gameObject.Add<SphereCollider>();
            gameObject.Get<SphereCollider>().Radius = 1f * gameObject.Transform.LocalScale.Y;
            gameObject.Get<SphereCollider>().Transform = gameObject.Transform;

            Renderer renderer = new Renderer(model, gameObject.Transform, camera, Content,
            GraphicsDevice, light, 1, "SimpleShading", 20f, texture);

            gameObject.Add<Renderer>(renderer);

            gameobjects.Add(gameObject);
        }

        private void CollisionReset(Object obj)
        {
            while (running)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void FPSReset(Object obj)
        {
            while (running)
            {
                avgFrames = totalFrames / 10.0f;
                frames = 0;
                totalFrames = 0;
                System.Threading.Thread.Sleep(10000);
            }
        }

        private void AddFrames(Object obj)
        {
            while (running)
            {
                totalFrames += frames;
                frames = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
