using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieSurvival
{
    public class Zombie
    {
        public Texture2D texture;
        public Texture2D textureAttack;
        public Vector2 position;
        public Vector2 speed;
        public float rotation;
        public float delay;
        public List<Astar.Node> nodes;
        public Vector2 range;
        public float health;
        public Rectangle hitbox;
        public bool attacking = false;
        public float attackCoolDown;
        public float damage = /*100*/ 33.5f;

        public List<SoundEffect> groans;

        public Zombie(Texture2D Texture, Texture2D TextureAttack, Vector2 Position, Vector2 Speed, float Rotation, List<SoundEffect> Groans, float Health)
        {
            texture = Texture;
            textureAttack = TextureAttack;
            position = Position;
            speed = Speed;
            rotation = Rotation;
            hitbox = new Rectangle((int)position.X - texture.Width / 2,(int)position.Y - texture.Height / 2,43,43);
            groans = Groans;
            health = Health;
        }
    }
}
