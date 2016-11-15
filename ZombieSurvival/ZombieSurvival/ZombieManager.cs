using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
namespace ZombieSurvival
{
    class ZombieManager
    {
        public int round = 0;
        Random ran = new Random();
        public List<Zombie> zombies = new List<Zombie>();
        public int zombiesToSpawn;
        PathFinder finder;
        Vector2 dPos;

        Texture2D zombieTextureStandingOne;
        Texture2D zombieTextureAttackingOne;
        Texture2D zombieTextureStandingTwo;
        Texture2D zombieTextureAttackingTwo;

        TileEngineGood tileEngine;

        Vector2 spawnPoint;
        List<Rectangle> spawnpoints = new List<Rectangle>();
        List<SoundEffect> groans = new List<SoundEffect>();

        public Dictionary<Zombie, SoundEffectInstance> _activeSounds = new Dictionary<Zombie, SoundEffectInstance>();

        public ZombieManager(Texture2D ZombieTextureStandingOne, Texture2D ZombieTextureAttackingOne, Texture2D ZombieTextureStandingTwo, Texture2D ZombieTextureAttackingTwo, TileEngineGood TileEngine, SoundEffect Groan1, SoundEffect Groan2, SoundEffect Groan3, SoundEffect Groan4)
        {
            zombieTextureStandingOne = ZombieTextureStandingOne;
            zombieTextureAttackingOne = ZombieTextureAttackingOne;
            zombieTextureStandingTwo = ZombieTextureStandingTwo;
            zombieTextureAttackingTwo = ZombieTextureAttackingTwo;
            tileEngine = TileEngine;
            groans.Add(Groan1);
            groans.Add(Groan2);
            groans.Add(Groan3);
            groans.Add(Groan4);
        }

        public void MoveZombie(PathFinder finder, Vector2 character)
        {
            foreach (var zomb in zombies)
            {
                if (zomb.delay == 20)
                {
                    zomb.delay = 0;
                    finder.SetStart((int)(zomb.position.X / 64), (int)(zomb.position.Y / 64));
                    finder.SetStop((int)(character.X / 64), (int)(character.Y / 64));
                    finder.MoveFromTo();
                    zomb.nodes = finder.PlotRoute();
                }
                else
                {
                    zomb.delay++;
                }
                if (zomb.nodes == null || zomb.nodes.Count == 0)
                {
                    continue;
                }

                if (zomb.nodes[0].X * 64 + 32 > zomb.position.X && zomb.nodes[0].Y * 64 + 32 < zomb.position.Y)
                {
                    zomb.position.X += 2;
                    zomb.position.Y -= 2;
                    zomb.rotation = 45;
                }
                else if (zomb.nodes[0].X * 64 + 32 < zomb.position.X && zomb.nodes[0].Y * 64 + 32 < zomb.position.Y)
                {
                    zomb.position.X -= 2;
                    zomb.position.Y -= 2;
                    zomb.rotation = 315;
                }
                else if (zomb.nodes[0].X * 64 + 32 > zomb.position.X && zomb.nodes[0].Y * 64 + 32 > zomb.position.Y)
                {
                    zomb.position.X += 2;
                    zomb.position.Y += 2;
                    zomb.rotation = 135;
                }
                else if (zomb.nodes[0].X * 64 + 32 < zomb.position.X && zomb.nodes[0].Y * 64 + 32 > zomb.position.Y)
                {
                    zomb.position.X -= 2;
                    zomb.position.Y += 2;
                    zomb.rotation = 225;
                }
                else if (zomb.nodes[0].X * 64 + 32 > zomb.position.X)
                {
                    zomb.position.X += 3;
                    zomb.rotation = 90;
                }

                else if (zomb.nodes[0].X * 64 + 32 < zomb.position.X)
                {
                    zomb.position.X -= 3;
                    zomb.rotation = 270;
                }

                else if (zomb.nodes[0].Y * 64 + 32 > zomb.position.Y)
                {
                    zomb.position.Y += 3;
                    zomb.rotation = 180;
                }

                else if (zomb.nodes[0].Y * 64 + 32 < zomb.position.Y)
                {
                    zomb.position.Y -= 3;
                    zomb.rotation = 0;
                }
                dPos = character - zomb.position;
                zomb.rotation = (float)Math.Atan2(dPos.Y, dPos.X);
                zomb.hitbox.X = (int)zomb.position.X - zomb.texture.Width / 2;
                zomb.hitbox.Y = (int)zomb.position.Y - zomb.texture.Height / 2;
            }
        }

