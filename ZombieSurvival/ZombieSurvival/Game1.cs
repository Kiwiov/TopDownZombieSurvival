using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Policy;
using System.Diagnostics;
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
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ZombieSurvival
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GameState GameState;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileEngineGood tileEngineGood;
        TmxMap map;
        ZombieManager zombieManager;
        Camera cam;
        bool reloading;

        Texture2D secondary;
        Texture2D secondaryReload;
        Texture2D primary;
        Texture2D primaryReload;
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
        SoundEffect hit;
        SoundEffect dying;
        SoundEffect click;
        SoundEffect secondaryReloadSound;
        SoundEffect primaryReloadSound;

        float rot;
        float stamina;
        float playerHitBoxRadius;
        float health;
        float timeTillHeal;
        float healTime;

        int timeBetweenRounds;
        int timeBetweenRoundsTime;
        int switchTimer;
        int weaponCooldown;

        private int primaryMagazine;
        private int primaryTotalAmmo;
        private int primaryReloadSpeed;
        private int secondaryMagazine;
        private int secondaryTotalAmmo;
        private int secondaryReloadSpeed;

        MouseState ms;
        MouseState previousMs;

        PathFinder finder;

        Weapon currentSecondary;
        Weapon currentPrimary;
        Weapon currentWeapon;
        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;

            //graphics.IsFullScreen = true;
        }


        protected override void Initialize()
        {
            position = new Vector2(2000, 1800);
            previusPosition = position;
            Components.Add(new MenuComponent(this));
            Components.Add(new KeyboardComponent(this));
            cam = new Camera();
            mousePosition = new Vector2();
            base.Initialize();
            playerHitBoxRadius = 25;
            shots = new List<Bullet>();
            timeBetweenRounds = 3;
            health = 100;
            timeTillHeal = 5;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            map = new TmxMap("forest.tmx");
            tileEngineGood = new TileEngineGood(map);
            tileEngineGood.LoadContent(this);
            tileEngineGood.FindHitBoxes();
            tileEngineGood.FindSpawnZones();

            finder = new PathFinder(map);

            zombieManager = new ZombieManager(Content.Load<Texture2D>("zoimbie1_stand"), Content.Load<Texture2D>("zoimbie1_hold"), Content.Load<Texture2D>("zombie2_stand"), Content.Load<Texture2D>("zombie2_hold"), tileEngineGood, Content.Load<SoundEffect>("Zombie_groan1"), Content.Load<SoundEffect>("Zombie_groan2"), Content.Load<SoundEffect>("Zombie_groan3"), Content.Load<SoundEffect>("Zombie_groan4"));
            zombieManager.SpawnZombies();

            secondary = Content.Load<Texture2D>("hitman2_gun");
            secondaryReload = Content.Load<Texture2D>("hitman2_stand");
            primary = Content.Load<Texture2D>("hitman2_machine");
            primaryReload = Content.Load<Texture2D>("hitman2_reload");
            bullet = Content.Load<Texture2D>("bullet");
            curs = Content.Load<Texture2D>("crshair_36px");

            bSong = Content.Load<Song>("bsong");

            shot = Content.Load<SoundEffect>("pistolShot");
            hit = Content.Load<SoundEffect>("HitMarkerSound");
            dying = Content.Load<SoundEffect>("DyingZombie");
            click = Content.Load<SoundEffect>("click");
            secondaryReloadSound = Content.Load<SoundEffect>("secondaryReload");
            primaryReloadSound = Content.Load<SoundEffect>("primaryReload");


            //MediaPlayer.Play(bSong);

            MediaPlayer.IsRepeating = true;
            currentSecondary = new Weapon(50, 1, "Pistol", /*21*/ int.MaxValue, 7, 2, 0, 0.03f, "Secondary", secondary, secondaryReload, 1, false);
            currentPrimary = new Weapon(80, 1, "SMG", 120, 30, 2, 2, 0.09f, "Primary", primary, primaryReload, 2, true);
            //currentPrimary = new Weapon(80, 8, "Shotgun", 20, 5, 2, 60, 0.3f, "Primary", primary, primaryReload, 1, false);
            currentWeapon = currentSecondary;

            primaryMagazine = currentPrimary.ammoMagazine;
            primaryTotalAmmo = currentPrimary.ammoTotal;
            primaryReloadSpeed = currentPrimary.reloadSpeed * 60;

            secondaryMagazine = currentSecondary.ammoMagazine;
            secondaryTotalAmmo = currentSecondary.ammoTotal;
            secondaryReloadSpeed = currentSecondary.reloadSpeed * 60;

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            zombieManager.MoveZombie(finder, position);
            
            if (zombieManager.zombies.Count == 0)
            {
                zombieManager._activeSounds.Clear();
                if (timeBetweenRoundsTime == timeBetweenRounds * 60)
                {
                    zombieManager.round++;
                    zombieManager.CalculateZombies();
                    Debug.WriteLine("Round: " + zombieManager.round);
                    Debug.WriteLine("Zombies: " + zombieManager.zombiesToSpawn);
                    zombieManager.SpawnZombies();
                    timeBetweenRoundsTime = 0;
                }
                else
                {
                    timeBetweenRoundsTime++;
                }
            }
            
            if (zombieManager.zombiesToSpawn > 0)
            {
                zombieManager.SpawnZombies();
            }

            zombieManager.ToGroanOrNotToGroan();

            if (zombieManager.ToAttackOrNotToAttack(position))
            {
                health -= zombieManager.zombies[0].damage;
                healTime = 0;
            }

            if (timeTillHeal * 60 == healTime)
            {
                if (health >= 100)
                {
                    health = 100;
                }
                else
                {
                    health += 0.2f;
                }
            }
            else
            {
                healTime++;
            }
            

            if (health <= 0)
            {
                this.Exit();
            }

            KeyboardState kb = Keyboard.GetState();

            previousMs = ms;
            ms = Mouse.GetState();

            if (reloading)
            {
                if (currentWeapon == currentPrimary)
                {
                    if (primaryReloadSpeed != 0)
                    {
                        primaryReloadSpeed--;
                    }
                    else
                    {
                        primaryReloadSpeed = currentPrimary.reloadSpeed * 60;
                        reloading = false;
                    }
                }
                else
                {
                    if (secondaryReloadSpeed != 0)
                    {
                        secondaryReloadSpeed--;
                    }
                    else
                    {
                        secondaryReloadSpeed = currentSecondary.reloadSpeed * 60;
                        reloading = false;
                    }
                }
                
                
            }

            if (currentWeapon.automatic)
            {
                if (ms.LeftButton == ButtonState.Pressed && weaponCooldown == 0 && reloading == false)
                {
                    if (currentWeapon == currentPrimary)
                    {
                        if (primaryMagazine > 0)
                        {
                            primaryMagazine--;
                            shot.Play(0.1f, 0, 0);
                            shots.AddRange(currentWeapon.CreateBullet(bullet, position, rot));
                            weaponCooldown = currentWeapon.fireRate;
                        }
                        else
                        {
                            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton != ms.LeftButton)
                            {
                                click.Play();
                            }
                        }
                    }
                    else
                    {
                        if (secondaryMagazine > 0)
                        {
                            secondaryMagazine--;
                            shot.Play(0.1f, 0, 0);
                            shots.AddRange(currentWeapon.CreateBullet(bullet, position, rot));
                            weaponCooldown = currentWeapon.fireRate;
                        }
                        else
                        {
                            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton != ms.LeftButton)
                            {
                                click.Play();
                            }
                        }
                    }
                }
                else
                {
                    if (weaponCooldown != 0)
                    {
                        weaponCooldown--;
                    }
                    
                }
            }
            else
            {
                if (ms.LeftButton == ButtonState.Pressed && weaponCooldown == 0 && previousMs.LeftButton != ms.LeftButton && reloading == false)
                {
                    if (currentWeapon == currentPrimary)
                    {
                        if (primaryMagazine > 0)
                        {
                            primaryMagazine--;
                            shot.Play(0.05f, 0, 0);
                            shots.AddRange(currentWeapon.CreateBullet(bullet, position, rot));
                            weaponCooldown = currentWeapon.fireRate;
                        }
                        else
                        {
                            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton != ms.LeftButton)
                            {
                                click.Play();
                            }
                        }
                    }
                    else
                    {
                        if (secondaryMagazine > 0)
                        {
                            secondaryMagazine--;
                            shot.Play(0.05f, 0, 0);
                            shots.AddRange(currentWeapon.CreateBullet(bullet, position, rot));
                            weaponCooldown = currentWeapon.fireRate;
                        }
                        else
                        {
                            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton != ms.LeftButton)
                            {
                                click.Play();
                            }
                        }
                    }
                    
                }
                else
                {
                    if (weaponCooldown != 0)
                    {
                        weaponCooldown--;
                    }

                }
            }
            
            if (kb.IsKeyDown(Keys.Q) && switchTimer == 0 && reloading == false)
            {
                
                if (currentWeapon == currentPrimary)
                {
                    switchTimer = 15;
                    currentWeapon = currentSecondary;
                }
                else
                {
                    switchTimer = 15;
                    currentWeapon = currentPrimary;
                }
            }
            else
            {
                if (switchTimer != 0)
                {
                    switchTimer--;
                }
                
            }

            if (kb.IsKeyDown(Keys.R) && currentWeapon == currentPrimary && primaryMagazine != currentPrimary.ammoMagazine && reloading == false)
            {
                
                int temp = currentPrimary.ammoMagazine - primaryMagazine;
                if (primaryTotalAmmo >= temp)
                {
                    primaryReloadSound.Play();
                    primaryMagazine += temp;
                    primaryTotalAmmo -= temp;
                    reloading = true;
                }
                else if(primaryTotalAmmo != 0)
                {
                    primaryReloadSound.Play();
                    primaryMagazine += primaryTotalAmmo;
                    primaryTotalAmmo -= primaryTotalAmmo;
                    reloading = true;
                }
            }
            else if(kb.IsKeyDown(Keys.R) && currentWeapon == currentSecondary && secondaryMagazine != currentSecondary.ammoMagazine && reloading == false)
            {
                int temp = currentSecondary.ammoMagazine - secondaryMagazine;
                if (secondaryTotalAmmo >= temp)
                {
                    secondaryReloadSound.Play();
                    secondaryMagazine += temp;
                    secondaryTotalAmmo -= temp;
                    reloading = true;
                }
                else if (secondaryTotalAmmo != 0)
                {
                    secondaryReloadSound.Play();
                    secondaryMagazine += secondaryTotalAmmo;
                    secondaryTotalAmmo -= secondaryTotalAmmo;
                    reloading = true;
                }
            }


            foreach (var bullet in shots)
            {
                bullet.position += bullet.speed;
                bullet.hitbox.X = (int)bullet.position.X - bullet.texture.Width / 2;
                bullet.hitbox.Y = (int)bullet.position.Y - bullet.texture.Height / 2;
            }

            for (int e = 0; e < zombieManager.zombies.Count; e++)
            {
                for (int i = 0; i < shots.Count; i++)
                {
                    if (shots.Count > 0 && zombieManager.zombies[e].hitbox.Contains(shots[i].position))
                    {
                        if (shots[i].penetration > 0)
                        {
                            hit.Play();
                            shots[i].penetration--;
                            zombieManager.zombies[e].health -= shots[i].damage;
                            if (zombieManager.zombies[e].health <= 0)
                            {
                                dying.Play();
                                if (zombieManager._activeSounds.ContainsKey(zombieManager.zombies[e]))
                                {
                                    zombieManager._activeSounds[zombieManager.zombies[e]].Stop();
                                }
                                zombieManager.zombies.RemoveAt(e);
                            }
                        }
                        else
                        {
                            shots.Remove(shots[i]);
                        }
                        break;
                    }
                }
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

            if (kb.IsKeyDown(Keys.Enter))
            {
                zombieManager.KillAllZombies();
                //zombieManager.KillOneZombie();
            }

            if (kb.IsKeyDown(Keys.D1))
            {
                currentWeapon = currentPrimary;
            }

            if (kb.IsKeyDown(Keys.D2))
            {
                currentWeapon = currentSecondary;
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
            spriteBatch.Draw(currentWeapon.texture, position, null, color: Color.White, rotation: rot, origin: new Vector2(currentWeapon.texture.Bounds.Center.X, currentWeapon.texture.Bounds.Center.Y));
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