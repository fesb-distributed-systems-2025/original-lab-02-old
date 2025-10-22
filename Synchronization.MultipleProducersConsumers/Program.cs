using System.Threading.Channels;

class Program
{
    private static readonly Channel<int> s_channel = Channel.CreateBounded<int>(new BoundedChannelOptions(10)
    {
        FullMode = BoundedChannelFullMode.Wait,
    });

    public static async Task Main()
    {
        var producers = new[]
        {
            new Producer("Producer1", s_channel.Writer),
            new Producer("Producer2",s_channel.Writer),
            new Producer("Producer3",s_channel.Writer),
        };

        var producerTasks = new List<Task>();

        foreach (var producer in producers)
        {
            var producerTask = producer.Produce();
            producerTasks.Add(producerTask);
        }

        var consumers = new[]
        {
            new Consumer("Consumer1", s_channel.Reader),
            new Consumer("Consumer2", s_channel.Reader),
        };

        var consumerTasks = new List<Task<int>>();
        foreach (var consumer in consumers)
        {
            var consumerTask = consumer.Consume();
            consumerTasks.Add(consumerTask);
        }

        await Task.WhenAll(producerTasks);

        s_channel.Writer.Complete();

        var sum = 0;
        foreach (var consumerTask in consumerTasks)
        {
            sum += await consumerTask;
        }

        System.Console.WriteLine($"Expected sum: {99 * 100 / 2 * producers.Length}");
        System.Console.WriteLine($"Actual sum: {sum}");
    }
}