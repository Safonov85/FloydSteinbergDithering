using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            MainWindowTitle.Title = fileList[0];

            ImageLeft.Source = bitmap;

            ImageRight.Source = Floyd(bitmap);

            

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

        public BitmapSource Floyd(BitmapSource source)
        {

            // Calculate stride of source
            int stride = source.PixelWidth * 4;

            // Create data array to hold source pixel data
            int length = stride * source.PixelHeight;
            byte[] data = new byte[length];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);
            
            // GRAYSCALE
            //for (int i = 0; i < length; i += 4)
            //{
            //    int grayScale = (data[i] + data[i + 1] + data[i + 2]) / 3;
            //    data[i] = (byte)grayScale;
            //    data[i + 1] = (byte)grayScale;
            //    data[i + 2] = (byte)grayScale;
            //}

            // Make
            for (int y = 0; y < source.Height; y ++)
            {
                for (int x = 0; x < source.Width ; x ++)
                {
                    int indexing = y * stride + 4 * x;

                    if (indexing >= data.Length - 10)
                    {
                        break;
                    }

                    //Color colorNow = new Color();
                    //colorNow.R = data[indexing];
                    //colorNow.G = data[indexing + 1];
                    //colorNow.B = data[indexing + 2];


                    // Floyd-Steinberg Dithering Begins
                    float oldB = data[indexing];
                    float oldG = data[indexing + 1];
                    float oldR = data[indexing + 2];

                    float newR;
                    float newG;
                    float newB;

                    // Change factor for color amount
                    int factor = Int32.Parse(FactorTextBox.Text);
                    newB = (float)(Math.Round(factor * oldB / 255) * (255 / factor));
                    newG = (float)(Math.Round(factor * oldG / 255) * (255 / factor));
                    newR = (float)(Math.Round(factor * oldR / 255) * (255 / factor));
                    //data[index(x, y)] = pix;

                    //if(oldR < 128)
                    //{
                    //    newR = 0;
                    //    newG = 0;
                    //    newB = 0;
                    //}
                    //else
                    //{
                    //    newR = 255;
                    //    newG = 255;
                    //    newB = 255;
                    //}


                    data[indexing    ] = (byte)newB;
                    data[indexing + 1] = (byte)newG;
                    data[indexing + 2] = (byte)newR;

                    
                    float errB = oldB - newB;
                    float errG = oldG - newG;
                    float errR = oldR - newR;

                    indexing = y * stride + 4 * (x + 1);

                    if (indexing >= data.Length - 10)
                    {
                        break;
                    }

                    float b = data[indexing];
                    float g = data[indexing + 1];
                    float r = data[indexing + 2];
                    b += errB * 7.0f / 16.0f;
                    g += errG * 7.0f / 16.0f;
                    r += errR * 7.0f / 16.0f;
                    data[indexing] = (byte)b;
                    data[indexing + 1] = (byte)g;
                    data[indexing + 2] = (byte)r;

                    indexing = (y + 1) * stride + 4 * (x - 1);

                    if (indexing >= data.Length - 10)
                    {
                        break;
                    }

                    b = data[indexing];
                    g = data[indexing + 1];
                    r = data[indexing + 2];
                    b += errB * 3.0f / 16.0f;
                    g += errG * 3.0f / 16.0f;
                    r += errR * 3.0f / 16.0f;
                    data[indexing] = (byte)b;
                    data[indexing + 1] = (byte)g;
                    data[indexing + 2] = (byte)r;

                    indexing = (y + 1) * stride + 4 * x;

                    if (indexing >= data.Length - 10)
                    {
                        break;
                    }

                    b = data[indexing];
                    g = data[indexing + 1];
                    r = data[indexing + 2];
                    b += errB * 5.0f / 16.0f;
                    g += errG * 5.0f / 16.0f;
                    r += errR * 5.0f / 16.0f;
                    data[indexing] = (byte)b;
                    data[indexing + 1] = (byte)g;
                    data[indexing + 2] = (byte)r;

                    indexing = (y + 1) * stride + 4 * (x + 1);

                    if (indexing >= data.Length - 10)
                    {
                        break;
                    }

                    b = data[indexing];
                    g = data[indexing + 1];
                    r = data[indexing + 2];
                    b += errB * 1.0f / 16.0f;
                    g += errG * 1.0f / 16.0f;
                    r += errR * 1.0f / 16.0f;
                    data[indexing] = (byte)b;
                    data[indexing + 1] = (byte)g;
                    data[indexing + 2] = (byte)r;

                    //if (r > 250 && g > 250 && b > 250 && colorNow.R < 20 && colorNow.G < 20 && colorNow.B < 20)
                    //{
                    //    // um ok.......
                    //    //Debug.WriteLine("Got one");
                    //    data[indexing] = 255;
                    //    data[indexing + 1] = 0;
                    //    data[indexing + 2] = 0;
                    //}
                }
                
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

        //public BitmapSource FloydCross(BitmapSource source)
        //{

        //    // Calculate stride of source
        //    int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

        //    // Create data array to hold source pixel data
        //    int length = stride * source.PixelHeight;
        //    byte[] data = new byte[length];

        //    // Copy source image pixels to the data array
        //    source.CopyPixels(data, stride, 0);

        //    // GRAYSCALE
        //    for (int i = 0; i < length; i += 4)
        //    {
        //        int grayScale = (data[i] + data[i + 1] + data[i + 2]) / 3;
        //        data[i] = (byte)grayScale;
        //        data[i + 1] = (byte)grayScale;
        //        data[i + 2] = (byte)grayScale;
        //    }

        //    // Make
        //    for (int y = 1; y < source.Height; y++)
        //    {
        //        for (int x = 1; x < source.Width; x++)
        //        {
        //            int indexing = y * stride + 4 * x;

        //            if (indexing >= data.Length - 10)
        //            {
        //                break;
        //            }



        //            Color colorNow = new Color();
        //            colorNow.R = data[indexing];
        //            colorNow.G = data[indexing + 1];
        //            colorNow.B = data[indexing + 2];

        //            //int colorLevel = 50;

        //            //if(data[indexing] < colorLevel && data[indexing+1] < colorLevel && data[indexing+2] < colorLevel)
        //            //{
        //            //    continue;
        //            //}



        //            // Floyd-Steinberg Dithering Begins
        //            float oldR = data[indexing];
        //            float oldG = data[indexing + 1];
        //            float oldB = data[indexing + 2];

        //            int newR;
        //            int newG;
        //            int newB;

        //            // Change factor for color amount
        //            int factor = Int32.Parse(FactorTextBox.Text);
        //            newR = (int)Math.Round(factor * oldR / 255) * (255 / factor);
        //            newG = (int)Math.Round(factor * oldG / 255) * (255 / factor);
        //            newB = (int)Math.Round(factor * oldB / 255) * (255 / factor);
        //            //data[index(x, y)] = pix;

        //            //if(oldR < 128)
        //            //{
        //            //    newR = 0;
        //            //    newG = 0;
        //            //    newB = 0;
        //            //}
        //            //else
        //            //{
        //            //    newR = 255;
        //            //    newG = 255;
        //            //    newB = 255;
        //            //}


        //            data[indexing] = (byte)newR;
        //            data[indexing + 1] = (byte)newG;
        //            data[indexing + 2] = (byte)newB;


        //            float errR = oldR - newR;
        //            float errG = oldG - newG;
        //            float errB = oldB - newB;

        //            indexing = (y - 1) * stride + 4 * (x + 1);

        //            if (indexing >= data.Length - 10)
        //            {
        //                break;
        //            }

        //            float r = data[indexing];
        //            float g = data[indexing + 1];
        //            float b = data[indexing + 2];
        //            r = r + errR * 7 / 16.0f;
        //            g = g + errG * 7 / 16.0f;
        //            b = b + errB * 7 / 16.0f;
        //            data[indexing] = (byte)r;
        //            data[indexing + 1] = (byte)g;
        //            data[indexing + 2] = (byte)b;

        //            indexing = (y + 1) * stride + 4 * (x + 1);

        //            if (indexing >= data.Length - 10)
        //            {
        //                break;
        //            }

        //            r = data[indexing];
        //            g = data[indexing + 1];
        //            b = data[indexing + 2];
        //            r = r + errR * 3 / 16.0f;
        //            g = g + errG * 3 / 16.0f;
        //            b = b + errB * 3 / 16.0f;
        //            data[indexing] = (byte)r;
        //            data[indexing + 1] = (byte)g;
        //            data[indexing + 2] = (byte)b;

        //            indexing = (y + 1) * stride + 4 * (x - 1);

        //            if (indexing >= data.Length - 10)
        //            {
        //                break;
        //            }

        //            r = data[indexing];
        //            g = data[indexing + 1];
        //            b = data[indexing + 2];
        //            r = r + errR * 5 / 16.0f;
        //            g = g + errG * 5 / 16.0f;
        //            b = b + errB * 5 / 16.0f;
        //            data[indexing] = (byte)r;
        //            data[indexing + 1] = (byte)g;
        //            data[indexing + 2] = (byte)b;

        //            indexing = (y - 1) * stride + 4 * (x - 1);

        //            if (indexing >= data.Length - 10)
        //            {
        //                break;
        //            }

        //            r = data[indexing];
        //            g = data[indexing + 1];
        //            b = data[indexing + 2];
        //            r = r + errR * 1 / 16.0f;
        //            g = g + errG * 1 / 16.0f;
        //            b = b + errB * 1 / 16.0f;
        //            data[indexing] = (byte)r;
        //            data[indexing + 1] = (byte)g;
        //            data[indexing + 2] = (byte)b;

        //            //if (r > 250 && g > 250 && b > 250 && colorNow.R < 20 && colorNow.G < 20 && colorNow.B < 20)
        //            //{
        //            //    // um ok.......
        //            //    //Debug.WriteLine("Got one");
        //            //    data[indexing] = 255;
        //            //    data[indexing + 1] = 0;
        //            //    data[indexing + 2] = 0;
        //            //}
        //        }

        //    }

        //    // Create a new BitmapSource from the inverted pixel buffer
        //    return BitmapSource.Create(
        //        source.PixelWidth, source.PixelHeight,
        //        source.DpiX, source.DpiY, source.Format,
        //        null, data, stride);
        //}

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
