using static System.Console;
using Hanpari.FileMonitor;

Clear();

var path = Path.GetTempFileName();
var fi = new FileInfo(path);
if (fi.Exists) fi.Delete();

FileMonitor fm = new(path);

WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus() is null);

using (fi.Create()) { }
;

WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus() is null);

Thread.Sleep(1000);

using var f = fi.CreateText();
f.WriteLine("line");
f.Close();


WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus());
WriteLine(fm.getChangedStatus() is null);





