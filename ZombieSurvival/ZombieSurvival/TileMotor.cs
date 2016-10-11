using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
 
namespace ZombieSurvival
{
    class TileEngine : GameComponent
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int[,] Data { get; set; }
        public Texture2D TileMap { get; set; }
        public Vector2 CameraPosition { get; set; }
 
        private int viewportWidth, viewportHeight;
 
        public override void Initialize()
        {
            viewportWidth = Game.GraphicsDevice.Viewport.Width;
            viewportHeight = Game.GraphicsDevice.Viewport.Height;
 
            base.Initialize();
        }
 
        public TileEngine(Game game) : base(game)
        {
            game.Components.Add(this);
            CameraPosition = Vector2.Zero;
        }
         
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
    }
}
