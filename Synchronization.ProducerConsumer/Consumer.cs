using Synchronization.ProducerConsumer;

public class Consumer
{
    private readonly SharedBuffer _buffer;
    private readonly Random _random = new();

    public Consumer(SharedBuffer buffer)
    {
        _buffer = buffer;
    }

    public void Consume()
    {
        var sum = 0;

        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(_random.Next(1, 500));
            sum += _buffer.GetElement();
        }

        Console.WriteLine("Sum should be " + (99 * 100 / 2));
        Console.WriteLine("Thank you kleine Karl: " + sum);
    }
}