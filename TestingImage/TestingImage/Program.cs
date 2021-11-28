using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TestingImage
{
    class Program
    {
        unsafe static void Main(string[] args)
        {

            const string inputPath = @"C:\Users\LENOVO\Desktop\Sobel\karsten-wurth-karsten-wuerth-7BjhtdogU3A-unsplash.jpg";
            const string outputPath = @"C:\Users\LENOVO\Desktop\Sobel\karsten-wurth-karsten-wuerth-7BjhtdogU3A-unsplash.out2.jpg";

            Bitmap image = new(inputPath);

            if(image.PixelFormat != PixelFormat.Format24bppRgb)
            {
                return;
            }

            // Obtain grayscale conversion of the image
            byte[] grayData = GrayScale_Scan0(ref image);

            int width = image.Width;
            int height = image.Height;

            // Buffers
            byte[] buffer = new byte[9];

            BitmapData bmpData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite,
                     PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            byte* line = (byte*)ptr;
            int stride = bmpData.Stride;
           
            // First pass - convolve sobel operator and calculate orientation. We're using the byte array now, since it's easier.
            for (int y = 1; y < height - 1; y++)
            {
                //byte* pCur = PtrFirstPixel + ( (y-1) * stride );
                byte* pCur = line;
                for (int x = 1; x < width - 1; x++)
                {
                    // Unlike the other Kernel operations, where the radius etc. might change this one is simple so we can hard code
                    // the kernel operations in. Pointer arithmetic would make this slightly faster, but we won't worry about it.
                    int index = y * width + x;

                    // 3x3 window around (x,y)
                    buffer[0] = grayData[index - width - 1];
                    buffer[1] = grayData[index - width];
                    buffer[2] = grayData[index - width + 1];
                    buffer[3] = grayData[index - 1];
                    buffer[4] = grayData[index];
                    buffer[5] = grayData[index + 1];
                    buffer[6] = grayData[index + width - 1];
                    buffer[7] = grayData[index + width];
                    buffer[8] = grayData[index + width + 1];

                    // Sobel horizontal and vertical response
                    double dx = buffer[2] + 2 * buffer[5] + buffer[8] - buffer[0] - 2 * buffer[3] - buffer[6];
                    double dy = buffer[6] + 2 * buffer[7] + buffer[8] - buffer[0] - 2 * buffer[1] - buffer[2];

                    double sobel = Math.Sqrt(dx * dx + dy * dy);
                    if (sobel > 255) sobel = 255;
                    else if (sobel < 0) sobel = 0;

                    pCur[0] = pCur[1] = pCur[2] = (byte)sobel;
                    pCur += 3;
                }
                line += stride;
            }

            image.UnlockBits(bmpData);
            image.Save(outputPath);
        }

        unsafe public static byte[] GrayScale_Scan0(ref Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                     PixelFormat.Format24bppRgb);

            byte[] grayData = new byte[bmp.Width * bmp.Height];
            IntPtr ptr = bmpData.Scan0;
            byte* line = (byte*)ptr;
            int stride = bmpData.Stride;


            for (int y = 0; y < bmpData.Height; y++)
            {
                byte* pCur = line;
                for (int x = 0; x < bmpData.Width; x++)
                {
                    byte gray = (byte)(pCur[2] * 0.299 + pCur[1] * 0.587 + pCur[0] * 0.114);
                    grayData[y * bmp.Width + x] = gray;
                    pCur += 3; // because 3 pixels * 1 byte per pixel = 3 (different value for 32 bpp, etc.)
                }
                line += stride;
            }
            bmp.UnlockBits(bmpData);
            return grayData;
        }

    }
}
