using System.Threading;
using System.Drawing.Configuration;
using System.Drawing;
using System;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WpfApp1
{
    public class TaskManager
    {
        public int numberOfThreads { get; set; }
        public string inputPath { get; }
        public bool isAsm { get; set; }

        public TaskManager(string path, bool isAsm, int numberOfThreads)
        {
            this.numberOfThreads = numberOfThreads;
            this.inputPath = path;
            this.isAsm = isAsm;
            setThreads();
        }

        public void setThreads()
        {
            ThreadPool.SetMinThreads(numberOfThreads, 1);
            ThreadPool.SetMaxThreads(numberOfThreads, 1);
        }

        public unsafe (Bitmap, double) manageAlgorithm()
        {
            Bitmap image = new(inputPath);
            (Bitmap, double) t1 = Grayscale(ref image);
            return (t1.Item1, t1.Item2);
        }

        [DllImport(@"D:\ProjektJA\ProjektJA-main\WpfApp1\x64\Debug\JAAsm.dll")]
        unsafe private static extern void GrayscaleAsm(byte* pCur, byte* output, int width);

        unsafe public (Bitmap, double) Grayscale(ref Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            byte* beginning = (byte*)ptr;
            int stride = bmpData.Stride;
            int height = bmpData.Height;
            int width = bmpData.Width;
            
            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte* outputPtr = (byte*)outputData.Scan0.ToPointer();
            int outStride = outputData.Stride;

            double elapsedTime = universalGrayscale(beginning, stride, outputPtr, outStride, height, width);

            output.UnlockBits(outputData);
            bmp.UnlockBits(bmpData);
            return (output, elapsedTime);
        }

        public unsafe double universalGrayscale(byte* beginning, int stride, byte* outputPtr, int outStride, int height, int width)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (this.isAsm)
            {
                int toProcess = height;
                using (ManualResetEvent resetEvent = new ManualResetEvent(false))
                {
                    for (int y = 0; y < height; y++)
                    {
                        int yy = y;
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            GrayscaleAsm(beginning + (yy * stride), outputPtr + (yy * outStride), width);

                            if (Interlocked.Decrement(ref toProcess) == 0)
                                resetEvent.Set();
                        });
                    }
                    resetEvent.WaitOne();
                }
            }
            else
            {
                int toProcess = height;
                using (ManualResetEvent resetEvent = new ManualResetEvent(false))
                {
                    for (int y = 0; y < height; y++)
                    {
                        int yy = y;
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            CSharpDLL.CSharpConversion.GrayscaleCSharp(beginning + (yy * stride), outputPtr + (yy * outStride), width);

                            if (Interlocked.Decrement(ref toProcess) == 0)
                                resetEvent.Set();
                        });
                    }
                    resetEvent.WaitOne();
                }
            }
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }


        public unsafe void generateBenchmark()
        {
            Bitmap bmp = new(inputPath);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            byte* beginning = (byte*)ptr;
            int stride = bmpData.Stride;
            int height = bmpData.Height;
            int width = bmpData.Width;

            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;
            BitmapData outputData = output.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte* outputPtr = (byte*)outputData.Scan0.ToPointer();
            int outStride = outputData.Stride;

            this.isAsm = true;
            for (int i = 1; i <= 12; i++)
            {
                this.numberOfThreads = i;
                setThreads();
                double timeSum = 0;
                for (int j = 0; j < 1000; j++)
                {
                    timeSum += universalGrayscale(beginning, stride, outputPtr, outStride, height, width);
                }
                string toWrite = $"ASM, Threads: {this.numberOfThreads}, Time: {timeSum/1000}";
                using (FileStream fs = new FileStream(@"D:\ProjektJA\MyTest.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(toWrite);
                }
            }
            this.isAsm = false;
            for(int i = 1; i <= 12; i++)
            {
                this.numberOfThreads = i;
                setThreads();
                double timeSum = 0;
                for (int j = 0; j < 1000; j++)
                {
                    timeSum += universalGrayscale(beginning, stride, outputPtr, outStride, height, width);
                }
                string toWrite = $"C#, Threads: {this.numberOfThreads}, Time: {timeSum/1000}";
                using (FileStream fs = new FileStream(@"D:\ProjektJA\MyTest.txt", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(toWrite);
                }
            }
        }
    }
}