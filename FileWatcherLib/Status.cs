namespace Hanpari.FileMonitor;

public abstract record Status
{
    static readonly double ALLOWED_DELTA_MILLISECONDS = 500;
    private DateTime _moment;

    public required DateTime Moment
    {
        get => _moment;
        init
        {
            if (value.Kind is not DateTimeKind.Utc)
                throw new ArgumentException("Moment must be UTC");
            _moment = value;
        }
    }

    public abstract Status Refresh(FileInfo fileInfo);
    Status() { }

    protected bool isMomentEqualTo(DateTime newMoment)
    {
        return AreMomentsEqual(Moment, newMoment);
    }

    protected static bool AreMomentsEqual(DateTime oldMoment, DateTime newMoment)
    {
        var difference = Math.Abs((newMoment - oldMoment).TotalMilliseconds);
        return difference < ALLOWED_DELTA_MILLISECONDS;
    }

    public record StatusInitiator : Status
    {
        public override Status Refresh(FileInfo fileInfo)
        {
            return new MonitorStarted() { Moment = Moment };
        }
    }
    public record MonitorStarted : Status
    {
        public override Status Refresh(FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                var (creationTime, writeTime) = (fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc);
                return (AreMomentsEqual(creationTime, writeTime)) ?
                        new Created() { Moment = creationTime } :
                        new LastWritten() { Moment = writeTime };
            }
            return new NotCreatedYet() { Moment = DateTime.UtcNow };
        }
    }
    public record Created() : Status
    {
        public override Status Refresh(FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                var writeTime = fileInfo.LastWriteTimeUtc;
                return isMomentEqualTo(writeTime) ?
                        this :
                        new LastWritten() { Moment = writeTime };
            }
            return new DeletedAfter() { Moment = Moment };
        }
    }

    public record LastWritten() : Status
    {
        public override Status Refresh(FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                return isMomentEqualTo(fileInfo.LastWriteTimeUtc) ?
                        this :
                        new LastWritten() { Moment = fileInfo.LastWriteTimeUtc };
            }
            return new DeletedAfter() { Moment = Moment };
        }
    }

    public record DeletedAfter() : Status
    {
        public override Status Refresh(FileInfo fileInfo) =>
                        fileInfo.Exists ?
                        new Created() { Moment = fileInfo.CreationTimeUtc } :
                        this;
    }

    public record NotCreatedYet : Status
    {
        public override Status Refresh(FileInfo fileInfo) =>
                        fileInfo.Exists ?
                        new Created() { Moment = fileInfo.CreationTimeUtc } :
                        this;
    }
}

