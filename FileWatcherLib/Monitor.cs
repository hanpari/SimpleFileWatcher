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

    public Status? GetChangedStatus() => Refresh() ? lastStatus : null;

    /// <summary>
    /// Updates the monitored file and evaluates whether its status has changed
    /// compared to the previously recorded state.
    /// </summary>
    /// 
    /// <remarks>
    /// The method first refreshes the metadata of the monitored file using
    /// <see cref="MonitoredFile.Refresh"/>. It then obtains an updated status
    /// by calling <c>lastStatus.Refresh(MonitoredFile)</c>. If the newly
    /// determined status differs from the previously stored one, the method
    /// updates the internal state and returns <c>true</c> to indicate that a
    /// change has occurred.
    /// </remarks>
    /// 
    /// <returns>
    /// <c>true</c> if the monitored file's status has changed; otherwise,
    /// <c>false</c>.
    /// </returns>
    public bool Refresh()
    {
        MonitoredFile.Refresh();
        var status = lastStatus.Refresh(MonitoredFile);
        if (status != lastStatus)
        {
            lastStatus = status;
            return true;
        }
        return false;
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

    void Stop() => Active = false;
}