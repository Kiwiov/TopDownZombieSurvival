using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledSharp;

namespace ZombieSurvival
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileEngineGood tileEngineGood;
        TmxMap map;
        Vector2 position;
        Texture2D character;
        Vector2 dPos;
        Vector2 mousePos;
        Texture2D curs;
        float rot;
        

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        
        protected override void Initialize()
        {
            position = new Vector2(300,300);
            



            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map = new TmxMap("house.tmx");
            tileEngineGood = new TileEngineGood(map);
            tileEngineGood.LoadContent(this);
            character = Content.Load<Texture2D>("manBlue_gun");
            curs = Content.Load<Texture2D>("crshair_36px");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            


            GamePadCapabilities c = GamePad.GetCapabilities(PlayerIndex.One);
            if (c.IsConnected)
            {
                GamePadState state = GamePad.GetState(PlayerIndex.One);
                if (c.HasLeftXThumbStick)
                {
                    position.X += state.ThumbSticks.Left.X * 10.0f;
                }
                if (c.GamePadType == GamePadType.GamePad)
                {
                    if (state.IsButtonDown(Buttons.A))
                    {
                        Exit();
                    }
                }
            }

            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                position.X = position.X - 5;
            }

            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                position.X = position.X + 5;
            }
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
            {
                position.Y = position.Y - 5;
            }
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
            {
                position.Y = position.Y + 5;
            }
            



            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            dPos = mousePos - position;
            rot = (float) Math.Atan2(dPos.Y, dPos.X);


            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            tileEngineGood.Draw(spriteBatch);
            spriteBatch.Draw(character, position, null, color:Color.White, rotation:rot, origin: new Vector2(character.Bounds.Center.X, character.Bounds.Center.Y));
            spriteBatch.Draw(curs, mousePos, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}