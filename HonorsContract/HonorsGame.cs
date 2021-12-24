using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;
using CPI311.GameEngine.Labs_Assignments;

namespace HonorsContract
{
    public class HonorsGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        AnimatedSprite slim;
        enemy2D boss;
        //Sprite bullet;
        Texture2D enemyLeft, enemyRight;
        Sprite test;
        SoundEffect gunshot, enemyDeath, normal, bossMusic, victory;
        SoundEffectInstance enemyPop, normalBgm, bossBgm, victoryBgm;
        
        ProgressBar killCount;
        Axis horizontalAxis, verticalAxis;
        int maxSpeed;
        bool gameOver, gameCompleted, checkpoint1, checkpoint2, checkpoint3, bosscheckpoint1, bosscheckpoint2, bosscheckpoint3, musicChange;
        SpriteFont font;

        Random rand = new Random();
        Bullet2D[] bullets = new Bullet2D[6];
        Bullet2D[] bossBullets = new Bullet2D[1];
        enemy2D[] enemies = new enemy2D[5];

        enemy2D[] verticalEnemies = new enemy2D[4];
        enemy2D[] horizontalEnemies = new enemy2D[3];

        float shootAnimTimer;

        float direction;
        float bulletSpeed;
        int bulletCount;
        public HonorsGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            Time.Initialize();

            horizontalAxis = new Axis();
            verticalAxis = new Axis();
            horizontalAxis.Positive = Keys.D;
            horizontalAxis.Negative = Keys.A;
            verticalAxis.Positive = Keys.S;
            verticalAxis.Negative = Keys.W;

            maxSpeed = 2;
            bulletSpeed = 10;
            shootAnimTimer = 0;

            gameOver = false;
            gameCompleted = false;

            direction = -1;

            checkpoint1 = false;
            checkpoint2 = false;
            checkpoint3 = false;

            musicChange = false;

            bosscheckpoint1 = false;
            bosscheckpoint2 = false;
            bosscheckpoint3 = false;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D square = Content.Load<Texture2D>("Square");

            enemyLeft = Content.Load<Texture2D>("slimenemy-left");
            enemyRight = Content.Load<Texture2D>("slimenemy-right");
            
            font = Content.Load<SpriteFont>("font");

            gunshot = Content.Load<SoundEffect>("sfx_gunshot");
            enemyDeath = Content.Load<SoundEffect>("sfx_enemy_death");
            enemyPop = enemyDeath.CreateInstance();

            normal = Content.Load<SoundEffect>("bgm_happy");
            bossMusic = Content.Load<SoundEffect>("bossthemesnowy_stefan");
            normalBgm = normal.CreateInstance();
            bossBgm = bossMusic.CreateInstance();
            normalBgm.IsLooped = true;
            bossBgm.IsLooped = true;

            victory = Content.Load<SoundEffect>("deaththeme_stefan");
            victoryBgm = victory.CreateInstance();
            victoryBgm.IsLooped = true;

            normalBgm.Play();

            killCount = new ProgressBar(square, Color.Green, 0f, 15f, 3, 0);
            killCount.Scale = new Vector2(5, 1);
            killCount.Position = new Vector2(GraphicsDevice.Viewport.Width / 2 - 75, GraphicsDevice.Viewport.Height - 50);

