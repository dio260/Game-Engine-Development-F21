using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CPI311.GameEngine;
using CPI311.GameEngine.Managers;

namespace Lab11
{
    public class Lab11 : Game
    {
        class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D texture;
        Color background;
        Button exit;
        SpriteFont font;

        Dictionary<string, Scene> scenes;
        Scene currentScene;

        List<GUIElement> guiElements;

        
        public Lab11()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            guiElements = new List<GUIElement>();
            scenes = new Dictionary<string, Scene>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");
            GUIGroup group = new GUIGroup();

            exit = new Button();
            exit.Texture = texture;
            exit.Text = "exit game";
            exit.Bounds = new Rectangle(100, 100, 100, 50);
            exit.Action += ExitGame;
            group.Children.Add(exit);

            CheckBox optionBox = new CheckBox();
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(50, 75, 300, 20);
            optionBox.Action += MakeFullScreen;
            optionBox.Text = "Full Screen";
            group.Children.Add(optionBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Time.Update(gameTime);
            InputManager.Update();
            //exit.Update();

            currentScene.Update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            // TODO: Add your drawing code here
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            // Call the draw of the "current state"
            //currentScene.Draw();
            _spriteBatch.Begin();
            exit.Draw(_spriteBatch, font);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void ExitGame(GUIElement element)
        {
            background = (background == Color.White ? Color.Blue : Color.White);
            //currentScene = scenes["Play"];
        }
        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Space))
                currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Play Mode! Press \"Space\" to go back",
            Vector2.Zero, Color.Black);
            _spriteBatch.End();
        }

        void MakeFullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.IsFullScreen,
                                 ScreenManager.Width + 1, ScreenManager.Height + 1);
        }
    }
}
