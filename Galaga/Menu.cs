using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galaga
{
    class Menu : GameState
    {
        GalagaGame game;

        Texture2D background;
        Texture2D start_button;
        Texture2D start_selected_button;
        Texture2D end_button;
        Texture2D end_selected_button;

        int hover = 1;

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(GalagaGame.GAME_WIDTH / 2 - background.Width / 2, 0), Color.White);
            if (hover == 1)
                spriteBatch.Draw(start_selected_button, new Vector2(GalagaGame.GAME_WIDTH / 2 - start_selected_button.Width / 2, 900), Color.White);
            else
                spriteBatch.Draw(start_button, new Vector2(GalagaGame.GAME_WIDTH / 2 - start_button.Width / 2, 900), Color.White);

            if (hover == 2)
                spriteBatch.Draw(end_selected_button, new Vector2(GalagaGame.GAME_WIDTH / 2 - end_selected_button.Width / 2, 1100), Color.White);
            else
                spriteBatch.Draw(end_button, new Vector2(GalagaGame.GAME_WIDTH / 2 - end_button.Width / 2, 1100), Color.White);

            spriteBatch.End();
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Textures/menu");
            start_button = content.Load<Texture2D>("Textures/start-game");
            start_selected_button = content.Load<Texture2D>("Textures/start-game-selected");
            end_button = content.Load<Texture2D>("Textures/exit");
            end_selected_button = content.Load<Texture2D>("Textures/exit-selected");
        }

        public override void OnEnter()
        {
            game.KeyboardKeyClicked += KeyClicked;
        }

        void KeyClicked(object s, EventArgs _k)
        {
            KeyboardKeyClickedEventArgs k = (KeyboardKeyClickedEventArgs)_k;
            if (k.key == Keys.Down)
            {
                hover++;
                if (hover > 2)
                    hover = 2;
            }
            else if (k.key == Keys.Up)
            {
                hover--;
                if (hover < 1)
                    hover = 1;
            }
            else if(k.key == Keys.Enter)
            {
                if(hover==1)
                {
                    game.KeyboardKeyClicked -= KeyClicked;
                    game.RunGameMode();
                }
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

        }

        public Menu(GalagaGame game)
        {
            this.game = game;
        }
    }
}
