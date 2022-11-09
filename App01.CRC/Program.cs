using System.Collections.ObjectModel;
using System.Diagnostics;
using Mar.Console;
using Newtonsoft.Json;

namespace App01.CRC;

internal static class Program
{
    private static void Main(string[] args)
    {
        "Hello World!".PrintMagenta();
        long a = 883114;
        long b = 889774;
        $"{a * 1.0f / b * 1.0f}".PrintErr();


        var url = "https://gitee.com/js-tech/iasystem-firmwares/raw/master/ias-wqm-a1/ias-wqm-a1.bin";
        var task = GetFileSize(url);
        task.ContinueWith(task1 => { $"文件大小：{task1.Result}".PrintErr(); });

        var ccbSlots = new ObservableCollection<CcbSlot>
        {
            new() { CcbAddr = 4, Ts = DateTime.Now }, new() { CcbAddr = 2, Ts = DateTime.Now },
            new() { CcbAddr = 3, Ts = DateTime.Now }, new() { CcbAddr = 4 }
        };
        var ccb2Slots = new ObservableCollection<CcbSlot>
        {
            new() { CcbAddr = 1, Ts = DateTime.Now }, new() { CcbAddr = 5, Ts = DateTime.Now },
            new() { CcbAddr = 3, Ts = DateTime.Now }, new() { CcbAddr = 4 }
        };

        Thread.Sleep(3 * 1000);
        var ts = DateTime.Now;
        var mSlot = ccbSlots.FirstOrDefault(t => (ts - t.Ts).Seconds > 2);
        while (mSlot != null)
        {
            ccbSlots.Remove(mSlot);
            mSlot = ccbSlots.FirstOrDefault(t => (ts - t.Ts).Seconds > 2);
        }

        foreach (var slot in ccbSlots)
        {
            $" {slot.CcbAddr}".PrintMagenta();
        }

        foreach (var slot in ccb2Slots)
        {
            $" {slot.CcbAddr}".PrintGreen();
        }


        //TextCrc16();

        //TestBitConverter();

        //TestJson();

        //WatchByteInStream();

        //TestTask();

        "Across the great wall, we can get reach every corner in the world".PrintMagenta();
        Console.Read();
    }

    class CcbSlot
    {
        public uint CcbAddr { get; set; }
        public bool Drop { get; set; }
        public DateTime Ts { get; set; }
    }

