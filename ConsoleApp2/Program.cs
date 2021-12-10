#nullable enable
namespace ConsoleApp2;

internal class Program
{
    private static EventWaitHandle? _waitHandle;

    private static void Main(string[] args)
    {
        "Hello World!".PrintYellow();
        bool signaled;
        var token = BitConverter.ToString(
            BitConverter.GetBytes(DateTime.Now.Ticks)); //"CF2D4313-33DE-489D-9721-6AFF69841DEA"
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

        // Start a another thread that does something every 10 seconds.
        var timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        // Wait if someone tells us to die or do every five seconds something else.
        do
        {
            //TODO: Something else if desired.
            $"You know I am here for you :{DateTime.Now}".PrintYellow();
            signaled = _waitHandle.WaitOne(TimeSpan.FromMilliseconds(5 * 1000));
            Thread.Sleep(TimeSpan.FromHours(1));
            "Task run to completed".PrintGreen();
            //waitHandle.Set();
            //$"You should leave Now".PrintYellow();
        } while (!signaled);

        // The above loop with an interceptor could also be replaced by an endless waiter
        "Got signal to kill myself.".PrintMagenta();
    }

    private static int _count;

    private static void OnTimerElapsed(object? state)
    {
        $"Timer elapsed. : {DateTime.Now} {_count}".PrintErr();
        if (_count++ == 2000) _waitHandle?.Set(); //条件退出
    }
}