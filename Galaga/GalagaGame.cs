
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#if ANDROID
using Android.OS;
#endif

namespace Galaga
{

    public class GalagaGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int GAME_WIDTH, GAME_HEIGHT;


        // aspect ratio is different for windows and android version
        private float gameWidthRatio;
        
        private RenderTarget2D gameRenderer;

        GameState currentMode;
        GameState gameMode;
        GameState menu;

        public int deltaTime = 1;

#if WINDOWS
        public event EventHandler KeyboardKeysDown;
        public event EventHandler KeyboardKeyClicked;
        KeyboardState previousKbState;

        private void KbKeysDown(EventArgs e)
        {
            EventHandler handler = KeyboardKeysDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void KbKeyClicked(EventArgs e)
        {
            EventHandler handler = KeyboardKeyClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
#endif

        public void RunGameMode()
        {
            currentMode = gameMode;
            currentMode.OnEnter();
        }

        public void RunMenu()
        {
            currentMode = menu;
            currentMode.OnEnter();
        }

        public GalagaGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

#if WINDOWS
            GAME_WIDTH = 1560;
            GAME_HEIGHT = 1920;
            gameWidthRatio = 13;
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 720;
#elif ANDROID
            GAME_WIDTH = 1080;
            GAME_HEIGHT = 1920;
            gameWidthRatio = 9;
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 1920;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#endif
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameRenderer = new RenderTarget2D(GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);

            gameMode = new GameMode(this);
            menu = new Menu(this);
            //RunGameMode();
            RunMenu();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            gameMode.LoadContent(Content);
            menu.LoadContent(Content);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
#if WINDOWS
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#elif ANDROID
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                //Exit();
                Process.KillProcess(Process.MyPid());
            }
#endif

            // TODO: Add your update logic here
            Timer.UpdateAllTimers(gameTime.ElapsedGameTime.Milliseconds);

#if WINDOWS
            KeyboardState kbState = Keyboard.GetState();
            Keys[] keys = kbState.GetPressedKeys();
            if(keys.Length > 0)
            {
                KbKeysDown(new KeyboardKeysDownEventArgs(keys));
                
                foreach (Keys k in keys)
                {
                    if(previousKbState.IsKeyUp(k))
                    {
                        KbKeyClicked(new KeyboardKeyClickedEventArgs(k));
                    }
                }
            }

            previousKbState = kbState;
#endif

            deltaTime = gameTime.ElapsedGameTime.Milliseconds;

            currentMode.Update(deltaTime);
            


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(gameRenderer);

            GraphicsDevice.Clear(Color.Black);

            currentMode.Draw(spriteBatch);
            
            // Game will be displayed in specified proportions (13:16 on windows and 9:16 on android) 
            PresentationParameters windowSize = GraphicsDevice.PresentationParameters;
            Rectangle rendererPosition = new Rectangle(0, 0, windowSize.BackBufferWidth,
                windowSize.BackBufferHeight);
            

            if (rendererPosition.Width > rendererPosition.Height * gameWidthRatio / 16.0) // too wide
            {
                rendererPosition.Width = (int)(rendererPosition.Height * (gameWidthRatio / 16.0));
                rendererPosition.X = (int)((windowSize.BackBufferWidth - rendererPosition.Width) / 2.0);
            }
            else
            {
                rendererPosition.Height = (int)(rendererPosition.Width * (16.0 / gameWidthRatio));
                rendererPosition.Y = (int)((windowSize.BackBufferHeight - rendererPosition.Height) / 2.0);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White * 0.1f);
            spriteBatch.Begin();
            spriteBatch.Draw(gameRenderer, rendererPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
