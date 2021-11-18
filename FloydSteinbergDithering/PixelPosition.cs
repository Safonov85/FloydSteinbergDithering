using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloydSteinbergDithering
{
    // class used for ImagePixelValue.GetPixelValue Variable/Object
    public class PixelPosition
    {
        public PixelPosition(int x, int y)
        {
            PixelPositionX = x;
            PixelPositionY = y;
        }
        public int PixelPositionX { get; set; }
        public int PixelPositionY { get; set; }
    }
}