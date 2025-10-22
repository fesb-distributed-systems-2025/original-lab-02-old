class Program
{
    private static readonly AutoResetEvent s_autoRaiseEvent = new(false);
    private static readonly Semaphore s_semaphore = new(1, 2);
    private static readonly Random s_random = new();
    private static readonly object s_lock = new();
    private static void DoSomething()
    {
        Console.WriteLine("Waiting for auto raise event to be signaled ...");
        _ = s_autoRaiseEvent.WaitOne();
        Console.WriteLine("autoRaiseEvent is signaled.");
    }
    private static async Task DoConcurrentOperationWithSemaphore()
    {
        var managedThreadId = Environment.CurrentManagedThreadId;

        try
        {
            Console.WriteLine($"Thread {managedThreadId} is waiting to enter semaphore");

            _ = s_semaphore.WaitOne();

            Console.WriteLine($"Thread {managedThreadId} entered semaphore");

            await Task.Delay(TimeSpan.FromSeconds(1));

            var randomNumber = s_random.Next(0, 3);

            if (randomNumber == 1)
            {
                throw new NotSupportedException($"Uh oh, bad thing happened in thread {managedThreadId}");
            }
        }
        catch (NotSupportedException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception occurred on thread {managedThreadId}");
            Console.ResetColor();
        }
        finally
        {
            Console.WriteLine($"Thread {managedThreadId} is about to exit semaphore");
            _ = s_semaphore.Release();
        }
    }

    private static void DoConcurrentOperationWithLock()
    {
        var managedThreadId = Environment.CurrentManagedThreadId;

        Console.WriteLine($"Thread {managedThreadId} is waiting to enter lock");
        lock (s_lock)
        {
            Console.WriteLine($"Thread {managedThreadId} entered lock");

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        Console.WriteLine($"Thread {managedThreadId} left lock");

    }

    public static async Task Main()
    {
        var autoRaiseEventTask = Task.Run(DoSomething);

        Console.WriteLine("Press any key to signal raise autoRaiseEvent signal ...");
        _ = Console.ReadKey();

        _ = s_autoRaiseEvent.Set();

        await autoRaiseEventTask;

        var semaphoreTasks = new[]
        {
            Task.Run(DoConcurrentOperationWithSemaphore),
            Task.Run(DoConcurrentOperationWithSemaphore),
            Task.Run(DoConcurrentOperationWithSemaphore),
            Task.Run(DoConcurrentOperationWithSemaphore),
            Task.Run(() => DoConcurrentOperationWithSemaphore()),
        };
        await Task.WhenAll(semaphoreTasks);

        var lockTasks = new[]
        {
            Task.Run(DoConcurrentOperationWithLock),
            Task.Run(DoConcurrentOperationWithLock),
            Task.Run(() => DoConcurrentOperationWithLock()),
        };
        await Task.WhenAll(lockTasks);

        s_autoRaiseEvent.Dispose();
        s_semaphore.Dispose();
    }
}