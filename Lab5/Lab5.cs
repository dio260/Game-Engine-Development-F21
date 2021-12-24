using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using CPI311.GameEngine.Physics;
using CPI311.GameEngine.Managers;

namespace Lab5
{
    public class Lab5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;
        Model model;
        Effect effect;
        Camera cam;
        Transform parentTransform;
        Transform cameraTransform;
        Transform childTransform;
        Texture2D texture;

        int tech = 0;
        string shadertext;

        public Lab5()
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            effect = Content.Load<Effect>("SimpleShading2");
            model = Content.Load<Model>("Torus");
            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");

            cam = new Camera();

            parentTransform = new Transform();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 3;
            cam.Transform = cameraTransform;


            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                    effect.EnableDefaultLighting();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (InputManager.IsKeyDown(Keys.W))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
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

            if(InputManager.IsKeyPressed(Keys.Tab))
            {
                tech = (tech + 1) % 4;
            }

            switch(tech)
            {
                case (0):
                    shadertext = "Current technique is Gourard";
                    break;
                case (1):
                    shadertext = "Current technique is Phong";
                    break;
                case (2):
                    shadertext = "Current technique is Phong-Blinn";
                    break;
                case (3):
                    shadertext = "Current technique is Schlick";
                    break;
            }

            Time.Update(gameTime);
            InputManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Matrix view = cam.View;
            Matrix projection = cam.Projection;

            effect.CurrentTechnique = effect.Techniques[tech];
            effect.Parameters["World"].SetValue(parentTransform.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(cameraTransform.Position);
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0, 0));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0,0,0.5f));
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach(ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, shadertext,
                        new Vector2(10, 20), Color.Black);
            _spriteBatch.DrawString(font, "Use WASD to move the camera",
                           new Vector2(280, 410), Color.Black);
            _spriteBatch.DrawString(font, "Hold Shift while using WASD to rotate the camera view",
                       new Vector2(210, 435), Color.Black);
            _spriteBatch.DrawString(font, "Switch shader methods with tab",
                       new Vector2(285, 460), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
