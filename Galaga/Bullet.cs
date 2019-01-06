using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    public enum BulletType
    {
        ALLY,
        ENEMY
    }

    class Bullet
    {
        public static List<Bullet> ListOfBullets = new List<Bullet>();
        public static Texture2D BulletTexture;
        
        public const int Scale = 6;
        public BulletType Type;

        public Vector2 Position;
        public Rectangle Hitbox = new Rectangle(0, 0, 3 * Scale, 8 * Scale);

        public static void UpdateBulletsPosition(int deltaTime, GameMode game)
        {
            for(int i = ListOfBullets.Count-1; i >=0; i--)
            {
                Bullet bullet = ListOfBullets[i];

                // check collisions
                bool hit = false;
                for(int j = RotatingShip.ListOfShips.Count-1; j >= 0; j--)
                {
                    RotatingShip ship = RotatingShip.ListOfShips[j];
                    if(bullet.Type == BulletType.ALLY 
                        && bullet.Hitbox.Intersects(ship.Hitbox) 
                        && ship.Type != ShipTypeEnum.PLAYER)
                    {
                        // bullet hit enemy
                        RotatingShip.ListOfShips.RemoveAt(j);
                        hit = true;
                        game.GameEvent(GameEventEnum.ENEMY_DESTROYED);
                        game.SpawnExplosion(ship.Position.X, ship.Position.Y);
                        break;
                    }
                    else if(bullet.Type == BulletType.ENEMY
                        && bullet.Hitbox.Intersects(ship.Hitbox)
                        && ship.Type == ShipTypeEnum.PLAYER)
                    {
                        RotatingShip.ListOfShips.RemoveAt(j);
                        hit = true;
                        game.GameEvent(GameEventEnum.PLAYER_DESTROYED);
                        game.SpawnExplosion(ship.Position.X, ship.Position.Y);
                        break;
                    }
                }

                if (hit)
                {
                    ListOfBullets.RemoveAt(i);
                    continue;
                }

                // move bullet
                if (bullet.Type == BulletType.ALLY)
                {
                    bullet.Position.Y -= deltaTime * 1.5f;
                }
                else
                {
                    bullet.Position.Y += deltaTime * 1.5f;
                }

                bullet.Hitbox.Y = (int)bullet.Position.Y;

                // remove if it's outside screen
                if (bullet.Position.Y < -40 || bullet.Position.Y > GalagaGame.GAME_HEIGHT + 40)
                {
                    ListOfBullets.RemoveAt(i);
                    continue;
                }
            }
        }

        public Bullet(float x, float y, BulletType type)
        {
            Type = type;
            Position = new Vector2(x, y);
            Hitbox.X = (int)x;
            Hitbox.Y = (int)y;
            ListOfBullets.Add(this);
        }

        public void Draw(SpriteBatch sp)
        {
            if(Type == BulletType.ALLY)
                sp.Draw(BulletTexture, Position, new Rectangle(0,0,3,8), Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            else
                sp.Draw(BulletTexture, Position, new Rectangle(4,0,3,8), Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
