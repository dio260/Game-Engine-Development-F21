using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Physics;

namespace FinalProject
{
    public class FinalProject : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        FirstPersonPlayer player;
        TerrainRenderer arena;
        Camera playerCam;
        Light light;
        Effect effect;
        SpriteFont font;
        Model model;
        Texture2D background, texture, crosshair;
        SoundEffect gunshot, bombhit, bgm;
        SoundEffectInstance bgmInstance;

        Random rand;
        int minTime, maxTime;
        int minAlienSpawns, maxAlienSpawns, minShipSpawns, maxShipSpawns;

        List<BulletFinalProj> bullets = new List<BulletFinalProj>();
        List<AlienFinalProj> aliens = new List<AlienFinalProj>();
        List<ShipFinalProj> ships = new List<ShipFinalProj>();
        List<BombFinalProj> bombs = new List<BombFinalProj>();


        bool paused, running, mute;
        bool cp1, cp2, cp3, cp4, cp5, cp6, cp7, cp8, cp9, cp10;
        int score;

        //Scene stuff
        class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        Dictionary<string, Scene> scenes;
        Scene currentScene;

        Button start, exit, restart;

        public FinalProject()
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
            scenes = new Dictionary<string, Scene>();

            score = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //da screen
            ScreenManager.Setup(false, 1920, 1080);

            //da bools

            paused = false;
            mute = false;
            cp1 = false;
            cp2 = false;
            cp3 = false;
            cp4 = false;
            cp5 = false;
            cp6 = false;
            cp7 = false;
            cp8 = false;
            cp9 = false;
            cp10 = false;

            

            //da content
            background = Content.Load<Texture2D>("stars");
            model = Content.Load<Model>("Sphere");
            texture = Content.Load<Texture2D>("Square");
            crosshair = Content.Load<Texture2D>("crosshair");
            font = Content.Load<SpriteFont>("Font");
            gunshot = Content.Load<SoundEffect>("sfx_gunshot");
            bombhit = Content.Load<SoundEffect>("bombhit");
            bgm = Content.Load<SoundEffect>("spacebgm");
            bgmInstance = bgm.CreateInstance();
            bgmInstance.IsLooped = true;
            bgmInstance.Volume = 0.25f;

            //da light
            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.Position = Vector3.Up * 75 + Vector3.Backward * 165 + Vector3.Right * 165;
            light.Transform = lightTransform;

            

            //da arena
            arena = new TerrainRenderer(
                Content.Load<Texture2D>("arena"),
                Vector2.One * 100, Vector2.One * 200);

            arena.NormalMap = Content.Load<Texture2D>("arenaN");
            arena.Transform = new Transform();
            arena.Transform.LocalScale *= new Vector3(1, 4, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            //da player
            playerCam = new Camera();
            playerCam.FarPlane = 1000f;
            playerCam.NearPlane = 0.001f;
            player = new FirstPersonPlayer(arena, Content, playerCam, GraphicsDevice, light, model, "SimpleShading", texture);
            player.Add<Camera>(playerCam);

            //da scenes
            start = new Button();
            start.Texture = texture;
            start.Text = "Start Game";
            start.Bounds = new Rectangle(500, 780, 250, 100);
            start.Action += StartGame;

            exit = new Button();
            exit.Texture = texture;
            exit.Text = "Quit Game";
            exit.Bounds = new Rectangle(1200, 780, 250, 100);
            exit.Action += ExitGame;

            restart = new Button();
            restart.Texture = texture;
            restart.Text = "Restart Game";
            restart.Bounds = new Rectangle(500, 780, 250, 100);
            restart.Action += RestartGame;


            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("Dead", new Scene(DeadUpdate, DeadDraw));
            currentScene = scenes["Menu"];

            //da multithreading
            rand = new Random();
            minTime = 5;
            maxTime = 8;
            minAlienSpawns = 1;
            maxAlienSpawns = 4;
            minShipSpawns = 1;
            maxShipSpawns = 2;
            running = true;
            ThreadPool.QueueUserWorkItem(
              new WaitCallback(Spawn));

            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            InputManager.Update();
            Time.Update(gameTime);

            currentScene.Update();

            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            currentScene.Draw();

            base.Draw(gameTime);
        }

