using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Physics;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random;
        Camera camera;
        Light light;

        //Audio components
        SoundEffect gunSound, asteroidExplosion, shipExplosion;
        //SoundEffectInstance soundInstance;

        //Visual components
        ShipAssn4 ship;
        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        BulletAssn4[] bulletList = new BulletAssn4[GameConstants.NumBullets];

        //Score & background
        int score, bulletCount, asteroidCount;
        Texture2D stars;
        SpriteFont font;

        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;
        bool sound;


        public Assignment4()
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

            ScreenManager.Setup(false, 1920, 1080);

            score = 0;

            font = Content.Load<SpriteFont>("Font");

            random = new Random();

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5;

            camera = new Camera();
            camera.Transform = new Transform();
            //camera.Transform.LocalPosition = Vector3.Right * 5 + Vector3.Backward * 5 + Vector3.Up * 10;
            camera.Transform.LocalPosition = Vector3.Up * (GameConstants.CameraHeight);
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            camera.NearPlane = 0.1f;//GameConstants.CameraHeight - 1000f;
            camera.FarPlane = GameConstants.CameraHeight + 1000f;

            ship = new ShipAssn4(Content, camera, GraphicsDevice, light);

            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i] = new BulletAssn4(Content, camera, GraphicsDevice, light);
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);
            ResetAsteroids(); // look at the below private method

            bulletCount = bulletList.Length;
            asteroidCount = asteroidList.Length;

            //ship.Transform.Position = Vector3.Down * 100 + Vector3.Backward * 100;
            //ship.Transform.Rotation = Quaternion.Identity;

            stars = Content.Load<Texture2D>("stars");
            gunSound = Content.Load<SoundEffect>("tx0_fire1");
            asteroidExplosion = Content.Load<SoundEffect>("explosion2");
            shipExplosion = Content.Load<SoundEffect>("explosion3");

            // *** Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particleEffect = Content.Load<Effect>("ParticleShader");
            particleTex = Content.Load<Texture2D>("fire");

            sound = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            InputManager.Update();
            Time.Update(gameTime);
            ship.Update();
            //ship.Transform.Position += Vector3.Down;
            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                asteroidList[i].Update();

            if (score < 0)
                score = 0;

            if (InputManager.IsKeyPressed(Keys.Space) && (bulletCount == 0 || asteroidCount == 0)) { ResetGame(); }

            if (InputManager.IsKeyPressed(Keys.Tab)) sound = !sound;

            if (InputManager.IsMousePressed(0))
            {


                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].isActive && bulletList[i].usable)
                    {
                        if (sound)
                        {
                            SoundEffectInstance soundInstance = gunSound.CreateInstance();
                            soundInstance.IsLooped = false;
                            soundInstance.Pitch = 0.2f;
                            soundInstance.Volume = 0.7f;
                            soundInstance.Play();
                        }


                        bulletList[i].Rigidbody.Velocity =
                   (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position +
                                (200 * bulletList[i].Transform.Forward);
                        bulletList[i].isActive = true;
                        //score -= GameConstants.ShotPenalty;
                        // sound
                        bulletCount--;
                        break; //exit the loop     
                    }
                }
            }

            Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
                if (asteroidList[i].isActive)
                {
                    for (int j = 0; j < bulletList.Length; j++)
                        if (bulletList[j].isActive &&
                            !(bulletList[j].Transform.Position.X > GameConstants.PlayfieldSizeX ||
                            bulletList[j].Transform.Position.X < -GameConstants.PlayfieldSizeX ||
                            bulletList[j].Transform.Position.Z > GameConstants.PlayfieldSizeY ||
                            bulletList[j].Transform.Position.Z < -GameConstants.PlayfieldSizeY)
                            )
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                // Particles
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                  random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(1, 6);
                                particle.Init();
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                bulletList[j].usable = false;
                                asteroidCount--;
                                score += GameConstants.KillBonus;
                                if (sound)
                                {
                                    SoundEffectInstance soundInstance = asteroidExplosion.CreateInstance();
                                    soundInstance.IsLooped = false;
                                    soundInstance.Pitch = 0.2f;
                                    soundInstance.Volume = 0.1f;
                                    soundInstance.Play();
                                }

                                break; //no need to check other bullets
                            }

                    if (asteroidList[i].Collider.Collides(ship.Collider, out normal) && bulletCount != 0)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = ship.Transform.Position;
                        particle.Velocity = new Vector3(
                          random.Next(-5, 5), 2, random.Next(-50, 50));
                        particle.Acceleration = new Vector3(0, 3, 0);
                        particle.MaxAge = random.Next(1, 6);
                        particle.Init();
                        ship.Transform.Position = Vector3.Zero;
                        score -= GameConstants.DeathPenalty;
                        if (sound)
                        {
                            SoundEffectInstance soundInstance = shipExplosion.CreateInstance();
                            soundInstance.IsLooped = false;
                            soundInstance.Pitch = 0.2f;
                            soundInstance.Volume = 0.1f;
                            soundInstance.Play();
                        }

                    }
                }

            // particles update
            particleManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0, 0, 1920, 1080), Color.White);
            _spriteBatch.DrawString(font, "Current Score: " + score,
                       new Vector2(20, 20), Color.Green);
            _spriteBatch.DrawString(font, "Bullets Remaining: " + bulletCount,
                       new Vector2(20, 40), Color.Green);
            _spriteBatch.DrawString(font, "Asteroids Remaining: " + asteroidCount,
                       new Vector2(20, 60), Color.Green);

            if (bulletCount == 0 || asteroidCount == 0)
            {
                _spriteBatch.DrawString(font, "Game Over. Your score was " + score,
                       new Vector2(1920 / 2, 960), Color.Green);
                _spriteBatch.DrawString(font, "Press Space to reset the game",
                       new Vector2(1920 / 2, 980), Color.Green);
            }


            _spriteBatch.DrawString(font, "Use WASD to move",
                       new Vector2(20, 1020), Color.Green);
            _spriteBatch.DrawString(font, "Click Left Mouse to Shoot",
                       new Vector2(20, 1040), Color.Green);
            _spriteBatch.DrawString(font, "Press Tab to toggle sound. Sound: " + sound,
                       new Vector2(20, 1060), Color.Green);


            _spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            // ship, bullets, and asteroids
            ship.Draw();
            for (int i = 0; i < GameConstants.NumBullets; i++)
                if (bulletList[i].isActive)
                    bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                if (asteroidList[i].isActive)
                    asteroidList[i].Draw();

            //particle draw
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            particleEffect.CurrentTechnique = particleEffect.Techniques["particle"];
            particleEffect.CurrentTechnique.Passes[0].Apply();
            particleEffect.Parameters["ViewProj"].SetValue(camera.View * camera.Projection);
            particleEffect.Parameters["World"].SetValue(Matrix.Identity);
            particleEffect.Parameters["CamIRot"].SetValue(
            Matrix.Invert(Matrix.CreateFromQuaternion(camera.Transform.Rotation)));
            particleEffect.Parameters["Texture"].SetValue(particleTex);
            particleManager.Draw(GraphicsDevice);

            //...

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }

        private void ResetAsteroids()
        {
            float xStart = 0;
            float yStart = 0;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {

                while (xStart <= GameConstants.playerSpawnX && xStart >= -GameConstants.playerSpawnX)
                {
                    double rnd = random.NextDouble();
                    if (random.Next(2) == 0)
                        xStart = (float)rnd * (-GameConstants.PlayfieldSizeX);
                    else
                        xStart = (float)rnd * GameConstants.PlayfieldSizeX;
                }
                while (yStart <= GameConstants.playerSpawnY && yStart >= -GameConstants.playerSpawnY)
                {
                    double rnd = random.NextDouble();
                    if (random.Next(2) == 0)
                        yStart = (float)rnd * (-GameConstants.PlayfieldSizeY);
                    else
                        yStart = (float)rnd * GameConstants.PlayfieldSizeY;
                }
                //yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);
                asteroidList[i].Transform.Position = new Vector3(xStart, 0.0f, yStart);
                //asteroidList[i].Transform.Position = new Vector3(0, 0.0f, 0);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                   -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) *
            (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() *
            GameConstants.AsteroidMaxSpeed);
                asteroidList[i].isActive = true;
            }
        }

        private void ResetGame()
        {
            ship.Transform.Position = Vector3.Zero;
            ResetAsteroids();
            foreach (BulletAssn4 bullet in bulletList)
            {
                bullet.usable = true;
            }
            bulletCount = bulletList.Length;
            asteroidCount = asteroidList.Length;
            score = 0;
        }

    }
}
