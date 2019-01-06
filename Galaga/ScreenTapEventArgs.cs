using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    class ScreenTapEventArgs : EventArgs
    {
        public int x, y;
        public ScreenTapEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
