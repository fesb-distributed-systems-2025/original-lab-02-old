using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private const int IncrementAmount = 10_000;
    private static volatile int s_volatileSum;
    private static int s_interlockedSum;
    private static int s_normalSum;

    private static void IncrementSums()
    {
        for (var i = 0; i < IncrementAmount; i++)
        {
            s_volatileSum++;
            _ = Interlocked.Increment(ref s_interlockedSum);
            s_normalSum++;
        }
    }

    public static async Task Main()
    {
        var numberOfCores = Environment.ProcessorCount;
        Console.WriteLine($"You have {numberOfCores} cores installed.");

        var tasks = new List<Task>();

        foreach (var _ in Enumerable.Range(0, numberOfCores))
        {
            var task = Task.Run(IncrementSums);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"Volatile sum: {s_volatileSum}");
        Console.WriteLine($"Interlocked sum: {s_interlockedSum}");
        Console.WriteLine($"Normal sum: {s_normalSum}");
    }
}


