using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    class PlayerShip : RotatingShip
    {

        // Move player to x
        public void MovePlayer(float x)
        {
            if (x < 0)
                x = 0;
            else if (x > GalagaGame.GAME_WIDTH - 16*Scale)
                x = GalagaGame.GAME_WIDTH - 16*Scale;

            Position.X = x;
        }
        

        public PlayerShip(float x, float y) : base(x, y, ShipTypeEnum.PLAYER, (float)Math.PI / -2.0f)
        {

        }
    }
}
