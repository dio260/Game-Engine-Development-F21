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

namespace Lab6
{
    //This combines Lab 6 and 7
    public class Lab6 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;
        Model model;
        Random random;

        Transform cameraTransform;
        Camera camera;

        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        List<Transform> transforms;

        //lab7
        int numberCollisions = 0;
        int lastSecondCollisions;
        bool running;
        Texture2D texture;
        List<Renderer> renderers;
        Light light;

        BoxCollider boxCollider;
        public Lab6()
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
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            //add 2 spheres - lab 6

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += Vector3.Right * 3 * i; //avoid overlapping each sphere 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;

                Vector3 direction = new Vector3(
                  (float)random.NextDouble(), (float)random.NextDouble(),
                  (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity =
                   direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 2.5f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;

                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);

                
            }
            

            //Lab7
            renderers = new List<Renderer>();

            running = true;
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CollisionReset));
            

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
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            //lab7
            font = Content.Load<SpriteFont>("Font");
            //AddSphere();
            texture = Content.Load <Texture2D>("Square");

            

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTransform;

            /*
            for (int i = 0; i < 5; i++)// Lab7
            {
                AddSphere();
            }
            */

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            // TODO: Add your update logic here


            if (InputManager.IsKeyPressed(Keys.Space)) //Lab7 - add more spheres if you want
            {
                //AddSphere();
            }

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();

            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    // Lab 6
                    if (colliders[i].Collides(colliders[j], out normal))
                        numberCollisions++;

                    Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                           * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                    

                    // Lab 7
                    /*
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        numberCollisions++;
                        if (Vector3.Dot(normal, rigidbodies[i].Velocity) > 0 &&
                           Vector3.Dot(normal, rigidbodies[j].Velocity) < 0)
                            return;
                        Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                           * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                        rigidbodies[i].Impulse += velocityNormal / 2;
                        rigidbodies[j].Impulse += -velocityNormal / 2;
                    }
                    */
                       
                }
            }


            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            /*
            foreach (Transform transform in transforms)
                model.Draw(transform.World, camera.View, camera.Projection);*/

            //Lab7
            for (int i = 0; i < transforms.Count; i++)
            {
                
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                           new Vector3(speedValue, speedValue, speedValue);
                model.Draw(transform.World, camera.View, camera.Projection);
                
                //lab7
                //renderers[i].Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Collisions in last thread: " + lastSecondCollisions, new Vector2(5,5), Color.Black);
            _spriteBatch.DrawString(font, "Press Space to add a sphere", new Vector2(280, 410), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddSphere()
        {

            Transform transform = new Transform();
            transform.LocalPosition += Vector3.One * 5 * (float)random.NextDouble();
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;
            //rigidbody.Mass = (float) random.NextDouble();

            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity =
               direction * ((float)random.NextDouble() * 5 + 5);
            rigidbody.Acceleration = Vector3.Down * 9.81f;
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            //Texture2D texture2 = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content,
                GraphicsDevice, light, 1, "SimpleShading2", 20f, texture);

            renderers.Add(renderer);
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
        }

        private void CollisionReset(Object obj)
        {
            while(running)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
