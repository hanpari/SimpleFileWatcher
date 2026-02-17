using static System.Console;

public class FileChangeMonitor
{
    public required FileInfo fileInfo { get; init; }
    private DateTime lastWriteTime;

    private bool isMonitoringOn = false;
    private PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
    private FileChangeMonitor()
    {

    }

    public async Task StartAsync(long seconds = 0)
    {
        WriteLine($"Monitor {fileInfo}");
        lastWriteTime = fileInfo.LastWriteTimeUtc;
        timer = seconds is 0 ? timer : new PeriodicTimer(TimeSpan.FromSeconds(seconds));
        await MonitorAsync(true);
    }

    private async Task MonitorAsync(bool On)
    {
        while (On && await timer.WaitForNextTickAsync())
        {
            WriteLine(WasFileChanged());
        }
    }

    public bool WasFileChanged()
    {
        fileInfo.Refresh();
        if (!fileInfo.Exists)
        {
            StopAsync();
            return true;
        }
        if (lastWriteTime == fileInfo.LastWriteTimeUtc) return false;
        lastWriteTime = fileInfo.LastWriteTime;
        return true;
    }

    public async void StopAsync()
    {
        await MonitorAsync(false);
        WriteLine($"Monitoring stopped for {fileInfo}");
    }

    public static FileChangeMonitor? Create(string path)
    {
        var fi = new FileInfo(path);
        if (fi.Exists)
        {
            var se = new FileChangeMonitor() { fileInfo = new FileInfo(path) };
            return se;
        }
        return null;
    }
}