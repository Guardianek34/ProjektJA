using System;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {


            if (ThreadPool.SetMinThreads(2, 1))
            {
                Console.WriteLine("Pizda");
                // The minimum number of threads was set successfully.
            }
            else
            {
                Console.WriteLine("Chuj");
                // The minimum number of threads was not changed.
            }

            if (ThreadPool.SetMaxThreads(2, 1))
            {
                Console.WriteLine("Pizda");
                // The minimum number of threads was set successfully.
            }
            else
            {
                Console.WriteLine("Chuj");
                // The minimum number of threads was not changed.
            }
            ThreadManager program = new ThreadManager("kappa.txt", null, 6);
            program.createThreadPool();
        }
    }

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
          
            for (int i = 0; i < NumberOfTasks; i++)
            {
                doneTasks[i] = new ManualResetEvent(false);
                SobelAlgorithm sobel = new(doneTasks[i]);
                ThreadPool.QueueUserWorkItem(sobel.ThreadPoolCallback, i);
            }
            if (WaitHandle.WaitAll(doneTasks) == true)
            {
                Console.WriteLine("All calculations are completed.");
                Console.WriteLine($"Threads = {ThreadPool.ThreadCount}");
                Console.WriteLine($"Completed = {ThreadPool.CompletedWorkItemCount}");
                Console.WriteLine($"Pending = {ThreadPool.PendingWorkItemCount}");
            }
        }
    }

    public class SobelAlgorithm
    {
        private readonly ManualResetEvent DoneTask;

        public SobelAlgorithm(ManualResetEvent _doneTask)
        {
            DoneTask = _doneTask;
        }

        public void runAlgorithm()
        {
            int n = 0;
            for (int i = 0; i < 1000000; i++)
            {
                n += 10000;
            }
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            int taskIndex = (int)threadContext;
            Console.WriteLine($"Task: {taskIndex} started ; Thread: {Thread.CurrentThread} ...");
            runAlgorithm();
            Console.WriteLine($"Task {taskIndex} result calculated... ; Thread: {Thread.CurrentThread}");
            DoneTask.Set(); // releasing a thread after completing a task
        }
    }

}
