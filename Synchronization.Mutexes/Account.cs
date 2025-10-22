using System;
using System.Threading;

namespace Synchronization.Mutexes;

public class Account
{
    private const int Timeout = 10000;
    private readonly string _name;
    private decimal _balance;
    private readonly Mutex _mutex = new();

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
        return $"Account: {_name} Ballance: {_balance}";
    }

    public void Transfer(Account other, decimal amount)
    {
        if (other is null)
        {
            return;
        }

        var managedThreadId = Environment.CurrentManagedThreadId;

        Mutex[] lockers = { _mutex, other._mutex };

        if (WaitHandle.WaitAll(lockers, Timeout))
        {
            try
            {
                Console.WriteLine($"Account {_name} T{managedThreadId}: All locks are acquired.");

                Withdraw(amount);
                Thread.Sleep(1000);
                other.Deposit(amount);

                Console.WriteLine($"Account {_name} T{managedThreadId}: Transfer is performed.");
            }
            finally
            {
                foreach (var mutex in lockers)
                {
                    mutex.ReleaseMutex();
                }

                Console.WriteLine($"Account {_name} T{managedThreadId}: All locks are released.");
            }
        }
    }
}