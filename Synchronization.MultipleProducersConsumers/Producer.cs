using System.Threading.Channels;

public class Producer
{
    private readonly Random _random = new();
    private readonly string _name;
    private readonly ChannelWriter<int> _channelWriter;

    public Producer(string name, ChannelWriter<int> writer)
    {
        _name = name;
        _channelWriter = writer;
    }

    public async Task Produce()
    {
        for (var i = 0; i < 100; i++)
        {
            var delay = TimeSpan.FromMilliseconds(_random.Next(1, 200));
            await Task.Delay(delay);
            await _channelWriter.WriteAsync(i);
            Console.WriteLine($"Producer {_name} wrote {i} to channel.");
        }
    }
}