        private void Spawn(Object obj)
        {
            while (running)
            {
                if(currentScene == scenes["Play"] && !paused)
                {
                    for (int i = 0; i < rand.Next(minAlienSpawns, maxAlienSpawns); i++)
                    {
                        AlienFinalProj alien = new AlienFinalProj(arena, Content, playerCam, GraphicsDevice, light, null, texture, player);
                        aliens.Add(alien);
                    }

                    if (cp3)
                    {
                        for (int i = 0; i < rand.Next(minShipSpawns, maxShipSpawns); i++)
                        {
                            ShipFinalProj ship = new ShipFinalProj(arena, Content, playerCam, GraphicsDevice, light, null, texture, player);
                            ships.Add(ship);
                        }
                    }
                    Thread.Sleep(rand.Next(minTime, maxTime) * 1000);
                }
                
                //Thread.Sleep(1000);
            }
        }
        void ResetAll()
        {
            cp1 = false;
            cp2 = false;
            cp3 = false;
            cp4 = false;
            cp5 = false;
            cp6 = false;
            cp7 = false;
            cp8 = false;
            cp9 = false;
            cp10 = false;
            score = 0;
            player.hp = 100;
            bullets.Clear();
            aliens.Clear();
            ships.Clear();
            bombs.Clear();
            minTime = 5;
            maxTime = 8;
            minAlienSpawns = 1;
            maxAlienSpawns = 4;
            minShipSpawns = 1;
            maxShipSpawns = 2;
            //running = true;
            player.Transform.Position = Vector3.Zero;
        }
        void RestartGame(GUIElement element)
        {
            ResetAll();
            if(!mute)
                bgmInstance.Play();
            currentScene = scenes["Play"];
        }

