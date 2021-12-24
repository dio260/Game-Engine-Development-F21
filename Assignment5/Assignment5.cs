using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Physics;

namespace Assignment5
{
    public class Assignment5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrain;
        Camera topDownCamera;
        Light light;
        Effect effect;
        SpriteFont font;
        Texture2D background, texture;
        Player player;
        Agent alien, alien2, alien3;
        Bomb bomb;
        Model model;
        BoxCollider center;
        AlienFinalProj testalien;
        FirstPersonPlayer testplayer;

        bool gameOver, checkpoint1, checkpoint2;
        int score;

        public Assignment5()
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
            gameOver = false;
            score = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ScreenManager.Setup(false, 1920, 1080);

            background = Content.Load<Texture2D>("stars");
            model = Content.Load<Model>("Sphere");
            texture = Content.Load<Texture2D>("Square");

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 50;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.5f, 0f);
            topDownCamera.Size = new Vector2(1f, 1f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;
            font = Content.Load<SpriteFont>("Font");

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.Position = Vector3.Up * 75 + Vector3.Backward * 165 + Vector3.Right * 165;
            light.Transform = lightTransform;

            terrain = new TerrainRenderer(
                Content.Load<Texture2D>("arena"),
                Vector2.One * 100, Vector2.One * 200);

            terrain.NormalMap = Content.Load<Texture2D>("arenaN");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 4, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);


            alien = new Agent(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture);
            alien.Renderer.Material.Ambient = Color.Red.ToVector3();
            alien.Renderer.Material.Diffuse = Color.Red.ToVector3();
            alien.Renderer.Material.Specular = Color.Red.ToVector3();
            alien2 = new Agent(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture);
            alien2.Renderer.Material.Ambient = Color.Red.ToVector3();
            alien2.Renderer.Material.Diffuse = Color.Red.ToVector3();
            alien2.Renderer.Material.Specular = Color.Red.ToVector3();
            alien3 = new Agent(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture);
            alien3.Renderer.Material.Ambient = Color.Red.ToVector3();
            alien3.Renderer.Material.Diffuse = Color.Red.ToVector3();
            alien3.Renderer.Material.Specular = Color.Red.ToVector3();
            //player = new Player(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture);
            //bomb = new Bomb(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture, player);
            //bomb.Renderer.Material.Ambient = Color.Black.ToVector3();
            //bomb.Renderer.Material.Diffuse = Color.Black.ToVector3();
            //bomb.Renderer.Material.Specular = Color.Black.ToVector3();

            testplayer = new FirstPersonPlayer(terrain, Content, topDownCamera, GraphicsDevice, light, model, "SimpleShading", texture);
            testalien = new AlienFinalProj(terrain, Content, topDownCamera, GraphicsDevice, light, null, texture, testplayer);

            center = new BoxCollider();
            center.Size = 3;

            checkpoint1 = false;
            checkpoint2 = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            if(!gameOver)
            {
                Vector3 normal;
                testplayer.Update();
                
                testalien.Update();
                //bomb.Update();
                /*
                if (checkpoint1)
                {
                    alien2.Update();
                    if (player.Collider.Collides(alien2.Collider, out normal))
                    {
                        alien2.RandomPathFinding();
                        score += 100;
                    }
                }
                    
                if (checkpoint2)
                {
                    alien3.Update();
                    if (player.Collider.Collides(alien3.Collider, out normal))
                    {
                        alien3.RandomPathFinding();
                        score += 100;
                    }
                }

                if (player.Collider.Collides(alien.Collider, out normal))
                {
                    alien.RandomPathFinding();
                    score += 100;
                }

                if (center.Collides(alien.Collider, out normal))
                {
                    alien.RandomPathFinding();
                    score -= 200;
                }

                if (score >= 500 && checkpoint1 == false)
                {
                    checkpoint1 = true;
                    bomb.speed += 5;
                }

                if (score >= 1000 && checkpoint2 == false)
                {
                    checkpoint2 = true;
                    bomb.speed += 5;
                }

                if (player.Collider.Collides(bomb.Collider, out normal))
                {
                    gameOver = true;
                }
                */
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);
            _spriteBatch.End();
            if (gameOver)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "Game over. Score: " + score, new Vector2(GraphicsDevice.Viewport.Width/2 - 150, GraphicsDevice.Viewport.Height / 2), Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "Use WASD to move ", new Vector2(5, 5), Color.White);
                _spriteBatch.DrawString(font, "Catch aliens by running into them", new Vector2(5, 25), Color.White);
                _spriteBatch.DrawString(font, "Catching aliens will give you 100 points.", new Vector2(5, 45), Color.White);
                _spriteBatch.DrawString(font, "Letting aliens reach the center area will lose you 200 points ", new Vector2(5, 65), Color.White);
                _spriteBatch.DrawString(font, "Touching the bomb will result in game over", new Vector2(5, 85), Color.White);
                _spriteBatch.DrawString(font, "Score: " + score, new Vector2(5, 105), Color.White);
                _spriteBatch.End();


                GraphicsDevice.DepthStencilState = new DepthStencilState();


                effect.Parameters["World"].SetValue(terrain.Transform.World);
                effect.Parameters["View"].SetValue(topDownCamera.View);
                effect.Parameters["Projection"].SetValue(topDownCamera.Projection);
                effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
                effect.Parameters["CameraPosition"].SetValue(topDownCamera.Transform.Position);

                effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    terrain.Draw();
                }

                testplayer.Draw();
                testalien.Draw();
                //bomb.Draw();
                if (checkpoint1)
                    alien2.Draw();
                if (checkpoint2)
                    alien3.Draw();
            }
            


            base.Draw(gameTime);
        }
    }
}
