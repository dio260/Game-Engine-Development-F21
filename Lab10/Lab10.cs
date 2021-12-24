using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Physics;

namespace Lab10
{
    public class Lab10 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Effect effect;
        SpriteFont font;
        public Lab10()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(_graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("Font");
            terrain = new TerrainRenderer(
                Content.Load<Texture2D>("Heightmap"),
                Vector2.One * 100, Vector2.One * 200);

            terrain.NormalMap = Content.Load<Texture2D>("Normalmap");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyDown(Keys.W))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
                }
                else
                {
                    camera.Transform.LocalPosition += camera.Transform.Forward * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    camera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);
                }
                else
                {
                    camera.Transform.LocalPosition += camera.Transform.Back * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.A))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    camera.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
                }
                else
                {
                    camera.Transform.LocalPosition += camera.Transform.Left * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    camera.Transform.Rotate(Vector3.Down, Time.ElapsedGameTime);
                }
                else
                {
                    camera.Transform.LocalPosition += camera.Transform.Right * Time.ElapsedGameTime;
                }
            }

            camera.Transform.LocalPosition = new Vector3(
            camera.Transform.LocalPosition.X,
            terrain.GetAltitude(camera.Transform.LocalPosition),
            camera.Transform.LocalPosition.Z) + Vector3.Up;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(camera.Transform.Position + Vector3.Up * 10);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Use WASD to move ", new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(font, "Hold Shift and press WASD to rotate the camera", new Vector2(5, 25), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
