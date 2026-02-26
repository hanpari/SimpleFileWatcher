namespace Hanpari.Monitor;

public class Monitor
{
    public FileInfo MonitoredFile { get; private set; }
    private Status lastStatus;
    private bool Active = true;


    public Monitor(string path)
    {
        MonitoredFile = new FileInfo(path);
        lastStatus = new Status.StatusInitiator() { Moment = DateTime.UtcNow };
    }

    public static Monitor? CreateOnlyForExistingFile(string path)
    {
        return File.Exists(path) ? new Monitor(path) : null;
    }

    public Status? GetChangedStatus()
    {
        MonitoredFile.Refresh();
        var newStatus = lastStatus.Refresh(MonitoredFile);
        return newStatus == lastStatus ? null : StoreStatusAndGetItBack(newStatus);
    }

    public async IAsyncEnumerable<(FileInfo, Status)> WatchEvery(int Delay = 1000)
    {
        while (Active)
        {
            var status = GetChangedStatus();
            if (status is not null) yield return (MonitoredFile, status);
            await Task.Delay(Delay);
        }
    }

    Status StoreStatusAndGetItBack(Status status)
    {
        lastStatus = status;
        return status;
    }

    void Stop() => Active = false;
}