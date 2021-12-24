using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using CPI311.GameEngine;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;

namespace Lab8
{
    public class Lab8 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SoundEffect sound;
        SoundEffectInstance soundInstance;

        Texture2D texture;
        Model model;
        Camera camera, topDownCamera;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;

        Effect effect;
        SpriteFont font;

        bool stereo;

        public Lab8()
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
            ScreenManager.Initialize(_graphics);



            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();

            stereo = false;

            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // *** Lab 8 Item ***********************
            
            ScreenManager.Setup(false, 1920, 1080);
            //***************************************

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(0.5f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);

            sound = Content.Load<SoundEffect>("Gun");
            model = Content.Load<Model>("Sphere");
            texture = Content.Load<Texture2D>("Square");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");

            Transform transform = new Transform();
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1f;
            collider.Transform = transform;
                
            transforms.Add(transform);
            colliders.Add(collider);
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Time.Update(gameTime);
            InputManager.Update();

            if(InputManager.IsKeyPressed(Keys.Space))
            {
                stereo = !stereo;            }



            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());
            float nearest = Single.MaxValue; // Start with highest value
            float? p;
            Collider target = null; // Assume no intersection
            foreach (Collider collider in colliders)
                if ((p = collider.Intersects(ray)) != null)
                {
                    float q = (float)p;
                    if (q < nearest)
                        nearest = q;
                    target = collider;
                }
            if (target != null && nearest < camera.FarPlane)
            {
                /*
                
                */
            }


            foreach (Collider collider in colliders)
            {
                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Blue.ToVector3();

                    if (InputManager.IsMousePressed(0))
                    {
                        if (!stereo)
                        {
                            soundInstance = sound.CreateInstance();

                            soundInstance.Play();

                        }
                        else
                        {
                            soundInstance = sound.CreateInstance(); ;
                            AudioListener listener = new AudioListener();
                            listener.Position = camera.Transform.Position;
                            listener.Forward = camera.Transform.Forward;
                            AudioEmitter emitter = new AudioEmitter();
                            emitter.Position = new Vector3(InputManager.GetMousePosition().X, InputManager.GetMousePosition().Y, 0);
                            soundInstance.Apply3D(listener, emitter);
                            soundInstance.Play();
                        }


                    }
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                                                 Color.Red.ToVector3();
                }

            }
                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;

                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
                effect.Parameters["Shininess"].SetValue(20f);

                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));

                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));

                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in model.Meshes)
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                            }
                    }
                }
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Press Space to turn on/off stereo sound. Stereo sound on: " + stereo, new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(font, "Click the left sphere to make a gunshot noise", new Vector2(5, 25), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
