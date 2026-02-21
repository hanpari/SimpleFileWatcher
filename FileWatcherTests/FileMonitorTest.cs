namespace FileWatcherTests;

using Hanpari.Monitor;

public class UnitTest1
{
    [Fact]
    public void BasicWorkflow()
    {
        var path = Path.GetTempFileName();
        var fileInfo = new FileInfo(path);

        if (fileInfo.Exists) fileInfo.Delete();

        Monitor fileMonitor = new(path);

        Assert.IsType<Status.MonitorStarted>(fileMonitor.getChangedStatus());

        Assert.IsType<Status.NotCreatedYet>(fileMonitor.getChangedStatus());

        Assert.Null(fileMonitor.getChangedStatus());
        Assert.Null(fileMonitor.getChangedStatus());

        using (fileInfo.Create()) { }
        ;

        Assert.IsType<Status.Created>(fileMonitor.getChangedStatus());

        Assert.Null(fileMonitor.getChangedStatus());
        Assert.Null(fileMonitor.getChangedStatus());

        Thread.Sleep(1000);

        using (var f = fileInfo.CreateText())
        {
            f.WriteLine("line");
        }
        ;

        Assert.IsType<Status.LastWritten>(fileMonitor.getChangedStatus());

        Assert.Null(fileMonitor.getChangedStatus());
        Assert.Null(fileMonitor.getChangedStatus());
    }
}
