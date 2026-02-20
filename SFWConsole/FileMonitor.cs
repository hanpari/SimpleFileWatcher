namespace Hanpari.FileMonitor;

public class FileMonitor
{
    public FileInfo MonitoredFile { get; private set; }
    private Status lastStatus;

    public FileMonitor(string path)
    {
        MonitoredFile = new FileInfo(path);
        lastStatus = new Status.StatusInitiator() { Moment = DateTime.UtcNow };
    }

    public static FileMonitor? CreateOnlyForExistingFile(string path)
    {
        return File.Exists(path) ? new FileMonitor(path) : null;
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