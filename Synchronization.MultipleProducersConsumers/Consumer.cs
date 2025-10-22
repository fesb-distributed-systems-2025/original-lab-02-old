using System.Threading.Channels;

public class Consumer
{
    private readonly Random _random = new();
    private readonly string _name;
    private readonly ChannelReader<int> _channelReader;

    public Consumer(string name, ChannelReader<int> channelReader)
    {
        _name = name;
        _channelReader = channelReader;
    }

    public async Task<int> Consume()
    {
        var sum = 0;

        await foreach (var data in _channelReader.ReadAllAsync())
        {
            var delay = TimeSpan.FromMilliseconds(_random.Next(1, 500));
            await Task.Delay(delay);
            sum += data;
            Console.WriteLine($"Consumer {_name} read {data} from channel.");
        }

        return sum;
    }
}