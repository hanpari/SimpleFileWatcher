using Microsoft.VisualBasic;
using static System.Console;

Clear();

var mon = FileChangeMonitor.Create(@"c:\\data\test.txt");
