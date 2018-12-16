using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga
{
    public enum GameEventEnum
    {
        ENEMY_DESTROYED,
        PLAYER_DESTROYED,
        PLAYER_FIRE,
        GAME_OVER
    }
    class GameMode : GameState
    {
        GalagaGame game;
        
        PlayerShip playerShip;
        EnemyGrid enemyGrid;

        Random random;

        Texture2D numbersTexture;

        private int Lifes = 3;
        private int Points = 0;

        public void GameEvent(GameEventEnum ev)
        {
            switch (ev)
            {
                case GameEventEnum.ENEMY_DESTROYED:
                    Points += 10;
                    if(RotatingShip.ListOfShips.Count < 2)
                    {
                        SpawnAllEnemies();
                    }
                    break;
                case GameEventEnum.PLAYER_DESTROYED:
                    Lifes--;
                    if (Lifes < 1)
                        GameEvent(GameEventEnum.GAME_OVER);
                    else
                    {
                        Timer respawn = new Timer((t) => {
                            playerShip = new PlayerShip(GalagaGame.GAME_WIDTH / 2, GalagaGame.GAME_HEIGHT - 150);
                        },
                        500, false);
                    }
                    break;
                case GameEventEnum.PLAYER_FIRE:
                    break;
                case GameEventEnum.GAME_OVER:
#if WINDOWS
                    game.KeyboardKeysDown -= KeysHoldDown;
                    game.KeyboardKeyClicked -= KeyClicked;
#endif
                    game.RunMenu();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            foreach (RotatingShip s in RotatingShip.ListOfShips) s.Draw(spriteBatch);
            foreach (Bullet b in Bullet.ListOfBullets) b.Draw(spriteBatch);

            // draw points
            string score = Points.ToString();
            for(int i = 0; i < score.Length; i++)
            {
                int digit = Int32.Parse(score[i].ToString());
                int width = numbersTexture.Width / 10 + 1;
                spriteBatch.Draw(numbersTexture, new Vector2(200 + i * 40, 50), new Rectangle(digit * width, 0, width, 30), Color.White);
            }

            // draw lifes
            string lifes = Lifes.ToString();
            for (int i = 0; i < lifes.Length; i++)
            {
                int digit = Int32.Parse(lifes[i].ToString());
                int width = numbersTexture.Width / 10 + 1;
                spriteBatch.Draw(numbersTexture, new Vector2(GalagaGame.GAME_WIDTH - 100 + i * 40, 50), new Rectangle(digit * width, 0, width, 30), Color.White);
            }

            spriteBatch.End();
        }

        public override void LoadContent(ContentManager content)
        {
            RotatingShip.EnemyAndPlayerTexture = content.Load<Texture2D>("Textures/enemies_player");
            Bullet.BulletTexture = content.Load<Texture2D>("Textures/bullet");
            numbersTexture = content.Load<Texture2D>("Textures/numbers");
        }

        private void SpawnAllEnemies()
        {
            for (int i = 0; i < 4; i++)
            {
                List<Point> bezierMove = QuadraticBezier.CalculatePoints(
                    new Point(-200 + (i * 600), -100),
                    new Point(GalagaGame.GAME_WIDTH / 2, GalagaGame.GAME_HEIGHT),
                    new Point(GalagaGame.GAME_WIDTH / 2, 700),
                    30);

                for (int j = 0; j < 6; j++)
                {
                    int tempJ = j;
                    int tempI = i;
                    Timer firstRowTimer = new Timer((t) =>
                    {
                        EnemyShip en = new EnemyShip(-200 + (i * 600), -100, (ShipTypeEnum)(tempI+2), 0);
                        en.PositionOnGrid.X = tempJ;
                        en.PositionOnGrid.Y = tempI;
                        List<Point> moveWithLanding = new List<Point>(bezierMove);

                        en.BeginMovingOnCurve(moveWithLanding, 1.0f);

                    }, 200 * j + 1500*i, false);
                }



            }
        }

        public override void OnEnter()
        {
            EnemyShip.ListOfShips.Clear();
            Bullet.ListOfBullets.Clear();

            Lifes = 3;
            Points = 0;
#if WINDOWS
            game.KeyboardKeysDown += KeysHoldDown;
            game.KeyboardKeyClicked += KeyClicked;
#endif

            playerShip = new PlayerShip(GalagaGame.GAME_WIDTH / 2, GalagaGame.GAME_HEIGHT - 150);
            
            enemyGrid = new EnemyGrid(new Vector2(100, 200));

            SpawnAllEnemies();

            Timer gridMovingTimer = new Timer((t) => {
                enemyGrid.UpdateEnemyGridPosition(game.deltaTime);}, 1000, true);
            
        }

        public override void TransitionEffectCompleted()
        {

        }

        public override void TransitionEffectHalfCompleted()
        {

        }

        public override void Update(int deltaTime)
        {
            Bullet.UpdateBulletsPosition(deltaTime, this);
            EnemyShip.UpdateEnemyPosition(deltaTime);

            if(random.Next() % 30 == 5)
            {
                foreach(RotatingShip ship in RotatingShip.ListOfShips)
                {
                    if(ship.Type != ShipTypeEnum.PLAYER)
                    {
                        if(random.Next() % 50 == 6)
                        {
                            Bullet b = new Bullet(ship.Hitbox.Center.X - 2 * RotatingShip.Scale, ship.Position.Y, BulletType.ENEMY);
                        }
                    }
                }
            }
            
        }

        void KeysHoldDown(object s, EventArgs _k)
        {
            KeyboardKeysDownEventArgs k = (KeyboardKeysDownEventArgs)_k;
            foreach (Keys key in k.keys)
            {
                if (key == Keys.Left)
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
            if (k.key == Keys.Space)
            {
                Bullet b = new Bullet(playerShip.Hitbox.Center.X - 2 * RotatingShip.Scale, playerShip.Position.Y, BulletType.ALLY);
                GameEvent(GameEventEnum.PLAYER_FIRE);
            }

        }

        public GameMode(GalagaGame game)
        {
            this.game = game;
            random = new Random();
        }
    }
}
