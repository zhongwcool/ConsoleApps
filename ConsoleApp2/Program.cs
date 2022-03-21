#nullable enable
namespace ConsoleApp2;

internal class Program
{
    private static EventWaitHandle? _waitHandle;

    private static void Main(string[] args)
    {
        "Hello World!".PrintYellow();

        // Start a another thread that does something every 10 seconds.
        var timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        Task.Factory.StartNew(TaskLoop);
        Console.Read();
    }

    private static void TaskLoop()
    {
        bool signaled;
        var token = BitConverter.ToString(BitConverter.GetBytes(DateTime.Now.Ticks));
        _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, token, out var createdNew);

        // If the handle was already there, inform the other process to exit itself.
        // Afterwards we'll also die.
        if (!createdNew)
        {
            "Inform other process to stop.".PrintGreen();
            _waitHandle.Set();
            "Informer exited.".PrintGreen();
            return;
        }

        // Wait if someone tells us to die or do every five seconds something else.
        do
        {
            $"You enter a loop :{DateTime.Now}".PrintYellow();
            signaled = _waitHandle.WaitOne(TimeSpan.FromSeconds(5));

            //TODO: Something else if desired.
            Thread.Sleep(TimeSpan.FromSeconds(2));
            "Loop run to completed".PrintGreen();
        } while (!signaled);

        // The above loop with an interceptor could also be replaced by an endless waiter
        "Got signal to kill loop task.".PrintMagenta();
    }

    private static int _count;

    private static void OnTimerElapsed(object? state)
    {
        $"Timer elapsed. : {DateTime.Now} {_count}".PrintErr();
        if (_count++ == 20) _waitHandle?.Set(); //条件退出
    }
}