using Synchronization.Mutexes;

class Program
{
    private static readonly Account s_firstAccount = new("First", 200);
    private static readonly Account s_secondAccount = new("Second", 300);

    public static async Task Main()
    {
        var t1 = Task.Run(() => s_firstAccount.Transfer(s_secondAccount, 100));
        var t2 = Task.Run(() => s_firstAccount.Transfer(s_secondAccount, 100));
        // var t2 = Task.Run(() => s_secondAccount.Transfer(s_firstAccount, 100));

        await Task.WhenAll(t1, t2);

        Console.WriteLine("Balances: {0} {1}", s_firstAccount, s_secondAccount);
    }
}