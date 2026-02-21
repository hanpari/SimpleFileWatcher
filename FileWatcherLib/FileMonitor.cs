namespace Hanpari.FileMonitor;

public class Monitor
{
    public FileInfo MonitoredFile { get; private set; }
    private Status lastStatus;

    public Monitor(string path)
    {
        MonitoredFile = new FileInfo(path);
        lastStatus = new Status.StatusInitiator() { Moment = DateTime.UtcNow };
    }

    public static Monitor? CreateOnlyForExistingFile(string path)
    {
        return File.Exists(path) ? new Monitor(path) : null;
    }

    public Status? getChangedStatus()
    {
        MonitoredFile.Refresh();
        var newStatus = lastStatus.Refresh(MonitoredFile);
        return newStatus == lastStatus ? null : StoreStatusAndGetIt(newStatus);
    }

    Status StoreStatusAndGetIt(Status status)
    {
        lastStatus = status;
        return status;
    }
}