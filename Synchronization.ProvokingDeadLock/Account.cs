public class Account
{
    private object _locker = new();
    private readonly string _name;
    private decimal _balance;

    public Account(string name, decimal balance)
    {
        _name = name;
        _balance = balance;
    }

    public void Withdraw(decimal amount)
    {
        _balance -= amount;
    }

    public void Deposit(decimal amount)
    {
        _balance += amount;
    }

    public override string ToString()
    {
        return $"Ballance: {_balance}";
    }

    public void Transfer(Account other, decimal amount)
    {
        if (other is null)
        {
            return;
        }

        var managedThreadId = Environment.CurrentManagedThreadId;

        lock (_locker)
        {
            Console.WriteLine($"Account {_name} Thread {managedThreadId} acquired lock 1.");
            Thread.Sleep(1000);

            Console.WriteLine($"Account {_name} Thread {managedThreadId} trying to acquire lock 2.");

            lock (other._locker)
            {
                Console.WriteLine($"Account {_name} Thread {managedThreadId} acquired lock 2.");

                Withdraw(amount);
                other.Deposit(amount);
                Console.WriteLine($"Account {_name} Thread {managedThreadId} performed transfer and is releasing lock 2.");
            }

            Console.WriteLine($"Thread {managedThreadId} is releasing 1.");
        }
    }
}
