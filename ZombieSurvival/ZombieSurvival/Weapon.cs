using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieSurvival
{
    class Weapon
    {
        private float damage;
        public int numberOfShots;
        private string name;
        public int ammoTotal;
        public int ammoMagazine;
        public int reloadSpeed;
        public int fireRate;
        private float spread;
        private string type;
        public Texture2D texture;
        public Texture2D textureReload;
        private int penetration;
        public bool automatic;

        
        public Weapon(float Damage, int NumberOfShots, string Name, int AmmoTotal, int AmmoMagazine, int ReloadSpeed, int FireRate, float Spread, string Type, Texture2D Texture, Texture2D TextureReload, int Penetration, bool Automatic)
        {
            damage = Damage;
            numberOfShots = NumberOfShots;
            name = Name;
            ammoTotal = AmmoTotal;
            ammoMagazine = AmmoMagazine;
            reloadSpeed = ReloadSpeed;
            fireRate = FireRate;
            spread = Spread;
            type = Type;
            texture = Texture;
            textureReload = TextureReload;
            penetration = Penetration;
            automatic = Automatic;
        }

        public List<Bullet> CreateBullet(Texture2D texture, Vector2 position, float rotation)
        {
            List<Bullet> bullets = new List<Bullet>();

            for (int i = 0; i < numberOfShots; i++)
            {
                float temp = rotation + RandomFireAngle.Angle(spread);
                bullets.Add(new Bullet(texture, position, 30* new Vector2( (float)Math.Cos(temp), (float)Math.Sin(temp)), temp, damage, penetration));
            }
            
            return bullets;
        }
    }
}
