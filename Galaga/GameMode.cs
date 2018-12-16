using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga
{
    class GameMode : GameState
    {
        GalagaGame game;

        EnemyShip enemyShip;
        PlayerShip playerShip;

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            foreach (RotatingShip s in RotatingShip.ListOfShips) s.Draw(spriteBatch);
            foreach (Bullet b in Bullet.ListOfBullets) b.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void LoadContent(ContentManager content)
        {
            RotatingShip.EnemyAndPlayerTexture = content.Load<Texture2D>("Textures/enemies_player");
            Bullet.BulletTexture = content.Load<Texture2D>("Textures/bullet");
        }

        public override void OnEnter()
        {
            game.KeyboardKeysDown += KeysHoldDown;
            game.KeyboardKeyClicked += KeyClicked;

            playerShip = new PlayerShip(GalagaGame.GAME_WIDTH / 2, GalagaGame.GAME_HEIGHT - 150);
            enemyShip = new EnemyShip(200, 200, ShipTypeEnum.BLUE_BAT, 0);

            List<Point> tst = QuadraticBezier.CalculatePoints(
                new Point(100, 100), 
                new Point(200, 100), 
                new Point(200, 700), 
                20);

            foreach(Point p in tst)
            {
                Console.WriteLine(p.X + " " + p.Y);
            }

        }

        public override void TransitionEffectCompleted()
        {

        }

        public override void TransitionEffectHalfCompleted()
        {

        }

        public override void Update(int deltaTime)
        {
            Bullet.UpdateBulletsPosition(deltaTime);
        }

        void KeysHoldDown(object s, EventArgs _k)
        {
            KeyboardKeysDownEventArgs k = (KeyboardKeysDownEventArgs)_k;
            foreach(Keys key in k.keys)
            {
                if(key == Keys.Left)
                {
                    playerShip.MovePlayer(playerShip.Position.X - game.deltaTime * 0.8f);
                }
                if (key == Keys.Right)
                {
                    playerShip.MovePlayer(playerShip.Position.X + game.deltaTime * 0.8f);

                }
            }
        }

        void KeyClicked(object s, EventArgs _k)
        {
            KeyboardKeyClickedEventArgs k = (KeyboardKeyClickedEventArgs)_k;
            if(k.key == Keys.Space)
            {
                Bullet b = new Bullet(playerShip.Hitbox.Center.X - 2*RotatingShip.Scale, playerShip.Position.Y, BulletType.ALLY);
            }

        }

        public GameMode(GalagaGame game)
        {
            this.game = game;
        }
    }
}
