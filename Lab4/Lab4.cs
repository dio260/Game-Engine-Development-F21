using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;

namespace Lab4
{
    public class Lab4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model sphere, plane, torus;
        private Transform sphereTransform, planeTransform, torusTransform, cameraTransform;
        private Camera camera;
        
        public Lab4()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            sphere = Content.Load<Model>("Sphere");
            sphereTransform = new Transform();
            

            torus = Content.Load<Model>("Torus");
            torusTransform = new Transform();
            torusTransform.Parent = sphereTransform;
            torusTransform.LocalPosition = Vector3.Left * 2.5f;

            //plane = Content.Load<Model>("Plane");
            //planeTransform = new Transform();


            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            camera = new Camera();
            camera.Transform = cameraTransform;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            InputManager.Update();
            Time.Update(gameTime);
            base.Update(gameTime);

            if (InputManager.IsKeyDown(Keys.W))
            {
                if(InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraTransform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                }
                else
                {
                    cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraTransform.Rotate(Vector3.Left, Time.ElapsedGameTime);
                }
                else
                {
                    cameraTransform.LocalPosition += cameraTransform.Back * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.A))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                }
                else
                {
                    cameraTransform.LocalPosition += cameraTransform.Left * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraTransform.Rotate(Vector3.Down, Time.ElapsedGameTime);
                }
                else
                {
                    cameraTransform.LocalPosition += cameraTransform.Right * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.Up))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    sphereTransform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                }
                else
                {
                    sphereTransform.LocalPosition += sphereTransform.Forward * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.Down))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    sphereTransform.Rotate(Vector3.Left, Time.ElapsedGameTime);
                }
                else
                {
                    sphereTransform.LocalPosition += sphereTransform.Back * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.Left))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    sphereTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                }
                else
                {
                    sphereTransform.LocalPosition += sphereTransform.Left * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    sphereTransform.Rotate(Vector3.Down, Time.ElapsedGameTime);
                }
                else
                {
                    sphereTransform.LocalPosition += sphereTransform.Right * Time.ElapsedGameTime;
                }
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            sphere.Draw(sphereTransform.World, camera.View, camera.Projection);
            torus.Draw(torusTransform.World, camera.View, camera.Projection);
            //plane.Draw(planeTransform.World, camera.View, camera.Projection);
        }
    }
}
