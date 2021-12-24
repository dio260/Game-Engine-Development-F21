using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CPI311.GameEngine.Managers;

namespace CPI311.GameEngine
{
    public class Button : GUIElement
    {
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.GetMousePosition()))
                OnAction();
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, Text,
                new Vector2(Bounds.X + Bounds.Width/2 - font.MeasureString(Text).X/2,
                            Bounds.Y + Bounds.Height/2 - font.MeasureString(Text).Y / 2), 
                            Color.Black);
        }
    }

}
