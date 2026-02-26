
namespace Hanpari.Monitor;

using System.Collections.Concurrent;
using System.Threading.Channels;

public class AsyncStreamHub<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _streams = new();

    public IAsyncEnumerable<T> Output => _channel.Reader.ReadAllAsync();

    public Guid Add(IAsyncEnumerable<T> source)
    {
        var id = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        _streams[id] = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var item in source.WithCancellation(cts.Token))
                {
                    await _channel.Writer.WriteAsync(item, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // expected on removal
            }
            finally
            {
                _streams.TryRemove(id, out _);
            }
        });

        return id;
    }

    public void Remove(Guid id)
    {
        if (_streams.TryGetValue(id, out var cts))
        {
            cts.Cancel();
        }
    }

    public void Complete()
    {
        foreach (var (_, cts) in _streams)
            cts.Cancel();

        _channel.Writer.Complete();
    }
}
