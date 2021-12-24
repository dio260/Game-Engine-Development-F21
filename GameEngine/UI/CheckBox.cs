using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CPI311.GameEngine.Managers;

namespace CPI311.GameEngine
{
    public class CheckBox : GUIElement
    {
        bool check;
        public Texture2D Box;

        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.GetMousePosition()))
            {
                OnAction();
                check = !check;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            int width = Math.Min(Bounds.Width, Bounds.Height);
            spriteBatch.Draw(Box, new Rectangle(Bounds.X, Bounds.Y, width, width),
                               check ? Color.Red : Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width, Bounds.Y), Color.Black);
        }
    }
}
