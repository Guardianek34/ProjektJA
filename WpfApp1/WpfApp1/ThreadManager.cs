using System.Threading;
using System.Drawing.Configuration;
using System.Drawing;
using System;

namespace WpfApp1
{
    public class ThreadManager
    {
        public int NumberOfThreads { get; set; }
        public int NumberOfTasks { get; set; } = 64;
        public string Filename { get; }
        public bool? IsAsm { get; }

        public ThreadManager(string filename, bool? isAsm, int numberOfThreads)
        {
            this.NumberOfThreads = numberOfThreads;
            this.Filename = filename;
            this.IsAsm = isAsm;
        }

        public void createThreadPool()
        {
            var doneTasks = new ManualResetEvent[NumberOfTasks]; // number of tasks
            ThreadPool.SetMinThreads(NumberOfThreads, 1);
            ThreadPool.SetMaxThreads(NumberOfThreads, 1);

            for (int i = 0; i < NumberOfTasks; i++)
            {
                doneTasks[i] = new ManualResetEvent(false);
                SobelAlgorithm f = new SobelAlgorithm(new Bitmap(100, 100), doneTasks[i]); // tworzenie klasy z algorytmem
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i); // task jako wykonanie metody
            }
            WaitHandle.WaitAll(doneTasks);
            Console.WriteLine("All calculations are complete.");
        }
             
    }

    public class SobelAlgorithm
    {
        private readonly ManualResetEvent DoneTask;
        public Bitmap ImageFragment { get; set; }

        public SobelAlgorithm(Bitmap fragment, ManualResetEvent _doneTask)
        {
            ImageFragment = fragment;
            DoneTask = _doneTask;
        }

        public void runAlgorithm()
        {
            int n = 0;
            for(int i = 0; i < 1000000; i++)
            {
                n += 10000;
            }
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            runAlgorithm();
            DoneTask.Set();
        }
    }


}
