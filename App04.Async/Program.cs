using Mar.Console;

namespace ConsoleApp4;

internal class Program
{
    private static readonly Random Random = new();

    public static void Main(string[] args)
    {
        Test05();

        //固定，使程序不立即结束退出
        Console.ReadKey();
    }

    #region 多线程操作List

    private static readonly List<long> List = new();

    private static void Test06()
    {
        "本案主要展示：多线程操作List".PrintGreen();
        "\n".PrintGreen();

        var factory = new TaskFactory(TaskCreationOptions.AttachedToParent,
            TaskContinuationOptions.AttachedToParent);
        var childTasks = new[]
        {
            factory.StartNew(Task_0),
            factory.StartNew(Task_1),
            factory.StartNew(Task_2)
        };
        var tfTask = factory.ContinueWhenAll(childTasks,
            completedTasks => completedTasks.Where(w => !w.IsFaulted && !w.IsCanceled),
            TaskContinuationOptions.None);

        tfTask.ContinueWith(task => { $"长度是3000000吗? ：{List.Count}".PrintGreen(); });
    }

    private static void Task_0()
    {
        for (var i = 0; i < 1000000; i++)
            lock (List)
            {
                List.Add(i);
            }
    }

    private static void Task_1()
    {
        for (var i = 0; i < 1000000; i++)
            lock (List)
            {
                List.Add(i);
            }
    }

    private static void Task_2()
    {
        for (var i = 0; i < 1000000; i++)
            lock (List)
            {
                List.Add(i);
            }
    }

    #endregion

    #region 使用IProgress实现异步编程的进程通知

    private static void Test05()
    {
        "本案主要展示：使用IProgress实现异步编程的进程通知".PrintGreen();
        "\n".PrintGreen();

        Display();
    }

    private static async void Display()
    {
        var progress = new Progress<int>(percent =>
        {
            Console.Clear();
            $"{percent}%".PrintGreen();
            "本案主要展示：使用IProgress实现异步编程的进程通知".PrintGreen();
        });
        await Task.Run(() => MyTask(progress));
        "\n".PrintGreen();
        "结束".PrintGreen();
    }

    private static void MyTask(IProgress<int> progress)
    {
        for (var i = 0; i <= 100; ++i)
        {
            Thread.Sleep(TimeSpan.FromSeconds(Random.Next(1, 3)));
            progress?.Report(i);
        }
    }

    #endregion

    #region Async+Await+Task 实现异步返回

    private static void Test04()
    {
        "本案主要展示：Async+Await+Task 实现异步返回".PrintGreen();
        "".PrintGreen();

        var ret1 = AsyncGetSum();
        "主线程执行其他处理".PrintGreen();
        for (var i = 1; i <= 3; i++)
        {
            "Call Main()".PrintGreen();
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        var result = ret1.Result; //阻塞主线程
        $"任务执行结果：{result}".PrintGreen();
    }

    private static async Task<int> AsyncGetSum()
    {
        var sum = 0;
        await Task.Run(() =>
        {
            "使用Task执行异步操作".PrintGreen();
            for (var i = 0; i < Random.Next(1, 100); i++)
            {
                "In Task()".PrintGreen();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                sum += i;
            }
        });

        "Exit from Task()".PrintGreen();
        return sum;
    }

    #endregion

    #region 有返回值Task

    private static void Test03()
    {
        "本案主要展示：有返回值Task".PrintGreen();
        "".PrintGreen();

        Meow("主线程");

        var task = CreateTask("1");
        task.Start();
        var result = task.Result; //阻塞主线程
        $"Task 1 Result is: {result}".PrintGreen();

        task = CreateTask("2");
        //该任务会运行在主线程中
        task.RunSynchronously();
        result = task.Result; //阻塞主线程
        $"Task 2 Result is: {result}".PrintGreen();

        task = CreateTask("3");
        $"Status: {task.Status}".PrintGreen();
        task.Start();

        while (!task.IsCompleted)
        {
            $"Status: {task.Status}".PrintGreen();
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }

        task.Status.ToString().PrintGreen();
        result = task.Result;
        $"Task 3 Result is: {result}".PrintGreen();

        #region 常规使用方式

        //创建任务
        var getsumtask = new Task<int>(GetSum);
        //启动任务,并安排到当前任务队列线程中执行任务(System.Threading.Tasks.TaskScheduler)
        getsumtask.Start();
        "主线程执行其他处理".PrintGreen();
        //等待任务的完成执行过程。
        getsumtask.Wait();
        //获得任务的执行结果
        $"任务执行结果：{getsumtask.Result.ToString()}".PrintGreen();

        #endregion
    }

    private static Task<int> CreateTask(string name)
    {
        return new Task<int>(() => Meow(name));
    }

    private static int Meow(string name)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

        //模拟耗时操作
        var rand = Random.Next(1, 20);
        Thread.Sleep(TimeSpan.FromSeconds(rand));
        return rand;
    }

    private static int GetSum()
    {
        var sum = 0;
        "使用Task执行异步操作.".PrintGreen();
        for (var i = 0; i < 100; i++) sum += i;

        return sum;
    }

    #endregion

    #region Async+Await+Task 实现异步

    private static void Test02()
    {
        "本案主要展示：Async+Await+Task 实现异步".PrintGreen();
        "".PrintGreen();

        "主线程执行业务处理.".PrintGreen();
        AsyncFunction();
        "主线程执行其他处理".PrintGreen();
        for (var i = 0; i < 10; i++)
        {
            $"Main:i={i}".PrintGreen();
            Thread.Sleep(2000);
        }
    }

    private static async void AsyncFunction()
    {
        "使用Task执行异步操作".PrintGreen();
        await Task.Run(() =>
        {
            for (var i = 0; i < 10; i++)
            {
                $"AsyncFunction:i={i}".PrintGreen();
                Thread.Sleep(1000);
            }
        });
        "使用Task执行异步操作- task end".PrintGreen();
    }

    #endregion

    #region 无返回值Task

    private static void Test01()
    {
        "本案主要展示：无返回值Task".PrintGreen();
        "".PrintGreen();

        var t1 = new Task(() => Wow("1"));
        t1.Start();
        t1.ContinueWith(task =>
        {
            "任务完成，完成时候的状态为：".PrintGreen();
            Console.WriteLine("IsCanceled={0}\tIsCompleted={1}\tIsFaulted={2}", task.IsCanceled, task.IsCompleted,
                task.IsFaulted);
        });
        var t2 = new Task(() => Wow("2"));
        t2.Start();
        //Task.WaitAll(t1, t2);

        var t3 = Task.Run(() => Wow("3"));
        //Task.WaitAll(t3);

        var t4 = Task.Factory.StartNew(() => Wow("4"));
        Task.WaitAll(t1, t2, t3, t4);
    }

    private static void Wow(string taskName)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            taskName,
            Thread.CurrentThread.ManagedThreadId,
            Thread.CurrentThread.IsThreadPoolThread
        );

        var life = Random.Next(1, 10);
        while (life > 0)
        {
            //$"Task: {taskName} HeartBeat: {life}".PrintGreen();
            Thread.Sleep(1000);
            life--;
        }

        $"Task: {taskName} end".PrintGreen();
    }

    #endregion
}