using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    // Contains list of buttons that are being hold down
    class KeyboardKeysDownEventArgs : EventArgs
    {
        public Keys[] keys;
        public KeyboardKeysDownEventArgs(Keys[] k)
        {
            keys = k;
        }
    }

    // Contains one button that was clicked
    class KeyboardKeyClickedEventArgs : EventArgs
    {
        public Keys key;
        public KeyboardKeyClickedEventArgs(Keys k)
        {
            key = k;
        }
    }

}
