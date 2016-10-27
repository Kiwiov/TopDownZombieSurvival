using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
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
        ZombieManager zombieManager;
        Camera cam;

        Texture2D character;
        Texture2D bullet;
        Texture2D curs;
        Texture2D zombieTextureOne;
        Texture2D zombieTextureOneAttack;
        Texture2D zombieTextureTwo;
        Texture2D zombieTextureTwoAttack;

        Vector2 position;
        Vector2 previusPosition;
        Vector2 dPos;
        Vector2 mousePos;
        Vector2 mousePosition;
        
        List<Bullet> shots;

        Song bSong;
        SoundEffect shot;

        float rot;
        float stamina;
        
        float playerHitBoxRadius;
        

        MouseState ms;
        MouseState previousMs;

        PathFinder finder;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            
            //graphics.IsFullScreen = true;
        }

        
        protected override void Initialize()
        {
            position = new Vector2(2368,1800);
            previusPosition = position;
            cam = new Camera();
            mousePosition = new Vector2();
            base.Initialize();
            playerHitBoxRadius = 25;
            shots = new List<Bullet>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map = new TmxMap("forest.tmx");
            tileEngineGood = new TileEngineGood(map);
            tileEngineGood.LoadContent(this);
            tileEngineGood.FindHitBoxes();
            tileEngineGood.FindSpawnZones();
            int temp = tileEngineGood.spawnZones.Count;

            finder = new PathFinder(map);

            zombieManager = new ZombieManager(Content.Load<Texture2D>("zoimbie1_stand"), Content.Load<Texture2D>("zoimbie1_hold"), Content.Load<Texture2D>("zombie2_stand"), Content.Load<Texture2D>("zombie2_hold"), tileEngineGood);
            zombieManager.SpawnZombies();
            
            character = Content.Load<Texture2D>("hitman2_gun");
            bullet = Content.Load<Texture2D>("bullet");
            curs = Content.Load<Texture2D>("crshair_36px");
            bSong = Content.Load<Song>("bsong");
            shot = Content.Load<SoundEffect>("KiaGun");
            MediaPlayer.Play(bSong);
            MediaPlayer.IsRepeating = true;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            
                zombieManager.MoveZombie(finder, position);
            

            if (zombieManager.zombies.Count == 0)
            {
                zombieManager.round++;
                zombieManager.SpawnZombies();
            }
      
            KeyboardState kb = Keyboard.GetState();

            previousMs = ms;
            ms = Mouse.GetState();
            
            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
            {
                shot.Play();
                shots.Add(new Bullet(bullet, position, new Vector2(25 * (float)Math.Cos(rot), 25 * (float)Math.Sin(rot)), rot));
            }
            
            
            foreach (var bullet in shots)
            {
                bullet.position += bullet.speed;
            }

            foreach (var hitbox in tileEngineGood.mapHitBoxes)
            {
                for (int i = 0; i < shots.Count; i++)
                {
                    if (shots.Count > 0 && hitbox.Contains(shots[i].position))
                    {
                        shots.Remove(shots[i]);
                    }
                }
            }

            #region Movemente
            
            if ((kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift)) && stamina > 0)
            {
                
                if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) && (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 3.5355339059327376220042218105242f;
                    }
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 3.5355339059327376220042218105242f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) && (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 3.5355339059327376220042218105242f;
                    }
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 3.5355339059327376220042218105242f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) && (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 3.5355339059327376220042218105242f;
                    }
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 3.5355339059327376220042218105242f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) && (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 3.5355339059327376220042218105242f;
                    }
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 3.5355339059327376220042218105242f;
                    }
                }
                else if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 5;
                    }
                }

                else if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 5;
                    }
                }

                else if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
                {
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 5;
                    }
                }
                else if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
                {
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 5;
                    }
                }

                stamina -= 5;
            }
            else
            {
                if (stamina <= 25 * 60)
                {
                    stamina += 1;
                }
                if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) && (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 2.1213203435596425732025330863145f;
                    }
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 2.1213203435596425732025330863145f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) && (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 2.1213203435596425732025330863145f;
                    }
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 2.1213203435596425732025330863145f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) && (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 2.1213203435596425732025330863145f;
                    }
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 2.1213203435596425732025330863145f;
                    }
                }
                else if ((kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) && (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 2.1213203435596425732025330863145f;
                    }
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 2.1213203435596425732025330863145f;
                    }
                }
                else if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
                {
                    if (MapHitBoxHit("x") != "left")
                    {
                        previusPosition = position;
                        position.X -= 3;
                    }
                }
                else if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
                {
                    if (MapHitBoxHit("x") != "right")
                    {
                        previusPosition = position;
                        position.X += 3;
                    }
                }

                else if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
                {
                    if (MapHitBoxHit("y") != "above")
                    {
                        previusPosition = position;
                        position.Y -= 3;
                    }
                }
                else if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
                {
                    if (MapHitBoxHit("y") != "below")
                    {
                        previusPosition = position;
                        position.Y += 3;
                    }
                }
            }
#endregion


            cam.Pos = position;
            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            mousePosition = new Vector2(mousePos.X, mousePos.Y) + cam.pos - new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            dPos = mousePosition - position;
            rot = (float)Math.Atan2(dPos.Y, dPos.X);
            base.Update(gameTime);
        }

        private string MapHitBoxHit(string axis)
        {
            foreach (var hitbox in tileEngineGood.playerMapHitBoxes)
            {
                if (hitbox.Contains(position))
                {
                    if (axis == "x")
                    {
                        if (position.X - hitbox.X < hitbox.X + hitbox.Width - position.X)
                        {
                            position = previusPosition;
                            return "right";
                        }
                        else
                        {
                            position = previusPosition;
                            return "left";
                        }
                    }
                    if (axis == "y")
                    {
                        if (position.Y - hitbox.Y < hitbox.Y + hitbox.Height - position.Y)
                        {
                            position = previusPosition;
                            return "below";

                        }
                        else
                        {
                            position = previusPosition;
                            return "above";
                        }
                    }
                }
                
            }
            return "none";
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            spriteBatch.Begin(SpriteSortMode.Deferred,
                                BlendState.AlphaBlend,
                                SamplerState.PointClamp,
                                DepthStencilState.Default,
                                RasterizerState.CullNone,
                                null,
                                cam.get_transformation(GraphicsDevice));
            tileEngineGood.Draw(spriteBatch);
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                                BlendState.AlphaBlend,
                                SamplerState.AnisotropicWrap,
                                DepthStencilState.Default,
                                RasterizerState.CullNone,
                                null,
                                cam.get_transformation(GraphicsDevice));
            spriteBatch.Draw(character, position, null, color: Color.White, rotation: rot, origin: new Vector2(character.Bounds.Center.X, character.Bounds.Center.Y));
            zombieManager.Draw(spriteBatch);
            spriteBatch.Draw(curs, mousePosition, null, color: Color.RoyalBlue, rotation: 0, origin: new Vector2(curs.Bounds.Center.X, curs.Bounds.Center.Y));
            foreach (var shot in shots)
            {
                spriteBatch.Draw(shot.texture, shot.position, null, color: Color.White, rotation: shot.rotation + (float)Math.PI, origin: new Vector2(shot.texture.Bounds.Center.X, shot.texture.Bounds.Center.Y));
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}