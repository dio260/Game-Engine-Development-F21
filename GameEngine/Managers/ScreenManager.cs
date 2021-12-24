using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine.Managers
{

    public static class ScreenManager
    {
        private static GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicsDevice
        {
            get { return graphics.GraphicsDevice; }
        }

        public static void Initialize(GraphicsDeviceManager g)
        {
            graphics = g;
        }
        public static int Width
        {
            get
            {
                return GraphicsDevice.PresentationParameters.BackBufferWidth;
            }
            set
            {
                graphics.PreferredBackBufferWidth = value;
                graphics.ApplyChanges();
            }
        }
        public static int Height
        {
            get
            {
                return GraphicsDevice.PresentationParameters.BackBufferHeight;
            }
            set
            {
                graphics.PreferredBackBufferHeight = value;
                graphics.ApplyChanges();
            }
        }
        public static bool IsFullScreen
        {
            get
            {
                return GraphicsDevice.PresentationParameters.IsFullScreen;
            }
            set
            {
                graphics.GraphicsDevice.PresentationParameters.IsFullScreen = value;
                graphics.ApplyChanges();
            }
        }

        public static void Setup(int width = 0, int height = 0)
        {
            Setup(IsFullScreen, width, height);
        }

        public static void Setup(bool fullScreen, int width = 0, int height = 0)
        {
            if (width > 0)
                graphics.PreferredBackBufferWidth = width;
            if (height > 0)
                graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = fullScreen;
            graphics.ApplyChanges();
        }
    }
}
