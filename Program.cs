using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"\nPlease Enter C to Cancel");


            var cancellationToken = new CancellationTokenSource();
            var task = Task.Run(() =>
             {
                 while (true)
                 {
                     if (cancellationToken.IsCancellationRequested)
                     {
                         Console.WriteLine($"\nCancelled Successfully!");
                         break;
                     }
                     Console.WriteLine($"\nI should stop printing the moment you Cancel...");
                     Thread.Sleep(1000);
                 }
             }, cancellationToken.Token);

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
    }
}
