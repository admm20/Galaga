using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    class EnemyShip : RotatingShip
    {
        public bool MovingOnCurve = false;
        private List<Point> currentPath;
        private int currentSection;
        private float speed;

        public void BeginMovingOnCurve(List<Point> bezierCurvePoints, float speed)
        {
            MovingOnCurve = true;
            currentPath = bezierCurvePoints;
            currentSection = 0;
            this.speed = speed;
        }

        public static void UpdateEnemyPosition()
        {
            foreach(RotatingShip ship in ListOfShips)
            {
                if(ship.Type != ShipTypeEnum.PLAYER)
                {


                }
            }
        }

        public EnemyShip(float x, float y, ShipTypeEnum type, float angle) : base(x, y, type, angle)
        {

            ListOfShips.Add(this);
        }
    }
}
