using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;
using CPI311.GameEngine.Rendering;

namespace Assignment1
{

    public class Assignment1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Random rnd = new Random();

        private SpriteFont font;

        private AnimatedSprite player;
        private Sprite bonus;
        private ProgressBar timeBar, distanceBar;
        private Axis horizontalAxis, verticalAxis;
        private int maxSpeed;

        private bool gameOver, gameCompleted;

        public Assignment1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InputManager.Initialize();
            Time.Initialize();
            horizontalAxis = new Axis();
            verticalAxis = new Axis();
            horizontalAxis.Positive = Keys.None;
            horizontalAxis.Negative = Keys.None;
            verticalAxis.Positive = Keys.Up;
            verticalAxis.Negative = Keys.None;

            maxSpeed = 2;

            gameOver = false;
            gameCompleted = false;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D square = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");
            Texture2D clock = Content.Load<Texture2D>("Stopwatch");
            bonus = new Sprite(clock , new Vector2(
                                        rnd.Next((int)clock.Width/2, GraphicsDevice.Viewport.Width - (int) (clock.Width / 2)),
                                        rnd.Next((int)clock.Height / 2, GraphicsDevice.Viewport.Height - (int)(clock.Height / 2))
                                        ));
            player = new AnimatedSprite(Content.Load<Texture2D>("explorer"), 8, 0, 5, 1, 10);
            player.Origin = new Vector2(player.Width/2, player.Height/2);
            player.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            player.Update();
            
            
            timeBar = new ProgressBar(square, Color.Red , 15f, 15f, 3, 0);
            timeBar.Position = new Vector2(130, 10);
 
            distanceBar = new ProgressBar(square, Color.Green, 0, 2500, 3);
            distanceBar.Position = new Vector2(355, 10);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(!gameCompleted && !gameOver)
            {
                if (distanceBar.Value == distanceBar.MaxValue)
                    gameCompleted = !gameCompleted;
                else if (timeBar.Value == 0)
                    gameOver = !gameOver;
            }
            // TODO: Add your update logic here
            InputManager.Update();
            Time.Update(gameTime);
            
            horizontalAxis.Update();
            verticalAxis.Update();

            if(gameOver || gameCompleted)
            {
                timeBar.Speed = 0;
                distanceBar.Speed = 0;

            }
            else
            {
                if (InputManager.IsKeyDown(Keys.Up))
                {
                    player.Position += (Vector2.UnitX * horizontalAxis.Value +
                                    Vector2.UnitY * verticalAxis.Value) * maxSpeed;
                    distanceBar.Speed = Math.Abs(horizontalAxis.Value * maxSpeed) + Math.Abs(verticalAxis.Value * maxSpeed);
                    player.Update();
                }
                else
                {
                    distanceBar.Speed = 0;
                }

                if (InputManager.IsKeyPressed(Keys.Left))
                {
                    switch (player.Clip)
                    {

                        case (4):
                            player.Clip = 2;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.Up;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.None;
                            break;
                        case (2):
                            player.Clip = 1;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.Up;
                            verticalAxis.Negative = Keys.None;
                            break;
                        case (3):
                            player.Clip = 4;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.Up;
                            break;
                        case (1):
                            player.Clip = 3;
                            horizontalAxis.Positive = Keys.Up;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.None;
                            break;
                        case (5):
                            //how did you get here lol
                            break;
                    }
                    player.Update();
                }

                if (InputManager.IsKeyPressed(Keys.Right))
                {
                    switch (player.Clip)
                    {
                        case (1):
                            player.Clip = 2;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.Up;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.None;
                            break;
                        case (4):
                            player.Clip = 3;
                            horizontalAxis.Positive = Keys.Up;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.None;
                            break;
                        case (2):
                            player.Clip = 4;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.None;
                            verticalAxis.Negative = Keys.Up;
                            break;
                        case (3):
                            player.Clip = 1;
                            horizontalAxis.Positive = Keys.None;
                            horizontalAxis.Negative = Keys.None;
                            verticalAxis.Positive = Keys.Up;
                            verticalAxis.Negative = Keys.None;
                            break;

                    }
                    player.Update();
                }

                if ((player.Position - bonus.Position).Length() < 32 || InputManager.IsKeyPressed(Keys.Space))
                {
                    timeBar.Value += timeBar.MaxValue * 0.2f;
                    bonus.Position = new Vector2(
                                            rnd.Next((int)bonus.Texture.Width / 2, GraphicsDevice.Viewport.Width - (int)(bonus.Texture.Width / 2)),
                                            rnd.Next((int)bonus.Texture.Height / 2, GraphicsDevice.Viewport.Height - (int)(bonus.Texture.Height / 2))
                                            );

                }

                timeBar.Speed = -Time.ElapsedGameTime;

                timeBar.Update();
                distanceBar.Update();
            }

            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            if (gameOver)
            {
                _spriteBatch.DrawString(font, "Time's up! Guess he didn't walk fast enough",
                           new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2), Color.Black);
            }
            else if(gameCompleted)
            {
                _spriteBatch.DrawString(font, "You did it! The guy really got his steps in",
                           new Vector2(GraphicsDevice.Viewport.Width/2 - 150, GraphicsDevice.Viewport.Height/2), Color.Black);
            }
            else
            {
                _spriteBatch.DrawString(font, "Time Remaining:",
                        new Vector2(10, 20), Color.Red);
                _spriteBatch.DrawString(font, "Distance Walked:",
                            new Vector2(230, 20), Color.Black);

                _spriteBatch.DrawString(font, "Use left and right to turn the guy (left and right based on the guy's left and right)",
                           new Vector2(150, 410), Color.Black);
                _spriteBatch.DrawString(font, "Hold up to make the guy walk in the direction he's facing",
                           new Vector2(185, 435), Color.Black);
                _spriteBatch.DrawString(font, "Fill up the bar by walking before time runs out! (walk over clocks to get more time)",
                           new Vector2(140, 460), Color.Black);
                bonus.Draw(_spriteBatch);
                player.Draw(_spriteBatch);
                timeBar.Draw(_spriteBatch);
                distanceBar.Draw(_spriteBatch);
            }
            
           _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
