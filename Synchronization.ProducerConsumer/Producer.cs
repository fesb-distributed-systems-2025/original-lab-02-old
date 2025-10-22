using Synchronization.ProducerConsumer;

public class Producer
{
    private readonly SharedBuffer _buffer;
    private readonly Random _random = new();

    public Producer(SharedBuffer buffer)
    {
        _buffer = buffer;
    }

    public void Produce()
    {
        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(_random.Next(1, 200));
            _buffer.PutElement(i);
        }
    }
}