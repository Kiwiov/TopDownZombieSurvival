using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieSurvival
{
    class Bullet
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 speed;
        public float rotation;
        public Rectangle hitbox;
        public float damage;
        public int penetration;
        public Bullet(Texture2D Texture, Vector2 Position, Vector2 Speed, float Rotation, float Damage, int Penetration)
        {
            texture = Texture;
            position = Position;
            speed = Speed;
            rotation = Rotation;
            hitbox = new Rectangle((int)position.X - texture.Width / 2,(int)position.Y - texture.Height / 2, 36, 36);
            damage = Damage;
            penetration = Penetration;
        }
        
    }
}
