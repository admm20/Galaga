using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    abstract class GameState
    {
        public abstract void LoadContent(ContentManager content);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(int deltaTime);
        public abstract void OnEnter();

        // invoked when screen is 100% black
        public abstract void TransitionEffectHalfCompleted();
        
        public abstract void TransitionEffectCompleted();
    }
}