        public void SpawnZombies()
        {
            GetSpawningTiles();

            if (zombies.Count < 500 && zombiesToSpawn > 0)
            {
                zombiesToSpawn--;

                int random = ran.Next(0, spawnpoints.Count);
                spawnPoint = new Vector2(spawnpoints[random].X + 32, spawnpoints[random].Y + 32);

                if (ran.Next(1, 3) == 2)
                {
                    zombies.Add(new Zombie(zombieTextureStandingOne, zombieTextureAttackingOne, spawnPoint, new Vector2(0, 0), 0, groans));
                }
                else
                {
                    zombies.Add(new Zombie(zombieTextureStandingTwo, zombieTextureAttackingTwo, spawnPoint, new Vector2(0, 0), 0, groans));
                }
            }
        }


        public void KillAllZombies()
        {
            zombies.Clear();
        }

        public void KillOneZombie()
        {
            zombies.RemoveAt(0);
        }

        private void GetSpawningTiles()
        {
            foreach (var spawn in tileEngine.spawnZones)
            {
                spawnpoints.Add(spawn);
            }
        }

        public void CalculateZombies()
        {
            zombiesToSpawn = (int)(350 - 350 * Math.Pow(2, -0.008 * round));
        }

        public bool ToAttackOrNotToAttack(Vector2 position)
        {
            foreach (var zomb in zombies)
            {
                if (zomb.attackCoolDown == 0)
                {
                    if ((position - zomb.position).Length() < 32)
                    {
                        zomb.attackCoolDown = 1 * 60;
                        zomb.attacking = true;
                        return true;
                    }
                    else
                    {
                        zomb.attacking = false;
                        return false;
                    }
                }
                else
                {
                    zomb.attackCoolDown--;
                }
                
            }
            return false;
        }
        public void ToGroanOrNotToGroan()
        {
            foreach (var zomb in zombies)
            {
                int rando = ran.Next(1000);
                SoundEffectInstance i = null;

                switch (rando)
                {
                    case 1:
                        i = zomb.groans[0].CreateInstance();
                        break;
                    case 2:
                        i = zomb.groans[1].CreateInstance();
                        break;
                    case 3:
                        i = zomb.groans[2].CreateInstance();
                        break;
                    case 4:
                        i = zomb.groans[3].CreateInstance();
                        break;
                }

                if (i != null)
                {
                    if (!_activeSounds.ContainsKey(zomb))
                        _activeSounds.Add(zomb, null);

                    if (_activeSounds[zomb] == null || _activeSounds[zomb].State != SoundState.Playing)
                    {
                        i.Play();
                        _activeSounds[zomb] = i;
                    }
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var zomb in zombies)
            {
                if (zomb.attacking)
                {
                    spriteBatch.Draw(zomb.textureAttack, zomb.position, null, color: Color.White, rotation: zomb.rotation, origin: new Vector2(zomb.textureAttack.Width / 2, zomb.textureAttack.Height / 2));
                }
                else
                {
                    spriteBatch.Draw(zomb.texture, zomb.position, null, color: Color.White, rotation: zomb.rotation, origin: new Vector2(zomb.texture.Width / 2, zomb.texture.Height / 2));
                }
                
            }
        }
    }
}
