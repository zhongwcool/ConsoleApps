using System.Diagnostics;
using Mar.Console;

namespace App07.Run;

internal static class Program
{
    private static System.Timers.Timer _watchTimer;

    private static void Main(string[] args)
    {
        "Hello World!".PrintMagenta();

        _watchTimer = new System.Timers.Timer();
        _watchTimer.AutoReset = true;
        _watchTimer.Interval = 5000;
        _watchTimer.Elapsed += WatchTimerOnTick;
        _watchTimer.Start();

        var tik = new System.Timers.Timer();
        tik.AutoReset = true;
        tik.Interval = 1000;
        tik.Elapsed += (_, _) =>
        {
            _watchTimer.Interval = 5000;
            _watchTimer.Start();
        };
        tik.Start();

        "Across the great wall, we can get reach every corner in the world".PrintMagenta();
        Console.Read();
    }

    private static void WatchTimerOnTick(object? sender, EventArgs e)
    {
        "tik tok".PrintGreen();
    }

    private static void SayHello()
    {
        var frames = new StackTrace().GetFrames();

        "----------------------------------------------------------------------".PrintGreen();
        $"- method name: {frames[1].GetMethod()?.Name}".PrintGreen();
        "----------------------------------------------------------------------".PrintGreen();
    }
}