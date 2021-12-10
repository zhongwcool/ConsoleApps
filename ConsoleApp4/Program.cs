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
        Console.WriteLine("本案主要展示：多线程操作List");
        Console.WriteLine("");

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

        tfTask.ContinueWith(task => { Console.WriteLine("长度是3000000吗? ：" + List.Count); });
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
        Console.WriteLine("本案主要展示：使用IProgress实现异步编程的进程通知");
        Console.WriteLine("");

        Display();
    }

    private static async void Display()
    {
        var progress = new Progress<int>(percent =>
        {
            Console.Clear();
            Console.Write("{0}%", percent);
            Console.WriteLine("");
            Console.WriteLine("本案主要展示：使用IProgress实现异步编程的进程通知");
        });
        await Task.Run(() => MyTask(progress));
        Console.WriteLine("");
        Console.WriteLine("结束");
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
        Console.WriteLine("本案主要展示：Async+Await+Task 实现异步返回");
        Console.WriteLine("");

        var ret1 = AsyncGetSum();
        Console.WriteLine("主线程执行其他处理");
        for (var i = 1; i <= 3; i++)
        {
            Console.WriteLine("Call Main()");
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        var result = ret1.Result; //阻塞主线程
        Console.WriteLine("任务执行结果：{0}", result);
    }

    private static async Task<int> AsyncGetSum()
    {
        var sum = 0;
        await Task.Run(() =>
        {
            Console.WriteLine("使用Task执行异步操作");
            for (var i = 0; i < Random.Next(1, 100); i++)
            {
                Console.WriteLine("In Task()");
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                sum += i;
            }
        });

        Console.WriteLine("Exit from Task()");
        return sum;
    }

    #endregion

    #region 有返回值Task

    private static void Test03()
    {
        Console.WriteLine("本案主要展示：有返回值Task");
        Console.WriteLine("");

        Meow("主线程");

        var task = CreateTask("1");
        task.Start();
        var result = task.Result; //阻塞主线程
        Console.WriteLine("Task 1 Result is: {0}", result);

        task = CreateTask("2");
        //该任务会运行在主线程中
        task.RunSynchronously();
        result = task.Result; //阻塞主线程
        Console.WriteLine("Task 2 Result is: {0}", result);

        task = CreateTask("3");
        Console.WriteLine("Status: " + task.Status);
        task.Start();

        while (!task.IsCompleted)
        {
            Console.WriteLine("Status: " + task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }

        Console.WriteLine(task.Status);
        result = task.Result;
        Console.WriteLine("Task 3 Result is: {0}", result);

        #region 常规使用方式

        //创建任务
        var getsumtask = new Task<int>(GetSum);
        //启动任务,并安排到当前任务队列线程中执行任务(System.Threading.Tasks.TaskScheduler)
        getsumtask.Start();
        Console.WriteLine("主线程执行其他处理");
        //等待任务的完成执行过程。
        getsumtask.Wait();
        //获得任务的执行结果
        Console.WriteLine("任务执行结果：{0}", getsumtask.Result.ToString());

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
        Console.WriteLine("使用Task执行异步操作.");
        for (var i = 0; i < 100; i++) sum += i;

        return sum;
    }

    #endregion

    #region Async+Await+Task 实现异步

    private static void Test02()
    {
        Console.WriteLine("本案主要展示：Async+Await+Task 实现异步");
        Console.WriteLine("");

        Console.WriteLine("主线程执行业务处理.");
        AsyncFunction();
        Console.WriteLine("主线程执行其他处理");
        for (var i = 0; i < 10; i++)
        {
            Console.WriteLine($"Main:i={i}");
            Thread.Sleep(2000);
        }
    }

    private static async void AsyncFunction()
    {
        Console.WriteLine("使用Task执行异步操作");
        await Task.Run(() =>
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"AsyncFunction:i={i}");
                Thread.Sleep(1000);
            }
        });
        Console.WriteLine("使用Task执行异步操作- task end");
    }

    #endregion

    #region 无返回值Task

    private static void Test01()
    {
        Console.WriteLine("本案主要展示：无返回值Task");
        Console.WriteLine("");

        var t1 = new Task(() => Wow("1"));
        t1.Start();
        t1.ContinueWith(task =>
        {
            Console.WriteLine("任务完成，完成时候的状态为：");
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
            //Console.WriteLine("Task: " + taskName + " HeartBeat: " + life);
            Thread.Sleep(1000);
            life--;
        }

        Console.WriteLine("Task: " + taskName + " end");
    }

    #endregion
}