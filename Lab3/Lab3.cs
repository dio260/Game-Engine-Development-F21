using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine.Managers;

namespace Lab3
{
    public class Lab3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont font;

        private Model model;
        private Matrix view, world, projection;
        private Vector3 cameraPosition, modelScale, modelPosition;
        private float yaw, pitch, roll;
        private Vector2 cameraSize, cameraCenter, topLeft, bottomRight;

        private bool worldOrder, cameraMode;
        private string worldString, cameraString;

        public Lab3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            cameraPosition = new Vector3(0, 0, 3);
            modelScale = Vector3.One;
            modelPosition = Vector3.Zero;
            yaw = 0;
            pitch = 0;
            roll = 0;

            cameraSize = new Vector2(0.15f,0.1f);
            cameraCenter = new Vector2(0,0);

            topLeft = cameraCenter - cameraSize;
            bottomRight = cameraCenter + cameraSize;

            worldOrder = true;
            cameraMode = true;
            worldString = "Scale * Rotation * Translation";
            cameraString = "Perspective";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InputManager.Initialize();
            Time.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            model = Content.Load<Model>("Torus");
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            // TODO: Add your update logic here
            if(InputManager.IsKeyDown(Keys.Up))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    modelScale += Vector3.One * Time.ElapsedGameTime;
                }
                else
                {
                    modelPosition += Vector3.Up * Time.ElapsedGameTime;
                }
                    
            }

            if (InputManager.IsKeyDown(Keys.Down))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    modelScale -= Vector3.One * Time.ElapsedGameTime;
                }
                else
                {
                    modelPosition += Vector3.Down * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.Left))
            {
                modelPosition += Vector3.Left * Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.Right))
            {
                modelPosition += Vector3.Right * Time.ElapsedGameTime;
            }


            if (InputManager.IsKeyDown(Keys.Insert))
            {
                yaw += Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.Delete))
            {
                yaw -= Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.Home))
            {
                pitch += Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.End))
            {
                pitch -= Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.PageUp))
            {
                roll += Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.PageDown))
            {
                roll -= Time.ElapsedGameTime;
            }

            if (InputManager.IsKeyDown(Keys.W))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraCenter.Y += Time.ElapsedGameTime;
                }
                else if((InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl)))
                {
                    cameraSize.Y += Time.ElapsedGameTime;
                }
                else
                {
                    cameraPosition += Vector3.Up * Time.ElapsedGameTime;
                }
                
            }

            if (InputManager.IsKeyDown(Keys.A))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraCenter.X -= Time.ElapsedGameTime;
                }
                else if ((InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl)))
                {
                    cameraSize.X -= Time.ElapsedGameTime;
                }
                else
                {
                    cameraPosition += Vector3.Left * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraCenter.Y -= Time.ElapsedGameTime;
                }
                else if ((InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl)))
                {
                    cameraSize.Y -= Time.ElapsedGameTime;
                }
                else
                {
                    cameraPosition += Vector3.Down * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
                {
                    cameraCenter.X += Time.ElapsedGameTime;
                }
                else if ((InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl)))
                {
                    cameraSize.X += Time.ElapsedGameTime;
                }
                else
                {
                    cameraPosition += Vector3.Right * Time.ElapsedGameTime;
                }
            }

            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                cameraMode = !cameraMode;
            }

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                worldOrder = !worldOrder;
            }


            topLeft = cameraCenter - cameraSize;
            bottomRight = cameraCenter + cameraSize;


            if (worldOrder)
            {
                world = Matrix.CreateScale(modelScale) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(modelPosition);
                worldString = "Scale * Rotation * Translation";
            }
            else
            {
                world = Matrix.CreateTranslation(modelPosition) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateScale(modelScale);
                worldString = "Translation * Rotation * Scale";
            }

            if(cameraMode)
            {
                /*
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2,// field of view
                                                             GraphicsDevice.Viewport.AspectRatio, // screen aspect
                                                             0.1f, 1000f) // near and far planes
                */
                           projection = Matrix.CreatePerspectiveOffCenter(topLeft.X, bottomRight.X, 
                               topLeft.Y, bottomRight.Y, 0.1f, 1000f)
                           ;            
                cameraString = "Perspective";
            }
            else
            {
                //projection = Matrix.CreateOrthographic( , , 0.1f, 1000f);
                //projection = Matrix.CreateOrthographicOffCenter( , , , , 0.1f, 1000f);
                projection = Matrix.CreateOrthographicOffCenter(topLeft.X*10, bottomRight.X*10,
                               topLeft.Y*10, bottomRight.Y*10, 0.1f, 1000f)
                           ;
                cameraString = "Orthographic";
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);            
            
            model.Draw(world, view, projection);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, "Object Controls",
                        new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(font, "Arrow Keys: Move Object",
                        new Vector2(5, 20), Color.Black);
            _spriteBatch.DrawString(font, "Rotation Controls:",
                        new Vector2(5, 35), Color.Black);
            _spriteBatch.DrawString(font, "Insert/Delete: Increase/Decrease Yaw",
                        new Vector2(5, 50), Color.Black);
            _spriteBatch.DrawString(font, "Home/End: Increase/Decrease Pitch",
                        new Vector2(5, 65), Color.Black);
            _spriteBatch.DrawString(font, "PgUp/PgDn: Increase/Decrease Roll",
                        new Vector2(5, 80), Color.Black);
            _spriteBatch.DrawString(font, "Space: Change World Matrix Order",
                        new Vector2(5, 95), Color.Black);
            _spriteBatch.DrawString(font, "Current Matrix Order: " + worldString,
                        new Vector2(5, 110), Color.Black);

            _spriteBatch.DrawString(font, "WASD Keys: Move Camera",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 110), Color.Black);
            _spriteBatch.DrawString(font, "Tab: Change Camera Mode",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 95), Color.Black);
            _spriteBatch.DrawString(font, "Current Camera Mode: " + cameraString,
                        new Vector2(5, GraphicsDevice.Viewport.Height - 80), Color.Black);
            _spriteBatch.DrawString(font, "Shift + WS: Move Camera Center Y",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 65), Color.Black);
            _spriteBatch.DrawString(font, "Shift + AD: Move Camera Center X",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 50), Color.Black);
            _spriteBatch.DrawString(font, "Ctrl + WS: Increase/Decrease Camera Height",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 35), Color.Black);
            _spriteBatch.DrawString(font, "Ctrl + AD: Increase/Decrease Camera Width",
                        new Vector2(5, GraphicsDevice.Viewport.Height - 20), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
