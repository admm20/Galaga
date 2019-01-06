using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    public class Explosion
    {

        public int currentFrame;
        public float x, y;

        public const int ShiftTime = 80; // in ms

        // spawn an explosion in x and y
        public Explosion(float x, float y)
        {
            currentFrame = 0;
            this.x = x;
            this.y = y;
        }
    }
}
