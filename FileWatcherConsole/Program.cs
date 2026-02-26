using Hanpari.Monitor;
using static System.Console;

var hub = new AsyncStreamHub<(FileInfo, Status)>();

Hanpari.Monitor.Monitor m1 = new("test1");
Hanpari.Monitor.Monitor m2 = new("test2");

var id1 = hub.Add(m1.WatchEvery());
var id2 = hub.Add(m2.WatchEvery());

Clear();

await foreach (var (f, status) in hub.Output)
{
    WriteLine(f);
    WriteLine(status);
}
