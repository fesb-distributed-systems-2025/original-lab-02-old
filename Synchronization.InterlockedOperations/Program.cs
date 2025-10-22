using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static int s_locker = 0;
    private static int s_total = 0;
    private static void AddNumbers()
    {
        var threadId = Environment.CurrentManagedThreadId;
        while (Interlocked.Exchange(ref s_locker, 1) != 0)
        {
            Console.WriteLine($"Thread {threadId} is waiting for lock to release");
            Thread.Sleep(300);
        }

        Console.WriteLine($"Thread {threadId} has acquired lock");

        s_total += 5;
        Console.WriteLine("total = " + s_total);

        s_total -= 2;
        Console.WriteLine("total = " + s_total);

        s_total *= 2;
        Console.WriteLine("total = " + s_total);

        Console.WriteLine($"Thread {threadId} is about to release lock");
        Thread.VolatileWrite(ref s_locker, 0);
    }

    public static async Task Main()
    {
        var tasks = new[]
        {
            Task.Run(() => AddNumbers()),
            Task.Run(() => AddNumbers()),
            Task.Run(AddNumbers),
        };

        await Task.WhenAll(tasks);
    }
}