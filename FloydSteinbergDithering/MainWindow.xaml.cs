using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FloydSteinbergDithering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage bitmap = new BitmapImage();
        ImagePixelsValue picture2 = new ImagePixelsValue();


        public MainWindow()
        {
            InitializeComponent();
            ImageLeft.AllowDrop = true;
            //Debug.WriteLine(AverageNum(10, 50, 7));
        }

        private void ImageLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MainGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void MainGrid_Drop(object sender, DragEventArgs e)
        {
            bitmap = new BitmapImage();
            bitmap.BeginInit();

            string[] fileList = e.Data.GetData(DataFormats.FileDrop) as string[];

            bitmap.UriSource = new Uri(fileList[0]);
            bitmap.EndInit();

            ImageLeft.Source = GrayScaleImage(bitmap);


            //bitmap = (BitmapImage)Invert(bitmap);

            ChangePicture();
            
            
        }

        // HUGE MESS
        void ChangePicture()
        {
            //ImageRight.Source = Invert((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = MakeGray((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = BlackOrWhite((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = BlackOrWhiteOrGray((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = FloydSteinbergDithering((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = BlackOrWhiteDots((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = FloydSteinbergDitheringAgain((BitmapSource)ImageLeft.Source);

            DrawingVisual drawVis = new DrawingVisual();
            using (DrawingContext dc = drawVis.RenderOpen())
            {
                dc.DrawImage(ImageLeft.Source, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

                // Calculate stride of source
                int stride = (bitmap.PixelWidth * 4);

                // Create data array to hold source pixel data
                int length = stride * bitmap.PixelHeight;
                byte[] data = new byte[length];

                // Copy source image pixels to the data array
                bitmap.CopyPixels(data, stride, 0);


                

                ImagePixelsValue picture = new ImagePixelsValue();

                picture.CopyPixelRGBInfo(bitmap);
                picture2.CopyPixelRGBInfo(bitmap);

                //ImageRight.Source = ImageLeft.Source;

                foreach (var item in picture2.GetPixelValue)
                {

                    //int index = bitmap.PixelWidth * stride + 4 * bitmap.PixelHeight;

                    float oldR = item.Value.Red;
                    float oldG = item.Value.Green;
                    float oldB = item.Value.Blue;
                    int factor = 4;
                    int newR = (int)Math.Round(factor * oldR / 255) * (255 / factor);
                    int newG = (int)Math.Round(factor * oldG / 255) * (255 / factor);
                    int newB = (int)Math.Round(factor * oldB / 255) * (255 / factor);
                    //data[index(x, y)] = pix;
                    item.Value.Red = (byte)newR;
                    item.Value.Green = (byte)newB;
                    item.Value.Blue = (byte)newG;

                    SolidColorBrush colors = new SolidColorBrush(Color.FromRgb(item.Value.Red, item.Value.Green, item.Value.Blue));


                    dc.DrawRectangle(colors, null,
                        new Rect(new Point(item.Key.PixelPositionX, item.Key.PixelPositionY),
                                 new Point(item.Key.PixelPositionX + 1, item.Key.PixelPositionY + 1)));

                    float errR = oldR - newR;
                    float errG = oldG - newG;
                    float errB = oldB - newB;

                    //PixelPosition lastposition = new PixelPosition(item.Key.PixelPositionX + 1, item.Key.PixelPositionY);
                    //RGBaValue currentRGB = picture.GetPixelValue[lastposition];

                    //float r = currentRGB.Red;
                    //float g = currentRGB.Green;
                    //float b = currentRGB.Blue;
                    //r = r + errR * 7 / 16;
                    //g = g + errG * 7 / 16;
                    //b = b + errB * 7 / 16;

                    //item.Value.Red = (byte)r;
                    //item.Value.Green = (byte)g;
                    //item.Value.Blue = (byte)b;

                    //colors = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));

                    //// (x + 1, y    )
                    //dc.DrawRectangle(colors, null,
                    //    new Rect(new Point(item.Key.PixelPositionX + 1, item.Key.PixelPositionY),
                    //             new Point(item.Key.PixelPositionX + 2, item.Key.PixelPositionY + 2)));

                }
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawVis);

            ImageRight.Source = rtb;



        }

        public BitmapSource GrayScaleImage(BitmapSource source)
        {

            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Make GRAYSCALE
            for (int i = 0; i < length; i += 4)
            {
                int grayScale = (data[i] + data[i + 1] + data[i + 2]) / 3; 
                data[i] = (byte)grayScale;
                data[i + 1] = (byte)grayScale;
                data[i + 2] = (byte)grayScale;
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        public BitmapSource Invert(BitmapSource source)
        {
            
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Change this loop for other formats
            for (int i = 0; i < length; i += 4)
            {
                data[i] = (byte)(255 - data[i]); //R
                data[i + 1] = (byte)(255 - data[i + 1]); //G
                data[i + 2] = (byte)(255 - data[i + 2]); //B
                                                         //data[i + 3] = (byte)(255 - data[i + 3]); //A
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        public BitmapSource MakeGray(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // MAKES GRAY !!!
            for (int i = 0; i < length; i += 4)
            {
                data[i] = (byte)(data[i+2]); //R
                data[i + 1] = (byte)(data[i + 2]); //G
                //data[i + 2] = (byte)(255 - data[i + 2]); //B
                                                         //data[i + 3] = (byte)(255 - data[i + 3]); //A
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }


        public BitmapSource BlackOrWhite(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // MAKES Black or White !!!
            for (int i = 0; i < length; i += 4)
            {
                // looks for average RGB number
                int average = AverageNum(data[i], data[i + 1], data[i + 2]);

                if(average < 128)
                {
                    data[i] = 0; //R
                    data[i + 1] = 0; //G
                    data[i + 2] = 0; //B
                }
                else
                {
                    data[i] = 255; //R
                    data[i + 1] = 255; //G
                    data[i + 2] = 255; //B
                }
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        public BitmapSource BlackOrWhiteOrGray(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // MAKES Black or White !!!
            for (int i = 0; i < length; i += 4)
            {
                // looks for average RGB number
                int average = AverageNum(data[i], data[i + 1], data[i + 2]);

                if (average < 85)
                {
                    data[i] = 0; //R
                    data[i + 1] = 0; //G
                    data[i + 2] = 0; //B
                }
                else if(average > 85 && average < 170)
                {
                    data[i] = 127; //R
                    data[i + 1] = 127; //G
                    data[i + 2] = 127; //B
                }
                else if(average > 170)
                {
                    data[i    ] = 255; //R
                    data[i + 1] = 255; //G
                    data[i + 2] = 255; //B
                }
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        public BitmapSource FloydSteinbergDithering(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);



            // MAKES Didthering !!!
            for (int i = 4; i <= length - (source.PixelWidth*2); i += 4)
            {
                //byte pix = data[index(x, y)];
                float oldR = data[i];
                float oldG = data[i + 1];
                float oldB = data[i + 2];
                // Change factor for color amount
                int factor = 2;
                int newR = (int)Math.Round(factor * oldR / 255) * (255 / factor);
                int newG = (int)Math.Round(factor * oldG / 255) * (255 / factor);
                int newB = (int)Math.Round(factor * oldB / 255) * (255 / factor);
                //data[index(x, y)] = pix;
                data[i] = (byte)newR;
                data[i + 1] = (byte)newB;
                data[i + 2] = (byte)newG;


                float errR = oldR - newR;
                float errG = oldG - newG;
                float errB = oldB - newB;


                float r = data[i+4];
                float g = data[i+4 + 1];
                float b = data[i+4 + 2];
                r = r + errR * 7 / 16.0f;
                g = g + errG * 7 / 16.0f;
                b = b + errB * 7 / 16.0f;
                data[i+4] = (byte)r;
                data[i+4 + 1] = (byte)g;
                data[i + 4 + 2] = (byte)b;

                //r = data[(i - 4)];
                //g = data[(i - 4) + 1];
                //b = data[(i - 4) + 2];
                r = data[(i - 4) + source.PixelWidth    ];
                g = data[(i - 4) + source.PixelWidth + 1];
                b = data[(i - 4) + source.PixelWidth + 2];
                r = r + errR * 3 / 16.0f;
                g = g + errG * 3 / 16.0f;
                b = b + errB * 3 / 16.0f;
                data[(i - 4) + source.PixelWidth] = (byte)r;
                data[(i - 4) + source.PixelWidth + 1] = (byte)g;
                data[(i - 4) + source.PixelWidth + 2] = (byte)b;


                r = data[i + source.PixelWidth];
                g = data[i + source.PixelWidth + 1];
                b = data[i + source.PixelWidth + 2];
                r = r + errR * 5 / 16.0f;
                g = g + errG * 5 / 16.0f;
                b = b + errB * 5 / 16.0f;
                data[i + source.PixelWidth] = (byte)r;
                data[i + source.PixelWidth + 1] = (byte)g;
                data[i + source.PixelWidth + 2] = (byte)b;

                r = data[(i + 4) + source.PixelWidth];
                g = data[(i + 4) + source.PixelWidth + 1];
                b = data[(i + 4) + source.PixelWidth + 2];
                r = r + errR * 1 / 16.0f;
                g = g + errG * 1 / 16.0f;
                b = b + errB * 1 / 16.0f;
                data[(i + 4) + source.PixelWidth] = (byte)r;
                data[(i + 4) + source.PixelWidth + 1] = (byte)g;
                data[(i + 4) + source.PixelWidth + 2] = (byte)b;


                //data[index(x + 1, y    )] = 0;
                //data[index(x - 1, y + 1)] = 0;
                //data[index(x, y + 1    )] = 0;
                //data[index(x + 1, y + 1)] = 0;
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        int index(int x, int y)
        {
            return x + y * (int)ImageLeft.Width;
        }

        public BitmapSource BlackOrWhiteDots(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // MAKES Black or White !!!
            for (int i = 0; i < length - 10; i += 4)
            {
                // looks for average RGB number
                int average = AverageNum(data[i], data[i + 1], data[i + 2]);
                int averageNext = AverageNum(data[i + 4], data[i + 5], data[i + 6]);


                if (average < 127 && averageNext < 127)
                {
                    data[i] = 0; //R
                    data[i + 1] = 0; //G
                    data[i + 2] = 0; //B

                    // next pixel
                    data[i + 16] = 255; //R
                    data[i + 17] = 255; //G
                    data[i + 18] = 255; //B
                }
                else
                {
                    data[i] = 255; //R
                    data[i + 1] = 255; //G
                    data[i + 2] = 255; //B
                }
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }

        void ChangePictureTEST() // Not Used
        {
            DrawingVisual drawVis = new DrawingVisual();

            using (DrawingContext dc = drawVis.RenderOpen())
            {
                dc.DrawImage(bitmap, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                dc.DrawLine(new Pen(Brushes.Red, 2), new Point(0, 0), new Point(bitmap.Width, bitmap.Height));
                dc.DrawRectangle(Brushes.Green, null, new Rect(20, 20, 150, 100));

            }

            RenderTargetBitmap rtb = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawVis);



            ImageRight.Source = rtb;

            ImageRight.Source = Invert((BitmapSource)ImageRight.Source);
        }

        int AverageNum(int one, int two, int three)
        {
            int[] listing = new int[] { one, two, three };

            double result = listing.Average();

            return (int)result;
        }


        //
        // SKRÄP SLÄNG NÄR KLAR!!!! 
        //
        public BitmapSource FloydSteinbergDitheringAgain(BitmapSource source)
        {
            // Calculate stride of source
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            ImagePixelsValue picture = new ImagePixelsValue();

            picture.CopyPixelRGBInfo(bitmap);
            picture2.CopyPixelRGBInfo(bitmap);


            // MAKES Didthering !!!
            for (int y = 0; y < source.PixelHeight; y++)
            {
                for (int x = 0; x < source.PixelWidth; x++)
                {

                    //float oldR = picture.GetPixelValue.Values.ElementAt(x + y).Red;
                    //float oldG = picture.GetPixelValue.Values.ElementAt(x + y).Green;
                    //float oldB = picture.GetPixelValue.Values.ElementAt(x + y).Blue;
                    //int factor = 1;
                    //int newR = (int)Math.Round(factor * oldR / 255) * (255 / factor);
                    //int newG = (int)Math.Round(factor * oldG / 255) * (255 / factor);
                    //int newB = (int)Math.Round(factor * oldB / 255) * (255 / factor);
                    ////data[index(x, y)] = pix;
                    //picture2.GetPixelValue.Values.ElementAt(x + y).Red = (byte)newR;
                    //picture2.GetPixelValue.Values.ElementAt(x + y).Green = (byte)newB;
                    //picture2.GetPixelValue.Values.ElementAt(x + y).Blue = (byte)newG;


                    //data[x + y    ] = 0; //R
                    //data[x + y + 1] = 0; //G
                    //data[x + y + 2] = 0; //B


                    //float errR = oldR - newR;
                    //float errG = oldG - newG;
                    //float errB = oldB - newB;

                    //float r = data[pix];
                    //float g = data[pix + 1];
                    //float b = data[pix + 2];
                    //r = r + errR * 7 / 16;
                    //g = g + errG * 7 / 16;
                    //b = b + errB * 7 / 16;
                    //data[pix] = (byte)r;
                    //data[pix + 1] = (byte)g;
                    //data[pix + 2] = (byte)b;


                    //data[index(x + 1, y    )] = 0;
                    //data[index(x - 1, y + 1)] = 0;
                    //data[index(x, y + 1    )] = 0;
                    //data[index(x + 1, y + 1)] = 0;
                }
            }

            // Create a new BitmapSource from the inverted pixel buffer
            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY, source.Format,
                null, data, stride);
        }
    }
}
