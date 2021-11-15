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

            ImageLeft.Source = bitmap;

            //bitmap = (BitmapImage)Invert(bitmap);

            ChangePicture();
            
        }

        void ChangePicture()
        {
            //ImageRight.Source = Invert((BitmapSource)ImageLeft.Source);
            //ImageRight.Source = MakeGray((BitmapSource)ImageLeft.Source);
            ImageRight.Source = BlackOrWhite((BitmapSource)ImageLeft.Source);
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
    }
}
