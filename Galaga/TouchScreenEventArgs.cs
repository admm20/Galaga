using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
        class ScreenTouchEventArgs : EventArgs
        {
            public int x, y, id;
            public ScreenTouchEventArgs(int x, int y, int id)
            {
                this.x = x;
                this.y = y;
                this.id = id;
            }
        }
        
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
