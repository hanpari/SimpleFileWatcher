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

        Assert.IsType<Status.MonitorStarted>(fileMonitor.GetChangedStatus());

        Assert.IsType<Status.NotCreatedYet>(fileMonitor.GetChangedStatus());

        Assert.Null(fileMonitor.GetChangedStatus());
        Assert.Null(fileMonitor.GetChangedStatus());

        using (fileInfo.Create()) { }
        ;

        Assert.IsType<Status.Created>(fileMonitor.GetChangedStatus());

        Assert.Null(fileMonitor.GetChangedStatus());
        Assert.Null(fileMonitor.GetChangedStatus());

        Thread.Sleep(1000);

        using (var f = fileInfo.CreateText())
        {
            f.WriteLine("line");
        }
        ;

        Assert.IsType<Status.LastWritten>(fileMonitor.GetChangedStatus());

        Assert.Null(fileMonitor.GetChangedStatus());
        Assert.Null(fileMonitor.GetChangedStatus());
    }
}
