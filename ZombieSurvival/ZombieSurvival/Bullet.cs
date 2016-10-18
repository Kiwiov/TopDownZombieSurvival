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
        public Bullet(Texture2D Texture, Vector2 Position, Vector2 Speed, float Rotation)
        {
            texture = Texture;
            position = Position;
            speed = Speed;
            rotation = Rotation;
        }
        
    }
}
