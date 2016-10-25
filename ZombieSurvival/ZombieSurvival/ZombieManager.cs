using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZombieSurvival
{
    class ZombieManager
    {
        public int round = 1;
        Random ran = new Random();
        public List<Zombie> zombies = new List<Zombie>();
        PathFinder finder;

        Texture2D zombieTextureStandingOne;
        Texture2D zombieTextureAttackingOne;
        Texture2D zombieTextureStandingTwo;
        Texture2D zombieTextureAttackingTwo;

        TileEngineGood tileEngine;
        
        Vector2 spawnPoint = new Vector2();
        List<Rectangle> spawnpoints = new List<Rectangle>();
        public ZombieManager(Texture2D ZombieTextureStandingOne, Texture2D ZombieTextureAttackingOne, Texture2D ZombieTextureStandingTwo, Texture2D ZombieTextureAttackingTwo, TileEngineGood TileEngine)
        {
            zombieTextureStandingOne = ZombieTextureStandingOne;
            zombieTextureAttackingOne = ZombieTextureAttackingOne;
            zombieTextureStandingTwo = ZombieTextureStandingTwo;
            zombieTextureAttackingTwo = ZombieTextureAttackingTwo;
            tileEngine = TileEngine;
        }

        public void MoveZombie(PathFinder finder, Vector2 character)
        {
            foreach (var zomb in zombies)
            {
                finder.SetStart((int)(zomb.position.X / 64), (int)(zomb.position.Y / 64));
                finder.SetStop((int)(character.X / 64), (int)(character.Y / 64));
                finder.MoveFromTo();
                var list = finder.PlotRoute();

                if(list == null || list.Count == 0)
                    continue;

                if ((list[0].X + 32) * 64 > zomb.position.X)
                {
                    zomb.position.X += 3;
                }
                else if ((list[0].X + 32) *64 < zomb.position.X)
                {
                    zomb.position.X -= 3;
                }
                if ((list[0].Y + 32) *64 > zomb.position.Y)
                {
                    zomb.position.Y += 3;
                }
                else if ((list[0].Y + 32) *64 < zomb.position.Y)
                {
                    zomb.position.Y -= 3;
                }
                
            }
        }

        public void SpawnZombies()
        {
            GetSpawningTiles();
            CalculateZombies();
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
            for (int i = 0; i < round * 2; i++)
            {
                int random = ran.Next(0, spawnpoints.Count);
                    spawnPoint = new Vector2(spawnpoints[random].X + 32, spawnpoints[random].Y + 32);
                
                if (ran.Next(1, 3) == 2)
                {
                    zombies.Add(new Zombie(zombieTextureStandingOne,zombieTextureAttackingOne,spawnPoint,new Vector2(0,0),0));
                }
                else
                {
                    zombies.Add(new Zombie(zombieTextureStandingTwo, zombieTextureAttackingTwo,spawnPoint, new Vector2(0, 0), 0));
                }
                
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var zomb in zombies)
            {
                spriteBatch.Draw(zomb.texture, zomb.position, null, color: Color.White, rotation: zomb.rotation, origin: new Vector2(zomb.texture.Width/2, zomb.texture.Height/2));
            }
        }
    }
}
