using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvival
{
    class RandomFireAngle
    {
        static Random r = new Random();
        public static float Angle()
        {
            return Angle(1);
        }
        public static float Angle(float Multiplyer)
        {
            return ((float)r.NextDouble() * Multiplyer - Multiplyer * 0.5f);
        }
    }
}