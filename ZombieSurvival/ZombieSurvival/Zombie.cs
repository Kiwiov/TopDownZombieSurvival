using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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
            
        public Zombie(Texture2D Texture, Texture2D TextureAttack, Vector2 Position, Vector2 Speed, float Rotation)
        {
            texture = Texture;
            textureAttack = TextureAttack;
            position = Position;
            speed = Speed;
            rotation = Rotation;
        }
    }
}
