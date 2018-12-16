using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{

    class EnemyGrid
    {
        public static Vector2 Position;
        public float Speed;

        public const int COLS = 6, ROWS = 4;

        // gap between enemies in px
        public const int gap = 7;

        // true - grid is moving to the left, false - moving to the right
        public bool movingRight;
        

        public EnemyGrid(Vector2 startingPosition)
        {
#if ANDROID
            gap = 3;
#endif
            Position = startingPosition;
            movingRight = true;
            Speed = 0.8f;
        }

        public void UpdateEnemyGridPosition(int deltaTime)
        {
            if (movingRight)
                Position.X += Speed * deltaTime;
            else
                Position.X -= Speed * deltaTime;

            

            if(Position.X < 50)
            {
                Position.X = 50;
                movingRight = true;
            }
            else if(Position.X > GalagaGame.GAME_WIDTH - COLS * (16 + gap) * RotatingShip.Scale - 20)
            {
                Position.X = GalagaGame.GAME_WIDTH - COLS * (16 + gap) * RotatingShip.Scale - 20;
                movingRight = false;
            }

            foreach(RotatingShip s in EnemyShip.ListOfShips)
            {
                if (s.Type == ShipTypeEnum.PLAYER)
                    continue;
                EnemyShip ship = (EnemyShip)s;
                if(ship.PositionOnGrid.X != -1 && ship.MovingOnGrid)
                {
                    ship.Position.X = Position.X + ship.PositionOnGrid.X * (16 + gap) * RotatingShip.Scale;
                    ship.Position.Y = Position.Y + ship.PositionOnGrid.Y * (16 + gap) * RotatingShip.Scale;
                }
            }

            //for (int row = 0; row < ROWS; row++)
            //{
            //    for (int col = 0; col < COLS; col++)
            //    {
            //        if (Grid[col, row] != null && Grid[col, row].MovingOnGrid)
            //        {
            //            Grid[col, row].Position.X = Position.X + col *(16+1) * RotatingShip.Scale;
            //            Grid[col, row].Position.Y = Position.Y + row * (16 + 1) * RotatingShip.Scale;
            //        }
            //    }
            //}
        }

        //public bool PutEnemy(int row, int col, EnemyShip ship)
        //{
        //    if (Grid[row, col] == null)
        //        Grid[row, col] = ship;
        //    else
        //        return false;
            
        //    return true;
        //}

        //public void RemoveEnemy(EnemyShip ship)
        //{
        //    for(int row = 0; row < ROWS; row++)
        //    {
        //        for(int col = 0; col < COLS; col++)
        //        {
        //            if(ship == Grid[col, row])
        //            {
        //                Grid[col, row] = null;
        //            }
        //        }
        //    }
        //}

        //public EnemyShip GetEnemy(int row, int col)
        //{
        //    EnemyShip ship = Grid[row, col];

        //    return ship;
        //}

    }
}