    private static async Task<long> GetFileSize(string url)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var fileSize = response.Content.ReadAsStreamAsync().Result.Length;
        return fileSize;
    }

    private static async Task<long> GetFileSizeAsync(string url)
    {
        long fileSize = -1;
        var client = new HttpClient();
        await Task.Run(() =>
        {
            Thread.Sleep(5 * 1000);
            var response = client.GetAsync(url);
            fileSize = response.Result.Content.ReadAsStreamAsync().Result.Length;
        });
        return fileSize;
    }

    private static void TestTask()
    {
        SayHello();

        CancellationTokenSource source = new();

        var t = Task.Run(async delegate
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50000), source.Token);
            return 42;
        }, source.Token);
        //source.Cancel();

        try
        {
            t.Wait();
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
                "e.GetType().FullName: e.Message".PrintGreen();
        }

        $"Task t Status: {t.Status}".PrintYellow();
        if (t.Status == TaskStatus.RanToCompletion)
            $"Result: {t.Result}".PrintYellow();
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

        "原数据:".PrintGreen();
        //Array.Reverse(data); //反转数组转成大端。
        BitConverter.ToString(data).PrintGreen();

        "还原为C#识别的小端字节序:".PrintGreen();
        Array.Reverse(data); //还原为小端字节序
        BitConverter.ToString(data).PrintGreen();

        ("还原数字:" + BitConverter.ToUInt16(data)).PrintGreen();
    }

    private static void TestBitConverter()
    {
        SayHello();

        Console.WriteLine();

        #region Now we convert a bool to byte[]

        "Now we do convert bool to byte[]:".PrintMagenta();
        const bool y = false;
        ("raw data:" + y).PrintGreen();

        var data = BitConverter.GetBytes(y); //得到小端字节序数组
        BitConverter.ToString(data).PrintGreen();

        "反转成传输用大端:".PrintGreen();
        Array.Reverse(data); //反转数组转成大端。
        BitConverter.ToString(data).PrintGreen();

        "还原为C#识别的小端字节序:".PrintGreen();
        Array.Reverse(data); //还原为小端字节序
        BitConverter.ToString(data).PrintGreen();

        ("还原数字:" + BitConverter.ToBoolean(data)).PrintGreen();

        #endregion

        Console.WriteLine();

        #region Now we convert a int to byte[]

        "Now we convert a int to byte[]:".PrintMagenta();
        var x = -6;
        $"原数字:{x}".PrintGreen();

        "BitConverter.GetBytes() 默认得到小端字节序数组:".PrintGreen();
        var aa = BitConverter.GetBytes(x); //得到小端字节序数组
        BitConverter.ToString(aa).PrintGreen();

        "反转成传输用大端:".PrintGreen();
        Array.Reverse(aa); //反转数组转成大端。
        BitConverter.ToString(aa).PrintGreen();

        "还原为C#识别的小端字节序:".PrintGreen();
        Array.Reverse(aa); //还原为小端字节序
        BitConverter.ToString(aa).PrintGreen();

        ("还原数字:" + BitConverter.ToInt32(aa)).PrintGreen();

        #endregion

        Console.WriteLine();

        #region Now we convert a float to byte[]

        "Now we convert a float to byte[]:".PrintMagenta();
        var z = 2500000f;
        $"原数字 float:{z}".PrintGreen();

        "BitConverter.GetBytes() 默认得到小端字节序数组:".PrintGreen();
        var cc = BitConverter.GetBytes(z);
        BitConverter.ToString(cc).PrintGreen();

        "反转成传输用大端:".PrintGreen();
        Array.Reverse(cc); //反转数组转成大端。
        BitConverter.ToString(cc).PrintGreen();

        "还原为C#识别的小端字节序:".PrintGreen();
        Array.Reverse(cc); //还原为小端字节序
        BitConverter.ToString(cc).PrintGreen();

        "And we convert the byte[] to float:".PrintGreen();
        ("还原数字:" + BitConverter.ToSingle(cc, 0)).PrintGreen();

        #endregion

        Console.WriteLine();

        #region Now we convert a short to byte[]

        "Now we convert a short to byte[]:".PrintMagenta();
        const short s = -180;
        $"原数字 short:{s}".PrintGreen();

        "BitConverter.GetBytes() 默认得到小端字节序数组:".PrintGreen();
        var ss = BitConverter.GetBytes(s);
        BitConverter.ToString(ss).PrintGreen();

        "反转成传输用大端:".PrintGreen();
        Array.Reverse(ss); //反转数组转成大端。
        BitConverter.ToString(ss).PrintGreen();

        "还原为C#识别的小端字节序:".PrintGreen();
        Array.Reverse(ss); //还原为小端字节序
        BitConverter.ToString(ss).PrintGreen();

        "And we convert the byte[] to short:".PrintGreen();
        ("还原数字:" + BitConverter.ToInt16(ss, 0)).PrintGreen();

        #endregion
    }

    private static void TextCrc16()
    {
        SayHello();

        "Now we do CRC16 check:".PrintGreen();

        byte[] data =
            { 0x24, 0x43, 0x4D, 0x44, 0x01, 0x01, 0x01, 0x64, 0xF7, 0x76, 0xF7, 0x41, 0x54, 0xD9, 0x08, 0x00 };
        ("raw data:" + BitConverter.ToString(data)).PrintGreen();

        data = Crc16Util.CRC16(data);
        ("crc data:" + BitConverter.ToString(data)).PrintGreen();

        var yes = Crc16Util.CheckCRC16(data);

        ("CRC16 Check:" + yes).PrintGreen();
        Console.WriteLine();
    }

    private static void SayHello()
    {
        var frames = new StackTrace().GetFrames();

        "----------------------------------------------------------------------".PrintGreen();
        $"- method name: {frames[1].GetMethod()?.Name}".PrintGreen();
        "----------------------------------------------------------------------".PrintGreen();
    }
}