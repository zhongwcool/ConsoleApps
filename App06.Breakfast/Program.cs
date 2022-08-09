// See https://aka.ms/new-console-template for more information

using Mar.Console;

var cup = PourCoffee();
"coffee is ready".PrintGreen();

var eggs = FryEggs(2);
"eggs are ready".PrintGreen();

var bacon = FryBacon(3);
"bacon is ready".PrintGreen();

var toast = ToastBread(2);
ApplyButter(toast);
ApplyJam(toast);
"toast is ready".PrintGreen();

var oj = PourOj();
"oj is ready".PrintGreen();
"Breakfast is ready!".PrintMagenta();

Juice PourOj()
{
    "Pouring orange juice".PrintYellow();
    return new Juice();
}

void ApplyJam(Toast toast) =>
    "Putting jam on the toast".PrintYellow();

void ApplyButter(Toast toast) =>
    "Putting butter on the toast".PrintYellow();

Toast ToastBread(int slices)
{
    for (int slice = 0; slice < slices; slice++)
    {
        "Putting a slice of bread in the toaster".PrintYellow();
    }

    "Start toasting...".PrintYellow();
    Task.Delay(3000).Wait();
    "Remove toast from toaster".PrintYellow();

    return new Toast();
}

Bacon FryBacon(int slices)
{
    $"putting {slices} slices of bacon in the pan".PrintYellow();
    "cooking first side of bacon...".PrintYellow();
    Task.Delay(3000).Wait();
    for (int slice = 0; slice < slices; slice++)
    {
        "flipping a slice of bacon".PrintYellow();
    }

    "cooking the second side of bacon...".PrintYellow();
    Task.Delay(3000).Wait();
    "Put bacon on plate".PrintYellow();

    return new Bacon();
}

Egg FryEggs(int howMany)
{
    "Warming the egg pan...".PrintYellow();
    Task.Delay(3000).Wait();
    $"cracking {howMany} eggs".PrintYellow();
    "cooking the eggs ...".PrintYellow();
    Task.Delay(3000).Wait();
    "Put eggs on plate".PrintYellow();

    return new Egg();
}

Coffee PourCoffee()
{
    "Pouring coffee".PrintYellow();
    return new Coffee();
}

// These classes are intentionally empty for the purpose of this example.
// They are simply marker classes for the purpose of demonstration, contain no properties,
// and serve no other purpose.
internal class Bacon
{
}

internal class Coffee
{
}

internal class Egg
{
}

internal class Juice
{
}

internal class Toast
{
}