﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Mar.Console;

namespace ConsoleApp3;

internal class Program
{
    private static void Main(string[] args)
    {
        "Hello World!".PrintGreen();

        DoStuff();

        "Press any key to exit...".PrintGreen();
        Console.ReadKey(true);
    }

    private static void DoStuff()
    {
        var task = GetLocalIp();

        // Set up a continuation BEFORE MainWorkOfApplicationIDontWantBlocked
        var anotherTask = task.ContinueWith(r => { $"看到这句话代表上个任务已经完成, 上个任务的结果:{r.Result}".PrintGreen(); });

        MainWorkOfApplicationIDontWantBlocked();

        // OR wait for the result AFTER
        var result = task.Result;
        $"阻塞进程等待结果：{result}".PrintGreen();
    }

    private static void MainWorkOfApplicationIDontWantBlocked()
    {
        const string tang = "门前大桥下，\n游过一群鸭，\n快来快来数一数，\n二四六七八";
        foreach (var cha in tang.ToCharArray())
        {
            Console.Write(cha);
            Thread.Sleep(500);
        }
    }

    private static async Task<string> GetNameAsync()
    {
        var firstname = await PromptForStringAsync("Enter your first name: ");
        var lastname = await PromptForStringAsync("Enter your last name: ");
        return firstname + lastname;
    }

    // contrived example (edited in response to Servy's comment)
    private static Task<string> PromptForStringAsync(string prompt)
    {
        return Task.Factory.StartNew(() =>
        {
            prompt.PrintGreen();
            return Console.ReadLine();
        });
    }

    private static Task<string> GetLocalIp()
    {
        "using GetLocalIp to get main ip.".PrintGreen();
        return Task.Factory.StartNew(() =>
        {
            "RunApp works".PrintGreen();
            var task = RunApp("route", "print");
            var result = task.Result;
            var match = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (match.Success) return match.Groups[2].Value;

            "TcpClient works".PrintGreen();
            try
            {
                var tcpClient = new TcpClient();
                tcpClient.Connect("www.baidu.com", 80);
                var ip = ((IPEndPoint)tcpClient.Client.LocalEndPoint)?.Address.ToString();
                tcpClient.Close();
                return ip;
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        });
    }

    private static Task<string> RunApp(string filename, string arguments)
    {
        "RunApp start".PrintGreen();
        return Task.Factory.StartNew(() =>
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = filename,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();

            var streamReader = new StreamReader(process.StandardOutput.BaseStream, Encoding.Default);
            var result = streamReader.ReadToEnd();
            process.WaitForExit();
            streamReader.Close();
            Thread.Sleep(5000); // Too fast to feel
            "RunApp end".PrintGreen();
            return result;
        });
    }
}