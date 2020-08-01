using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationToken = new CancellationTokenSource();
            var task1 = RunLongTask(cancellationToken);
            var task2 = RunShortTask();
            CheckRunningTasks(task1, task2);
            while (true)
            {
                if (Char.ToLower(Console.ReadKey().KeyChar) == 'c')
                {
                    cancellationToken.Cancel();
                }
                else
                {
                    Console.WriteLine($"\nSorry :/ I Don't support this input...");
                }
            }
        }

        private static async Task RunShortTask()
        {
            var task = Task.Run(() =>
            {
                var cancellationToken = new CancellationTokenSource();

                Task.Run(() =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (true)
                    {
                        if (cancellationToken.Token.IsCancellationRequested)
                            return;
                        Thread.Sleep(1000);
                        if (!cancellationToken.Token.IsCancellationRequested)
                            Console.WriteLine($"\n[Short Running for {(int)sw.Elapsed.TotalSeconds} seconds]\tI should stop printing when I'm done...");
                    }
                }, cancellationToken.Token);
                Thread.Sleep(3000);
                cancellationToken.Cancel();
            });
            await task;
        }

        private static async Task RunLongTask(CancellationTokenSource cancellationToken)
        {
            await Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    if (sw.Elapsed.TotalSeconds > 30)
                        return;
                    if (sw.Elapsed.TotalSeconds % 2 == 0)
                        Console.WriteLine($"\nPlease Enter C to Cancel Long Runnning Task");

                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (sw.Elapsed.TotalSeconds < 10)
                        {
                            Console.WriteLine($"\nCancelled Long Running Task Successfully!");
                            cancellationToken.Token.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            Console.WriteLine($"\n# ERROR #\tCan NOT cancel Long Running Task after 10 seconds!");
                            throw new Exception("Can NOT cancel Long Running Task after 10 seconds");
                        }
                        break;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine($"\n[Long Running for {(int)sw.Elapsed.TotalSeconds} seconds]\tI should stop printing the moment you Cancel or I'm done...");
                }
            }, cancellationToken.Token);
        }

        private static void CheckRunningTasks(params Task[] tasks)
        {
            var task3 = Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("\n*******************\n\nPrinting Running Tasks Status");
                    var i = 1;
                    foreach (var task in tasks)
                    {
                        PrintCompletation(task, i++.ToString());
                    }
                    Console.WriteLine("\n*******************\n\n");
                    Thread.Sleep(5000);
                }
            });
        }

        private static void PrintCompletation(Task task, string taskName)
        {
            if (task.IsCompleted)
            {
                if (task.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"\n\tTask {taskName} Completed successfully");
                }
                if (task.IsCanceled)
                {
                    Console.WriteLine($"\n\tTask {taskName} Cancelled");
                }
                if (task.IsFaulted)
                {
                    Console.WriteLine($"\n\tTask {taskName} Failed");
                }
            }
        }
    }
}
