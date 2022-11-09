// See https://aka.ms/new-console-template for more information

using App08.Benchmark;
using BenchmarkDotNet.Running;
using Mar.Console;

"Hello, World!".PrintGreen();

var summary = BenchmarkRunner.Run<StackTrace2>();