            slim = new AnimatedSprite(Content.Load<Texture2D>("slimsheet"), 4, 0, 5, 2,10);
            slim.Position = new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height / 2);
            slim.Origin = Vector2.UnitX * 32 + Vector2.UnitY * 19;

            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new Bullet2D(Content.Load<Texture2D>("bullet"), direction, bulletSpeed);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new enemy2D(Content.Load<Texture2D>("slimenemy-right"), GraphicsDevice, slim, 0);
                enemies[i].Origin = Vector2.UnitX  * 32 + Vector2.UnitY * 19;
                enemies[i].left = enemyLeft;
                enemies[i].right = enemyRight;
                enemies[i].Spawn();
            }

            boss = new enemy2D(Content.Load<Texture2D>("slimenemy-right"), GraphicsDevice, slim, 0);
            boss.Dead = true;
            boss.Origin = Vector2.UnitX * 32 + Vector2.UnitY * 19;
            boss.left = enemyLeft;
            boss.right = enemyRight;
            boss.Scale = Vector2.One * 3;
            boss.Position = Vector2.One * -15;
            boss.moveSpeed = 50;
            for (int i = 0; i < bossBullets.Length; i++)
            {
                bullets[i] = new Bullet2D(Content.Load<Texture2D>("bullet"), direction, bulletSpeed);
            }

            for (int i = 0; i < horizontalEnemies.Length; i++)
            {
                horizontalEnemies[i] = new enemy2D(Content.Load<Texture2D>("slimenemy-right"), GraphicsDevice, slim, 1);
                horizontalEnemies[i].Origin = Vector2.UnitX * 32 + Vector2.UnitY * 19;
                horizontalEnemies[i].left = enemyLeft;
                horizontalEnemies[i].right = enemyRight;
                horizontalEnemies[i].moveSpeed = 200;

                float step = GraphicsDevice.Viewport.Height / horizontalEnemies.Length;
                float start = step / 2;
                if (rand.Next(0,2) == 0)
                {
                    
                    horizontalEnemies[i].Position = new Vector2(-16, step * (i + 1) - start);
                    horizontalEnemies[i].Texture = horizontalEnemies[i].right;
                }
                else
                {
                    horizontalEnemies[i].Position = new Vector2(GraphicsDevice.Viewport.Width,step * (i + 1) - start);
                    horizontalEnemies[i].Texture = horizontalEnemies[i].left;
                }
                
            }

            for (int i = 0; i < verticalEnemies.Length; i++)
            {
                verticalEnemies[i] = new enemy2D(Content.Load<Texture2D>("slimenemy-right"), GraphicsDevice, slim, 2);
                verticalEnemies[i].Origin = Vector2.UnitX * 32 + Vector2.UnitY * 19;
                verticalEnemies[i].left = enemyLeft;
                verticalEnemies[i].right = enemyRight;
                verticalEnemies[i].moveSpeed = 200;
                verticalEnemies[i].Rotation += MathHelper.PiOver2;

                float step = GraphicsDevice.Viewport.Width / verticalEnemies.Length;
                float start = step / 2;
                if (rand.Next(0, 2) == 0)
                {

                    verticalEnemies[i].Position = new Vector2(step * (i + 1) - start, 0);
                    verticalEnemies[i].Texture = verticalEnemies[i].right;
                }
                else
                {
                    verticalEnemies[i].Position = new Vector2(step * (i + 1) - start, GraphicsDevice.Viewport.Height);
                    verticalEnemies[i].Texture = verticalEnemies[i].left;
                }
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            
            InputManager.Update();
            Time.Update(gameTime);

            horizontalAxis.Update();
            verticalAxis.Update();

            //enemies[0].Scale = new Vector2(-1, -2);

            if (!gameCompleted && !gameOver)
            {
                if (killCount.Value == killCount.MaxValue && boss.Dead)
                {
                    boss.Dead = false;
                    foreach (enemy2D enemy in enemies)
                    {
                        enemy.Dead = true;
                    }

                    killCount.BarColor = Color.Red;

                    normalBgm.Stop();
                    bossBgm.Play();

                }

                if(killCount.Value == 0 && !boss.Dead)
                {
                    boss.Dead = true;
                    gameCompleted = !gameCompleted;
                    victoryBgm.Play();
                }

                bulletCount = 0;
                foreach(Bullet2D bullet in bullets)
                {
                    if(bullet.Usable)
                    {
                        bulletCount++;
                    }
                   
                }

                //slim updates and controls
                slim.Update();

                if (slim.Position.X > GraphicsDevice.Viewport.Width - 30)
                {
                    slim.Position = new Vector2(GraphicsDevice.Viewport.Width - 30, slim.Position.Y);
                }

                if (slim.Position.X < 16)
                {
                    slim.Position = new Vector2(16, slim.Position.Y);
                }

                if (slim.Position.Y < 20)
                {
                    slim.Position = new Vector2(slim.Position.X, 20);
                }

                if (slim.Position.Y > GraphicsDevice.Viewport.Height - 20)
                {
                    slim.Position = new Vector2(slim.Position.X, GraphicsDevice.Viewport.Height - 20);
                }

                

                if (InputManager.IsKeyDown(Keys.W) ||
                    InputManager.IsKeyDown(Keys.S) ||
                    InputManager.IsKeyDown(Keys.A) ||
                    InputManager.IsKeyDown(Keys.D))
                {
                    
                    slim.Position += (Vector2.UnitX * horizontalAxis.Value + Vector2.UnitY * verticalAxis.Value) * maxSpeed;
                    
                    if(InputManager.IsKeyDown(Keys.A))
                    {
                        direction = -1;
                    }
                    else if(InputManager.IsKeyDown(Keys.D))
                    {
                        direction = 1;
                    }
                }

                if (InputManager.IsKeyPressed(Keys.Space))
                {
                    
                    
                    foreach (Bullet2D bullet in bullets)
                    {
                        if(bullet.Usable)
                        {
                            gunshot.CreateInstance().Play();
                            bullet.Direction = direction;
                            bullet.Position = slim.Position;// + Vector2.UnitY * (slim.Height/2);
                            bullet.Active = true;
                            bullet.Usable = false;

                            slim.Clip = 2;
                            slim.Speed = 0;

                            if (direction == -1)
                            {
                                slim.Frame = 1;
                            }
                            else
                            {
                                slim.Frame = 0;
                            }
                            break;
                        }
                 
                    }

                }

                if (slim.Clip == 2 && (slim.Frame == 0 || slim.Frame == 1))
                {
                    shootAnimTimer += 0.1f;
                    if(shootAnimTimer >= 5)
                    {
                        shootAnimTimer = 0;
                        if (horizontalAxis.Value != 0 || verticalAxis.Value != 0)
                        {
                            slim.Speed = 0;
                            if (direction == -1)
                            {
                                slim.Frame = 3;
                            }
                            else
                            {
                                slim.Frame = 2;
                            }
                        }
                        else
                        {
                            slim.Speed = 10;
                            if (direction == -1)
                            {
                                slim.Clip = 1;
                            }
                            else
                            {
                                slim.Clip = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (horizontalAxis.Value != 0 || verticalAxis.Value != 0)
                    {
                        slim.Speed = 0;
                        slim.Clip = 2;
                        if (direction == -1)
                        {
                            slim.Frame = 3;
                        }
                        else
                        {
                            slim.Frame = 2;
                        }
                    }
                    else
                    {
                        slim.Speed = 10;
                        if (direction == -1)
                        {
                            slim.Clip = 1;
                        }
                        else
                        {
                            slim.Clip = 0;
                        }
                    }
                }

                //bullet interactions

                foreach (Bullet2D bullet in bullets)
                {
                    bullet.Update();
                    if (bullet.Position.X > GraphicsDevice.Viewport.Width ||
                        bullet.Position.X < 0 || bullet.Position.Y < 0 ||
                        bullet.Position.Y > GraphicsDevice.Viewport.Height)
                    {
                        bullet.Active = false;
                        bullet.Usable = true;
                    }
                    
                    foreach (enemy2D enemy in enemies)
                    {
                        if (bullet.Active && (bullet.Position - enemy.Position).Length() < 16 && !enemy.Dead)
                        {
                            
                            enemyPop.Play();
                            bullet.Active = false;
                            enemy.Dead = true;
                            

                            if (boss.Dead)
                            {
                                killCount.Value += killCount.MaxValue * 0.05f;
                                enemy.deadTimer = rand.Next(4, 8);
                            }
                            else
                            {
                                enemy.deadTimer = rand.Next(7, 10);
                            }
                            
                        }
                    }

                    if(!boss.Dead)
                    {
                        if (bullet.Active && (bullet.Position - boss.Position).Length() < 16)
                        {
                            killCount.Value -= killCount.MaxValue * 0.05f;
                            bullet.Active = false;
                        }
                    }

                    if(checkpoint2)
                    {
                        foreach (enemy2D enemy in horizontalEnemies)
                        {
                            if (bullet.Active && (bullet.Position - enemy.Position).Length() < 16 && !enemy.Dead)
                            {

                                enemyPop.Play();
                                bullet.Active = false;
                                enemy.Dead = true;
                                enemy.deadTimer = rand.Next(8, 10);

                            }
                        }

                        foreach (enemy2D enemy in verticalEnemies)
                        {
                            if (bullet.Active && (bullet.Position - enemy.Position).Length() < 16 && !enemy.Dead)
                            {

                                enemyPop.Play();
                                bullet.Active = false;
                                enemy.Dead = true;
                                enemy.deadTimer = rand.Next(8, 10);

                            }
                        }
                    }
                    
                }
                
                //enemy interactions

                if(boss.Dead)
                {
                    foreach (enemy2D enemy in enemies)
                    {
                        enemy.Update();
                        if ((slim.Position - enemy.Position).Length() < 32 && !enemy.Dead)
                        {
                            gameOver = true;
                        }
                    }

                    if (!checkpoint1 && killCount.Value >= killCount.MaxValue * 0.25f)
                    {
                        foreach (enemy2D enemy in enemies)
                        {
                            enemy.moveSpeed += 10;
                        }
                        checkpoint1 = true;
                    }
                    if (!checkpoint2 && killCount.Value >= killCount.MaxValue * 0.5f)
                    {
                        foreach (enemy2D enemy in enemies)
                        {
                            enemy.moveSpeed += 10;
                        }
                        checkpoint2 = true;
                    }
                    if (!checkpoint3 && killCount.Value >= killCount.MaxValue * 0.75f)
                    {
                        foreach (enemy2D enemy in enemies)
                        {
                            enemy.moveSpeed += 10;
                        }
                        maxSpeed *= 2;
                        checkpoint3 = true;
                    }
                    
                }
                else
                {
                    boss.Update();
                    if ((slim.Position - boss.Position).Length() < 32*1.5f)
                    {
                        gameOver = true;
                    }

                    if (!bosscheckpoint1 && killCount.Value <= killCount.MaxValue * 0.75f)
                    {
                        boss.moveSpeed += 25;
                        bosscheckpoint1 = true;
                    }

                    if (killCount.Value <= killCount.MaxValue * 0.5f)
                    {
                        for (int i = 0; i < horizontalEnemies.Length; i++)
                        {
                            horizontalEnemies[i].Update();
                            if ((slim.Position - horizontalEnemies[i].Position).Length() < 32 && !horizontalEnemies[i].Dead)
                            {
                                gameOver = true;
                            }
                        }

                        for (int i = 0; i < verticalEnemies.Length; i++)
                        {
                            verticalEnemies[i].Update();
                            if ((slim.Position - verticalEnemies[i].Position).Length() < 32 && !verticalEnemies[i].Dead)
                            {
                                gameOver = true;
                            }
                        }
                        
                        if(!bosscheckpoint2)
                        {
                            bosscheckpoint2 = true;
                        }
                    }

                    if (killCount.Value <= killCount.MaxValue * 0.25f)
                    {
                        if(!bosscheckpoint3)
                        {
                            foreach (enemy2D enemy in enemies)
                            {
                                enemy.Dead = false;
                                enemy.Spawn();
                            }
                            bosscheckpoint3 = true;
                        }
                        else
                        {
                            foreach (enemy2D enemy in enemies)
                            {
                                enemy.Update();
                            }
                        }
                        
                    }
                    

                }

                


            }
            else //restart or quit
            {
                if(InputManager.IsKeyPressed(Keys.Enter))
                {
                    killCount.Value = 0;
                    slim.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                    foreach (Bullet2D bullet in bullets)
                    {
                        bullet.Usable = true;
                        bullet.Active = false;
                    }

                    foreach (enemy2D enemy in enemies)
                    {
                        enemy.moveSpeed = 30;
                        enemy.Spawn();
                    }

                    foreach (enemy2D enemy in horizontalEnemies)
                    {
                        enemy.Spawn();
                    }

                    foreach (enemy2D enemy in verticalEnemies)
                    {
                        enemy.Spawn();
                    }

                    maxSpeed = 2;
                    gameCompleted = false;
                    gameOver = false;
                    boss.Dead = true;
                    boss.moveSpeed = 50;
                    boss.Position = Vector2.One * -15;
                    checkpoint1 = false;
                    checkpoint2 = false;
                    checkpoint3 = false;

                    bosscheckpoint1 = false;
                    bosscheckpoint2 = false;
                    bosscheckpoint3 = false;

                    victoryBgm.Stop();
                    bossBgm.Stop();
                    normalBgm.Play();
                }

            }

            killCount.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
                                  
            if (gameOver)
            {
                _spriteBatch.DrawString(font, "You died :(",
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2), Color.Black);
                _spriteBatch.DrawString(font, "Press esc to quit or enter to restart",
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2 + 20), Color.Black);
            }
            else if (gameCompleted)
            {
                _spriteBatch.DrawString(font, "You did it! You beat all the enemies!",
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2), Color.Black);
                _spriteBatch.DrawString(font, "Press esc to quit or enter to restart",
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2 + 20), Color.Black);
            }
            else
            {

                _spriteBatch.DrawString(font, "Use WASD to move slim (blue guy)",
                           new Vector2(10, 10), Color.Black);
                _spriteBatch.DrawString(font, "Press space to shoot a gun",
                           new Vector2(10, 30), Color.Black);
                if (boss.Dead)
                {
                    _spriteBatch.DrawString(font, "Fill up the progress bar by killing enemies",
                           new Vector2(10, 50), Color.Black);
                }
                else
                {
                    _spriteBatch.DrawString(font, "Shoot the boss to reduce their HP",
                           new Vector2(10, 50), Color.Black);

                }

                _spriteBatch.DrawString(font, "Bullets: " + bulletCount,
                           new Vector2(GraphicsDevice.Viewport.Width - 100, 10), Color.Black);


                killCount.Draw(_spriteBatch);

                slim.Draw(_spriteBatch);
                foreach (Sprite bullet in bullets)
                {
                    bullet.Draw(_spriteBatch);
                }
                foreach (enemy2D enemy in enemies)
                {
                    enemy.Draw(_spriteBatch);
                }
                
                if(bosscheckpoint2)
                {
                    for (int i = 0; i < horizontalEnemies.Length; i++)
                    {
                        horizontalEnemies[i].Draw(_spriteBatch);
                    }

                    for (int i = 0; i < verticalEnemies.Length; i++)
                    {
                        verticalEnemies[i].Draw(_spriteBatch);
                    }
                    
                }

                boss.Draw(_spriteBatch);

            }
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void SpawnEnemy()
        {

        }
        
    }
}
