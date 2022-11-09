using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Mar.Console;

namespace App08.Benchmark;

public class StackTrace2
{
    public IEnumerable<string> Contents()
    {
        yield return Guid.NewGuid().ToString();
        yield return Guid.NewGuid().ToString();
        yield return Guid.NewGuid().ToString();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Contents))]
    public void PrintCaller(string content)
    {
        var frames = new StackTrace().GetFrames();
        var caller = frames?.Length < 2 ? "Null" : frames?[1].GetMethod()?.Name;
        $"{caller}\t{content}".PrintMagenta();
    }
}