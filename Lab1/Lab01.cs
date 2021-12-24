using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab1
{
    public class Lab01 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont font;
        private Fraction a = new Fraction(3, 4);
        private Fraction b = new Fraction(8, 3);

        public Lab01()
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

            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            
            _spriteBatch.DrawString(font, a + " + " + b + " = " + (a + b),
                        new Vector2(50, 50), Color.Black);
            _spriteBatch.DrawString(font, a + " - " + b + " = " + (a - b),
                        new Vector2(50, 100), Color.Black);
            _spriteBatch.DrawString(font, a + " * " + b + " = " + (a * b),
                        new Vector2(50, 150), Color.Black);
            _spriteBatch.DrawString(font, a + " / " + b + " = " + (a / b),
                        new Vector2(50, 200), Color.Black);
            
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
