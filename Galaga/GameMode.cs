using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        Texture2D fireTexture;
        Texture2D explosionTexture;

        SoundEffect destroy_enemy;
        SoundEffect explosion;
        SoundEffect shot;

        private List<Explosion> explosions;

        private int Lifes = 3;
        private int Points = 0;
        private bool playerIsDead;

        // every enemy on grid must be in the same animation frame
        // if enemyAnimation == 0, then show frame 6
        // if 1, then show frame 7
        private int enemyAnimation = 0;

        private Timer gridMovingTimer;

#if ANDROID
        private Timer shootingTimer;
#endif

        public void GameEvent(GameEventEnum ev)
        {
            switch (ev)
            {
                case GameEventEnum.ENEMY_DESTROYED:
                    destroy_enemy.Play(0.5f, 0, 0);
                    Points += 10;
                    if(RotatingShip.ListOfShips.Count < 2)
                    {
                        SpawnAllEnemies();
                    }
                    break;
                case GameEventEnum.PLAYER_DESTROYED:
                    playerIsDead = true;
                    explosion.Play(0.5f, 0, 0);
                    Lifes--;
                    if (Lifes < 1)
                        GameEvent(GameEventEnum.GAME_OVER);
                    else
                    {
                        Timer respawn = new Timer((t) => {
                            int playerShipYposition = GalagaGame.GAME_HEIGHT - 200;
#if ANDROID
                            playerShipYposition -= 200;
#endif
                            playerShip = new PlayerShip(GalagaGame.GAME_WIDTH / 2, playerShipYposition);
                            playerIsDead = false;
                        },
                        2000, false);
                    }
                    break;
                case GameEventEnum.PLAYER_FIRE:
                    shot.Play(0.5f, 0, 0);
                    break;
                case GameEventEnum.GAME_OVER:
                    gridMovingTimer.Delete();
                    Timer endDelay = new Timer((t) =>
                    {
                        ExitGameMode();
                    }, 2000, false);

                    break;
            }
        }

        public void ExitGameMode()
        {
#if WINDOWS
            game.KeyboardKeysDown -= KeysHoldDown;
            game.KeyboardKeyClicked -= KeyClicked;
#elif ANDROID
            game.ScreenTapped -= ScreenTapped;
            game.ScreenTouched -= ScreenTouched;
            shootingTimer.Delete();
#endif
            gridMovingTimer.Delete();
            //Timer.CreatedTimers.Clear();
            foreach(Timer t in Timer.CreatedTimers)
            {
                Timer.Delete(t);
            }
            game.RunMenu();
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

            // draw explosions
            foreach(Explosion e in explosions)
            {
                spriteBatch.Draw(explosionTexture, new Vector2(e.x, e.y), new Rectangle(31*e.currentFrame,0,31,32), 
                    Color.White, 0, Vector2.Zero, RotatingShip.Scale, SpriteEffects.None, 0);
            }

            // draw fire button in android version
//#if ANDROID
//            spriteBatch.Draw(fireTexture, new Vector2(700, 1300), Color.White*0.8f);
//#endif

            spriteBatch.End();
        }

        public override void LoadContent(ContentManager content)
        {
            RotatingShip.EnemyAndPlayerTexture = content.Load<Texture2D>("Textures/enemies_player");
            Bullet.BulletTexture = content.Load<Texture2D>("Textures/bullet");
            numbersTexture = content.Load<Texture2D>("Textures/numbers");
            fireTexture = content.Load<Texture2D>("Textures/fire_button");
            explosionTexture = content.Load<Texture2D>("Textures/explosion");

            //audio
            destroy_enemy = content.Load<SoundEffect>("Audio/destroy_enemy");
            explosion = content.Load<SoundEffect>("Audio/explosion");
            shot = content.Load<SoundEffect>("Audio/shot");
        }

        public void SpawnExplosion(float x, float y)
        {
            Explosion exp = new Explosion(x, y);
            explosions.Add(exp);
            Timer ti = null;
            ti = new Timer((elapsed) =>
            {
                exp.currentFrame++;
                if (exp.currentFrame > 4)
                {
                    ti.Delete();
                    explosions.Remove(exp);
                }
            }, Explosion.ShiftTime, true);
        }
        
        private void SpawnAllEnemies()
        {
            for (int i = 0; i < 4; i++)
            {
                int randX = random.Next(200, GalagaGame.GAME_WIDTH - 200);
                int randY = random.Next(500, GalagaGame.GAME_HEIGHT);
                List<Point> bezierMove = QuadraticBezier.CalculatePoints(
                    new Point(-200 + (i * 600), -100),
                    //new Point(GalagaGame.GAME_WIDTH / 2, GalagaGame.GAME_HEIGHT),
                    //new Point(randX, randY),
                    getRandomPointOnScreen(),
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
            explosions.Clear();

            Lifes = 3;
            Points = 0;
            playerIsDead = false;
            int playerShipYposition = GalagaGame.GAME_HEIGHT - 200;
#if WINDOWS
            game.KeyboardKeysDown += KeysHoldDown;
            game.KeyboardKeyClicked += KeyClicked;
#elif ANDROID
            playerShipYposition -= 200;
            game.ScreenTapped += ScreenTapped;
            game.ScreenTouched += ScreenTouched;
#endif

            playerShip = new PlayerShip(GalagaGame.GAME_WIDTH / 2, playerShipYposition);
            
            enemyGrid = new EnemyGrid(new Vector2(100, 200));

            SpawnAllEnemies();

            gridMovingTimer = new Timer((t) => {
                enemyGrid.UpdateEnemyGridPosition(game.deltaTime);
                enemyAnimation = (enemyAnimation == 0) ? 1 : 0;
                foreach(RotatingShip s in RotatingShip.ListOfShips)
                {
                    if (s is EnemyShip)
                    {
                        EnemyShip en = (EnemyShip)s;
                        if (en.MovingOnGrid)
                        {
                            en.SetTexture(6 + enemyAnimation);
                        }
                    }
                }
            }, 800, true);

            // shoot automatically every 0.7s in Android version
#if ANDROID
            shootingTimer = new Timer((el) => {
                if (!RotatingShip.ListOfShips.Contains(playerShip))
                    return;

                Bullet b = new Bullet(playerShip.Hitbox.Center.X - 2 * RotatingShip.Scale, playerShip.Position.Y, BulletType.ALLY);
                GameEvent(GameEventEnum.PLAYER_FIRE);
            }, 700, true);
#endif

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
            EnemyShip.UpdateEnemyPosition(deltaTime, this);

            // random shooting
            if (!playerIsDead && random.Next() % 50 == 5)
            {
                foreach (RotatingShip ship in RotatingShip.ListOfShips)
                {
                    if (ship is EnemyShip)
                    {
                        if (random.Next() % 70 == 6)
                        {
                            Bullet b = new Bullet(ship.Hitbox.Center.X - 2 * RotatingShip.Scale, ship.Position.Y, BulletType.ENEMY);
                        }
                    }
                }
            }

            // choose one enemy on grid and do random movement
            if (!playerIsDead && random.Next() % 60 == 5)
            {
                foreach (RotatingShip ship in RotatingShip.ListOfShips)
                {
                    if (ship is EnemyShip)
                    {
                        EnemyShip enemy = (EnemyShip)ship;
                        if (random.Next() % 30 == 6 && enemy.MovingOnGrid)
                        {
                            List<Point> road_1 = QuadraticBezier.CalculatePoints(
                                new Point((int)enemy.Position.X, (int)enemy.Position.Y),
                                getRandomPointOnScreen(),
                                getRandomPointOnScreen(), 30);
                            enemy.BeginMovingOnCurve(road_1, 1.0f);
                        }
                    }
                }
            }
        }

        private Point getRandomPointOnScreen()
        {
            return new Point(random.Next(-400, GalagaGame.GAME_WIDTH+400), random.Next(900, GalagaGame.GAME_HEIGHT-500));
        }

        void ScreenTapped(object s, EventArgs _t)
        {
            //ScreenTapEventArgs t = (ScreenTapEventArgs)_t;
            
            //Point click = new Point(t.x, t.y);
            //Rectangle fireButtonPos = new Rectangle(700, 1300, fireTexture.Width, fireTexture.Height);
            //if (fireButtonPos.Contains(click))
            //{
            //    // don't shoot if player is dead
            //    if (!RotatingShip.ListOfShips.Contains(playerShip))
            //        return;

            //    Bullet b = new Bullet(playerShip.Hitbox.Center.X - 2 * RotatingShip.Scale, playerShip.Position.Y, BulletType.ALLY);
            //    GameEvent(GameEventEnum.PLAYER_FIRE);
            //}
        }

        int movingTouchId = -1;
        void ScreenTouched(object s, EventArgs _t)
        {
            ScreenTouchEventArgs t = (ScreenTouchEventArgs)_t;
            Point click = new Point(t.x, t.y);
            Rectangle touchArea = new Rectangle(0, GalagaGame.GAME_HEIGHT - 500, 
                GalagaGame.GAME_WIDTH, GalagaGame.GAME_HEIGHT);

            if (touchArea.Contains(click))
            {
                float move = game.deltaTime * 0.7f;
                if (click.X < playerShip.Position.X) // move left
                {
                    float x = playerShip.Position.X - move;
                    if (x >= click.X)
                        playerShip.MovePlayer(x);
                }
                else // move right
                {
                    float x = playerShip.Position.X + move;
                    if (x <= click.X)
                        playerShip.MovePlayer(x);
                }
            }

            //Rectangle biggerPlayerHitbox = playerShip.Hitbox;
            //biggerPlayerHitbox.X -= 100;
            //biggerPlayerHitbox.Y -= 100;
            //biggerPlayerHitbox.Width += 100;
            //biggerPlayerHitbox.Height += 100;
            //if (biggerPlayerHitbox.Contains(click))
            //{
            //    movingTouchId = t.id;
            //}

            //if(movingTouchId == t.id)
            //{
            //    playerShip.MovePlayer(t.x);
            //}
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
                // don't shoot if player is dead
                if (!RotatingShip.ListOfShips.Contains(playerShip))
                    return;

                Bullet b = new Bullet(playerShip.Hitbox.Center.X - 2 * RotatingShip.Scale, playerShip.Position.Y, BulletType.ALLY);
                GameEvent(GameEventEnum.PLAYER_FIRE);
            }

        }

        public GameMode(GalagaGame game)
        {
            this.game = game;
            random = new Random();
            explosions = new List<Explosion>(); 
        }
    }
}
