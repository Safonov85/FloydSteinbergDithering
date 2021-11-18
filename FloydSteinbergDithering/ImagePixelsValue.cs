using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FloydSteinbergDithering
{
    public class ImagePixelsValue
    {
        // variable created with self-made classes
        public Dictionary<PixelPosition, RGBaValue> GetPixelValue = new Dictionary<PixelPosition, RGBaValue>();

        public void CopyPixelRGBInfo(BitmapImage bitmap)
        {
            if (GetPixelValue.Count != 0)
            {
                GetPixelValue.Clear();
            }
            int stride = bitmap.PixelWidth * 4;
            int size = bitmap.PixelHeight * stride;
            byte[] pixelsRGBA = new byte[size];
            bitmap.CopyPixels(pixelsRGBA, stride, 0);

            for (int i = 0; i < size; i += 4)
            {
                int grayScale = (pixelsRGBA[i] + pixelsRGBA[i + 1] + pixelsRGBA[i + 2]) / 3;
                pixelsRGBA[i] = (byte)grayScale;
                pixelsRGBA[i + 1] = (byte)grayScale;
                pixelsRGBA[i + 2] = (byte)grayScale;
            }

            //int red = 0;
            int currentPixelX = 0;
            int currentPixelY = 0;

            // cycles through all the RGB for everypixel and sets them proper order in GetPixelValue "array"
            for (int i = 0; i < pixelsRGBA.Length; i += 4)
            {
                GetPixelValue.Add(new PixelPosition(currentPixelX, currentPixelY),
                    new RGBaValue(pixelsRGBA[i], pixelsRGBA[i + 1], pixelsRGBA[i + 2], 0));

                currentPixelX++;

                if (currentPixelX > bitmap.PixelWidth - 1)
                {
                    currentPixelY++;
                    currentPixelX = 0;
                }
            }
        }
    }
}