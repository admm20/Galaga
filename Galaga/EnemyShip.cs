using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    class EnemyShip : RotatingShip
    {
        public bool MovingOnCurve = false;
        public bool MovingOnGrid = false;
        public Point PositionOnGrid;

        private List<Point> currentPath;
        private int currentSection;
        private float traveledDistanceInSection;
        private float speed;

        private bool landing = false;

        public void BeginMovingOnCurve(List<Point> bezierCurvePoints, float speed)
        {
            // just in case someone put only 1 point (it should be min. 2)
            if (bezierCurvePoints.Count < 2)
                return; 

            MovingOnCurve = true;
            MovingOnGrid = false;
            currentPath = bezierCurvePoints;
            currentSection = 1;
            traveledDistanceInSection = 0;
            this.speed = speed;
            landing = false;

            Position.X = bezierCurvePoints[0].X;
            Position.Y = bezierCurvePoints[0].Y;
        }

        public void LandOnGrid()
        {
            currentPath.Add(new Point(
                (int)(EnemyGrid.Position.X + PositionOnGrid.X * (16 + EnemyGrid.gap) * RotatingShip.Scale),
                (int)(EnemyGrid.Position.Y + PositionOnGrid.Y * (16 + EnemyGrid.gap) * RotatingShip.Scale))
                );
        }

        public static void UpdateEnemyPosition(int deltaTime, GameMode game)
        {
            foreach(RotatingShip ship in ListOfShips)
            {
                if(ship.Type != ShipTypeEnum.PLAYER)
                {
                    EnemyShip enemy = (EnemyShip)ship;
                    if (!enemy.MovingOnCurve)
                        continue;

                    if (enemy.landing)
                    {
                        //enemy.currentPath[enemy.currentPath.Count - 1].X = (int)EnemyGrid.Position.X; // doesn't work because Point is struct
                        Point p = enemy.currentPath[enemy.currentPath.Count - 1];
                        p.X = (int)(EnemyGrid.Position.X + enemy.PositionOnGrid.X * (16 + EnemyGrid.gap) * RotatingShip.Scale);
                        enemy.currentPath[enemy.currentPath.Count - 1] = p;
                    }

                    Point src = enemy.currentPath[enemy.currentSection - 1];
                    Point dst = enemy.currentPath[enemy.currentSection];

                    Vector2 moveVector = new Vector2(dst.X - src.X, dst.Y - src.Y);
                    enemy.SetDirection(moveVector);

                    Vector2 normalized = Vector2.Normalize(moveVector);

                    enemy.Position.X += normalized.X * enemy.speed * deltaTime;
                    enemy.Position.Y += normalized.Y * enemy.speed * deltaTime;

                    enemy.traveledDistanceInSection = Vector2.Distance(
                        enemy.Position,
                        new Vector2(src.X, src.Y));
                    
                    if(enemy.traveledDistanceInSection > moveVector.Length())
                    {
                        enemy.currentSection++;
                        enemy.traveledDistanceInSection = 0;
                        if (enemy.currentSection > enemy.currentPath.Count-1 && !enemy.landing)
                        {
                            // end of road. Start landing procedure
                            enemy.LandOnGrid();
                            enemy.landing = true;
                            //enemy.speed = 1.0f;
                        }
                        else if (enemy.landing)
                        {
                            enemy.SetDirection(new Vector2(0, -1));
                            enemy.landing = false;
                            enemy.MovingOnCurve = false;
                            enemy.MovingOnGrid = true;
                            enemy.Position.X = (int)(EnemyGrid.Position.X + enemy.PositionOnGrid.X * (16 + EnemyGrid.gap) * RotatingShip.Scale);
                            enemy.Position.Y = (int)(EnemyGrid.Position.Y + enemy.PositionOnGrid.Y * (16 + EnemyGrid.gap) * RotatingShip.Scale);
                        }
                    }


                }
            }
        }

        public EnemyShip(float x, float y, ShipTypeEnum type, float angle) : base(x, y, type, angle)
        {
            PositionOnGrid = new Point(-1, -1);
        }
    }
}
