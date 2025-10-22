using System;
using System.Threading;

namespace Synchronization.ProducerConsumer;

public class SharedBuffer
{
    private readonly int[] _buffer = new int[10];
    private int _numberOfOccupiedElements;
    private int _elementIndexToPut;
    private int _elementIndexToTakeFrom;

    private readonly object _locker = new();

    public void PutElement(int element)
    {
        var managedThreadId = Environment.CurrentManagedThreadId;
        lock (_locker)
        {
            while (_numberOfOccupiedElements == _buffer.Length)
            {
                Console.WriteLine($"Thread {managedThreadId} is waiting");
                _ = Monitor.Wait(_locker);
            }

            _buffer[_elementIndexToPut] = element;
            _numberOfOccupiedElements++;
            _elementIndexToPut = (_elementIndexToPut + 1) % _buffer.Length;

            for (var i = 0; i < _buffer.Length; i++)
                Console.Write("{0:d2} ", _buffer[i]);
            Console.WriteLine(" ");

            Monitor.Pulse(_locker);
        }
    }


    public int GetElement()
    {
        var managedThreadId = Environment.CurrentManagedThreadId;

        lock (_locker)
        {
            while (_numberOfOccupiedElements == 0)
            {
                Console.WriteLine($"Thread {managedThreadId} is waiting");
                _ = Monitor.Wait(_locker);
            }

            var element = _buffer[_elementIndexToTakeFrom];
            _buffer[_elementIndexToTakeFrom] = 0;
            _numberOfOccupiedElements--;
            _elementIndexToTakeFrom = (_elementIndexToTakeFrom + 1) % _buffer.Length;

            for (var i = 0; i < _buffer.Length; i++)
            {
                Console.Write($"{_buffer[i]:d2} ");
            }

            Console.WriteLine("");

            Monitor.Pulse(_locker);

            return element;
        }
    }
}