        void StartGame(GUIElement element)
        {
            //running = true;
            if(!mute)
                bgmInstance.Play();
            currentScene = scenes["Play"];
        }
        void ExitGame(GUIElement element)
        {
            Exit();
        }
        void MainMenuUpdate()
        {
            exit.Update();
            start.Update();
        }
        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);
            _spriteBatch.DrawString(font, "shoot da aliens", new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("shoot da aliens").X / 2,
                                                                         GraphicsDevice.Viewport.Height / 2 - font.MeasureString("shoot da aliens").Y / 2), Color.White);
            _spriteBatch.DrawString(font, "CPI 311 Final Project - Dion Pimentel", new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("CPI 311 Final Project - Dion Pimentel").X / 2,
                                                                         GraphicsDevice.Viewport.Height / 2 - font.MeasureString("CPI 311 Final Project - Dion Pimentel").Y / 2 + 20), Color.White);
            start.Draw(_spriteBatch, font);
            exit.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }

        void DeadUpdate()
        {
            exit.Update();
            restart.Update();
        }
        void DeadDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);
            _spriteBatch.DrawString(font, "game over you died", new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("game over you died").X / 2,
                                                                         GraphicsDevice.Viewport.Height / 2 - font.MeasureString("game over you died").Y / 2), Color.White);
            _spriteBatch.DrawString(font, "final score: " + score, new Vector2(GraphicsDevice.Viewport.Width/2 - font.MeasureString("final score: " + score).X / 2,
                                                                         GraphicsDevice.Viewport.Height / 2 -  font.MeasureString("final score: " + score).Y / 2 + 20), Color.White);
            restart.Draw(_spriteBatch, font);
            exit.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyPressed(Keys.LeftShift) || InputManager.IsKeyPressed(Keys.RightShift))
            {
                //running = false;
                currentScene = scenes["Menu"];
                paused = false;
                ResetAll();
                bgmInstance.Stop();
            }
            
            if(!paused)
            {
                Vector3 normal;

                //player update
                player.Update();
                //testbomb.Update();
                if (player.hp <= 0)
                {
                    //running = false;
                    currentScene = scenes["Dead"];
                    if(!mute)
                        bgmInstance.Stop();
                }

                if (InputManager.IsKeyPressed(Keys.LeftControl))
                {
                    if(!mute)
                    {
                        bgmInstance.Pause();
                        for (int i = 0; i < aliens.Count; i++)
                        {
                            if (aliens[i] != null)
                            {
                                aliens[i].mute = true;
                            }

                        }
                    }
                    else
                    {
                        bgmInstance.Play();
                        for (int i = 0; i < aliens.Count; i++)
                        {
                            if (aliens[i] != null)
                            {
                                aliens[i].mute = false;
                            }

                        }
                    }
                    mute = !mute;
                }
                    

                if (InputManager.IsKeyPressed(Keys.Tab))
                {
                    paused = true;
                    //running = false;
                    bgmInstance.Pause();
                }

                if (InputManager.IsKeyPressed(Keys.Space))
                {
                    if (!mute)
                    {
                        SoundEffectInstance gun = gunshot.CreateInstance();
                        gun.Volume = 0.5f;
                        gun.Play();
                    }
                        
                    BulletFinalProj bullet = new BulletFinalProj(Content, player.Camera, GraphicsDevice, light, player);
                    bullets.Add(bullet);

                }

                //score and checkpoints
                if (score >= 1000 && !cp1)
                {
                    minAlienSpawns = 2;
                    maxAlienSpawns = 5;
                    cp1 = true;
                }

                if (score >= 1500 && !cp2)
                {
                    minAlienSpawns = 3;
                    maxAlienSpawns = 6;
                    cp2 = true;
                }

                if (score >= 2000 && !cp3) // ships unlocked
                {
                    cp3 = true;
                }


                if (score >= 2500 && !cp4)
                {
                    minAlienSpawns = 5;
                    maxAlienSpawns = 7;
                    cp4 = true;
                }

                if (score >= 3000 && !cp5)
                {
                    minShipSpawns = 2;
                    maxShipSpawns = 4;
                    cp5 = true;
                }

                if (score >= 3500 && !cp6)
                {
                    minShipSpawns = 3;
                    maxShipSpawns = 6;
                    cp6 = true;
                }

                if (score >= 4000 && !cp7)
                {
                    minAlienSpawns = 8;
                    maxAlienSpawns = 11;
                    cp7 = true;
                }

                if (score >= 4500 && !cp8)
                {
                    minAlienSpawns = 10;
                    maxAlienSpawns = 16;
                    minShipSpawns = 5;
                    maxShipSpawns = 8;
                    cp8 = true;
                }

                if (score >= 5000 && !cp9)
                {
                    minAlienSpawns = 12;
                    maxShipSpawns = 11;
                    cp9 = true;
                }


                if (score >= 5500 && !cp10)
                {
                    minAlienSpawns = 15;
                    maxShipSpawns = 10;
                    cp10 = true;
                }

                //testshot = false;

                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i] != null)
                    {
                        bullets[i].Update();

                        if (bullets[i].alivetime < 10f)
                        {
                            for (int j = 0; j < aliens.Count; j++)
                            {
                                if (aliens[j] != null && bullets[i] != null && bullets[i].Collider.Collides(aliens[j].Collider, out normal))
                                {

                                    aliens[j].hp -= 10;
                                    bullets[i] = null;
                                }
                            }

                            for (int j = 0; j < ships.Count; j++)
                            {
                                //if (ships[j] != null && bullets[i] != null && bullets[i].Collider.Collides(ships[j].Collider, out normal))
                                if (ships[j] != null && bullets[i] != null && Vector3.Distance(ships[j].Transform.Position, bullets[i].Transform.Position) <= 5f)
                                {

                                    ships[j].hp -= 10;
                                    bullets[i] = null;
                                }
                            }

                            for (int j = 0; j < bombs.Count; j++)
                            {
                                if (bombs[j] != null && bullets[i] != null && bullets[i].Collider.Collides(bombs[j].Collider, out normal))
                                {
                                    bullets[i] = null;
                                    bombs[j] = null;
                                }
                            }
                        }
                        else
                        {
                            bullets[i] = null;
                        }

                    }
                    else
                    {
                        bullets.RemoveAt(i);
                    }

                }

                for (int i = 0; i < aliens.Count; i++)
                {
                    if (aliens[i] != null)
                    {
                        aliens[i].Update();

                        if (aliens[i].hp <= 0)
                        {
                            score += 100;
                            aliens[i] = null;
                        }
                    }
                    else
                    {
                        aliens.RemoveAt(i);
                    }
                }

                for (int i = 0; i < ships.Count; i++)
                {
                    if (ships[i] != null)
                    {
                        ships[i].Update();

                        if (ships[i].hp <= 0)
                        {
                            score += 300;
                            ships[i] = null;
                        }

                        if (ships[i] != null && ships[i].shootTimer >= 3)
                        {
                            ships[i].shootTimer = 0;
                            BombFinalProj bomb = new BombFinalProj(arena, Content, playerCam, GraphicsDevice, light, "SimpleShading", texture, player);
                            bomb.Transform.LocalPosition = ships[i].Transform.LocalPosition + ships[i].Transform.Forward;
                            bombs.Add(bomb);

                        }
                    }
                    else
                    {
                        ships.RemoveAt(i);
                    }
                }

                for (int i = 0; i < bombs.Count; i++)
                {
                    if (bombs[i] != null)
                    {
                        bombs[i].Update();

                        if (bombs[i] != null && Vector3.Distance(bombs[i].Transform.Position, player.Transform.Position) <= 1f)
                        {
                            player.hp -= 20;
                            if(!mute)
                            {
                                SoundEffectInstance hitsound = bombhit.CreateInstance();
                                hitsound.Volume = 0.25f;
                                hitsound.Play();
                            }
                            
                            bombs[i] = null;
                        }
                    }
                    else
                    {
                        bombs.RemoveAt(i);
                    }
                }
            }
            else
            {
                if (InputManager.IsKeyPressed(Keys.Tab))
                {
                    paused = false;
                    //running = true;
                    if(!mute)
                        bgmInstance.Play();
                }
            }
            
        }
        void PlayDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);

            _spriteBatch.End();
            if(!paused)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                effect.Parameters["World"].SetValue(arena.Transform.World);
                effect.Parameters["View"].SetValue(playerCam.View);
                effect.Parameters["Projection"].SetValue(playerCam.Projection);
                effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
                effect.Parameters["CameraPosition"].SetValue(playerCam.Transform.Position);

                effect.Parameters["NormalMap"].SetValue(arena.NormalMap);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    arena.Draw();
                }
                player.Draw();

                if (bullets.Count != 0)
                {
                    foreach (BulletFinalProj bullet in bullets)
                    {
                        if (bullet != null)
                        {
                            bullet.Draw();
                        }
                    }
                }

                for (int i = 0; i < aliens.Count; i++)
                {
                    if (aliens[i] != null)
                    {
                        aliens[i].Draw();
                    }

                }

                for (int i = 0; i < ships.Count; i++)
                {
                    if (ships[i] != null)
                    {
                        ships[i].Draw();
                    }

                }
                for (int i = 0; i < bombs.Count; i++)
                {
                    if (bombs[i] != null)
                    {
                        bombs[i].Draw();
                    }

                }

                _spriteBatch.Begin();
                _spriteBatch.Draw(crosshair, new Vector2(GraphicsDevice.Viewport.Width / 2 - texture.Width / 2, GraphicsDevice.Viewport.Height / 2 - texture.Height / 2), Color.White);
                _spriteBatch.DrawString(font, "Score: " + score, new Vector2(5, 5), Color.White);
                _spriteBatch.DrawString(font, "Player HP: " + player.hp, new Vector2(5, 25), Color.White);
                _spriteBatch.DrawString(font, "Press tab to pause the game.", new Vector2(5, 45), Color.White);
                _spriteBatch.DrawString(font, "Press shift to go back to the menu.", new Vector2(5, 65), Color.White);
                _spriteBatch.DrawString(font, "Press Left Ctrl to mute audio. Audio muted: " + mute, new Vector2(5, 85), Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "Pause menu",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Pause menu").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Pause menu").Y / 2 - 60),
                    Color.White);
                _spriteBatch.DrawString(font, "Use WASD to move, Left/Right Arrow to rotate view, Space to shoot",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Use WASD to move, Left/Right Arrow to rotate view, Space to shoot").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Use WASD to move, Left/Right Arrow to rotate view, Space to shoot").Y / 2 - 40), 
                    Color.White);
                _spriteBatch.DrawString(font, "Aliens chase you and deal 10 damage. Takes 3 shots to kill and kill score is 100 pts",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Aliens chase you and deal 10 damage. Takes 3 shots to kill and kill score is 100 pts").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Aliens chase you and deal 10 damage. Takes 3 shots to kill and kill score is 100 pts").Y / 2 - 20),
                    Color.White);
                _spriteBatch.DrawString(font, "Ships stay in one place and shoot out white bombs. Take 10 shots to kill and kill score is 300 pts",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Ships stay in one place and shoot out white bombs. Take 10 shots to kill and kill score is 300 pts").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Ships stay in one place and shoot out white bombs. Take 10 shots to kill and kill score is 300 pts").Y / 2),
                    Color.White);
                _spriteBatch.DrawString(font, "Ship bombs also chase you and deal 20 damage. Takes 1 shot to kill but no pts",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Ship bombs also chase you and deal 20 damage. Takes 1 shot to kill but no pts").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Ship bombs also chase you and deal 20 damage. Takes 1 shot to kill but no pts").Y / 2 + 20),
                    Color.White);
                _spriteBatch.DrawString(font, "More enemies spawn every 500 points",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("More enemies spawn every 500 points").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("More enemies spawn every 500 points").Y / 2 + 40),
                    Color.White);
                _spriteBatch.DrawString(font, "Press tab to unpause the game.",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Press tab to unpause the game.").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Press tab to unpause the game.").Y / 2 + 60), 
                    Color.White);
                _spriteBatch.DrawString(font, "Press shift to go back to the menu.",
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - font.MeasureString("Press shift to go back to the menu.").X / 2,
                    GraphicsDevice.Viewport.Height / 2 - font.MeasureString("Press shift to go back to the menu.").Y / 2 + 80), 
                    Color.White);
                _spriteBatch.End();
            }
            
        }
    }
}
