namespace ConsoleApp3;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        DoStuff();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }

    private static void DoStuff()
    {
        var task = GetNameAsync();

        // Set up a continuation BEFORE MainWorkOfApplicationIDontWantBlocked
        var anotherTask = task.ContinueWith(r => { Console.WriteLine("\n安排的明明白白：" + r.Result); });

        MainWorkOfApplicationIDontWantBlocked();

        // OR wait for the result AFTER
        var result = task.Result;
        Console.WriteLine("\n苦心等来的结果：" + result);
    }

    private static void MainWorkOfApplicationIDontWantBlocked()
    {
        const string tang = "\n门前大桥下，\n游过一群鸭，\n快来快来数一数，\n二四六七八";
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
            Console.Write(prompt);
            return Console.ReadLine();
        });
    }

    private static async Task Test02()
    {
        await Task.Delay(10 * 1000);
        Console.WriteLine("Done!");
    }

    private static void Test01()
    {
        // Create the token source.
        var source = new CancellationTokenSource();

        // Pass the token to the cancelable operation.
        ThreadPool.QueueUserWorkItem(DoSomeWork, source.Token);
        Thread.Sleep(2500);

        // Request cancellation.
        source.Cancel();
        Console.WriteLine("Cancellation set in token source...");
        Thread.Sleep(2500);
        // Cancellation should have happened, so call Dispose.
        source.Dispose();
    }

    // Thread 2: The listener
    private static void DoSomeWork(object obj)
    {
        var token = (CancellationToken)obj;

        for (var i = 0; i < 100000; i++)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("In iteration {0}, cancellation has been requested...", i + 1);
                // Perform cleanup if necessary.
                //...
                // Terminate the operation.
                break;
            }

            // Simulate some work.
            Thread.SpinWait(10000);
        }
    }
}