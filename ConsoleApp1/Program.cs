using System.Diagnostics;
using Newtonsoft.Json;

namespace ConsoleApp1;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        //TextCrc16();

        //TestFloat();

        //TestBitConverter();

        //TestJson();

        //WatchByteInStream();

        TestTask();
    }

    private static void TestTask()
    {
        CancellationTokenSource source = new();

        var t = Task.Run(async delegate
        {
            await Task.Delay(TimeSpan.FromMilliseconds(5000), source.Token);
            return 42;
        }, source.Token);
        source.Cancel();

        try
        {
            t.Wait();
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
                Console.WriteLine("{0}: {1}", e.GetType().FullName, e.Message);
        }

        Console.Write("Task t Status: {0}", t.Status);
        if (t.Status == TaskStatus.RanToCompletion)
            Console.Write(", Result: {0}", t.Result);
        source.Dispose();
    }

    private static void TestJson()
    {
        SayHello();

        var myTask = new MyTask
        {
            RW = "2021年度下水道定期检修",
            FX = "南北方向",
            DD = "工业园区星湖街328号",
            RQ = "2021年08月10号",
            QS = "#WS504",
            ZZ = "#WS506",
            GJ = "300mm",
            GC = "波纹管",
            LX = "污水管道",
            DW = "苏州蛟视管道检测技术有限公司",
            YU = "Alex"
        };

        //序列化
        var json = JsonConvert.SerializeObject(myTask);
        json.PrintYellow();

        //反序列化
        var soul = JsonConvert.DeserializeObject<MyTask>(json);
        soul?.DD.PrintErr();
    }

    private class MyTask
    {
        public string RW;
        public string FX;
        public string DD;
        public string RQ;
        public string QS;
        public string ZZ;
        public string GJ;
        public string GC;
        public string LX;
        public string DW;
        public string YU;
    }

    private static void WatchByteInStream()
    {
        byte[] data = { 0x06, 0x87 };

        Console.WriteLine("原数据:");
        //Array.Reverse(data); //反转数组转成大端。
        Console.WriteLine(BitConverter.ToString(data));

        Console.WriteLine("还原为C#识别的小端字节序:");
        Array.Reverse(data); //还原为小端字节序
        Console.WriteLine(BitConverter.ToString(data));

        Console.WriteLine("还原数字:" + BitConverter.ToUInt16(data));
    }

    private static void TestBitConverter()
    {
        SayHello();

        Console.WriteLine("Now we do convert bool to byte[]:");
        const bool y = false;
        Console.WriteLine("raw data:" + y);

        var data = BitConverter.GetBytes(y); //得到小端字节序数组
        Console.WriteLine(BitConverter.ToString(data));

        Console.WriteLine("反转成传输用大端:");
        Array.Reverse(data); //反转数组转成大端。
        Console.WriteLine(BitConverter.ToString(data));

        Console.WriteLine("还原为C#识别的小端字节序:");
        Array.Reverse(data); //还原为小端字节序
        Console.WriteLine(BitConverter.ToString(data));

        Console.WriteLine("还原数字:" + BitConverter.ToBoolean(data));
    }

    private static void TextCrc16()
    {
        SayHello();

        Console.WriteLine("Now we do CRC16 check:");

        byte[] data =
            { 0x24, 0x43, 0x4D, 0x44, 0x01, 0x01, 0x01, 0x64, 0xF7, 0x76, 0xF7, 0x41, 0x54, 0xD9, 0x08, 0x00 };
        Console.WriteLine("raw data:" + BitConverter.ToString(data));

        data = Crc16Util.CRC16(data);
        Console.WriteLine("crc data:" + BitConverter.ToString(data));

        var yes = Crc16Util.CheckCRC16(data);

        Console.WriteLine("CRC16 Check:" + yes);
        Console.WriteLine();
    }

    private static void SayHello()
    {
        var frames = new StackTrace().GetFrames();

        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine($"- method name: {frames[1].GetMethod()?.Name}");
        Console.WriteLine("----------------------------------------------------------------------");
    }

    private static void TestFloat()
    {
        SayHello();

        #region Now we convert a int to byte[]

        Console.WriteLine("Now we convert a int to byte[]:");
        var x = -6;
        Console.WriteLine($"原数字:{x}");

        Console.WriteLine("BitConverter.GetBytes() 默认得到小端字节序数组:");
        var aa = BitConverter.GetBytes(x); //得到小端字节序数组
        Console.WriteLine(BitConverter.ToString(aa));

        Console.WriteLine("反转成传输用大端:");
        Array.Reverse(aa); //反转数组转成大端。
        Console.WriteLine(BitConverter.ToString(aa));

        Console.WriteLine("还原为C#识别的小端字节序:");
        Array.Reverse(aa); //还原为小端字节序
        Console.WriteLine(BitConverter.ToString(aa));

        Console.WriteLine("还原数字:" + BitConverter.ToInt32(aa));

        #endregion

        Console.WriteLine();

        #region Now we convert a float to byte[]

        Console.WriteLine("Now we convert a float to byte[]:");
        var z = 2500000f;
        Console.WriteLine($"原数字 float:{z}");

        Console.WriteLine("BitConverter.GetBytes() 默认得到小端字节序数组:");
        var cc = BitConverter.GetBytes(z);
        Console.WriteLine(BitConverter.ToString(cc));

        Console.WriteLine("反转成传输用大端:");
        Array.Reverse(cc); //反转数组转成大端。
        Console.WriteLine(BitConverter.ToString(cc));

        Console.WriteLine("还原为C#识别的小端字节序:");
        Array.Reverse(cc); //还原为小端字节序
        Console.WriteLine(BitConverter.ToString(cc));

        Console.WriteLine("And we convert the byte[] to float:");
        Console.WriteLine("还原数字:" + BitConverter.ToSingle(cc, 0));

        #endregion
    }
}