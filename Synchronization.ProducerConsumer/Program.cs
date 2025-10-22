using Synchronization.ProducerConsumer;

class Program
{
    private static readonly SharedBuffer s_buffer = new();
    private static readonly Producer s_producer = new(s_buffer);
    private static readonly Consumer s_consumer = new(s_buffer);

    public static async Task Main()
    {
        var t1 = Task.Run(s_producer.Produce);
        var t2 = Task.Run(s_consumer.Consume);

        await Task.WhenAll(t1, t2);
    }
}