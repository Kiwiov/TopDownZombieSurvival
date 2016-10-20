using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvival
{
    public class OButton
    {
        Texture2D _texture;
        Vector2 _position;
        Rectangle _rectangle;

        Color _colour = new Color(255, 255, 255, 255);

        private Vector2 _size;

        public OButton(Texture2D newTexture, GraphicsDevice graphics)
        {
            _texture = newTexture;

            // ScreenW = 1920, ScreenH = 1080
            // ImgW    = 500, ImgH    = 100
            _size = new Vector2(graphics.Viewport.Width / 8, graphics.Viewport.Height / 30);
        }

        bool _down;
        public bool IsClicked;

        public void Update(MouseState mouse)
        {
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y,
            (int)_size.X, (int)_size.Y);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(_rectangle))
            {
                if (_colour.A == 255) _down = false;
                if (_colour.A == 0) _down = true;
                if (_down) _colour.A += 3;
                else _colour.A -= 3;
                if (mouse.LeftButton == ButtonState.Pressed) IsClicked = true;
            }
            else if (_colour.A < 255)
            {
                _colour.A += 3;
                IsClicked = false;
            }
        }

        public void SetPosition(Vector2 newPostion)
        {
            _position = newPostion;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _rectangle, _colour);
        }
    }
}
