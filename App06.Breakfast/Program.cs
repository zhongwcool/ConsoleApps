// See https://aka.ms/new-console-template for more information

using Mar.Console;

"本文主要演示顺序执行多个异步Task".PrintMagenta();

CookBreakfast();

"The method CookBreakfast don't block ui thread, Or you can't see this tips at this position".PrintErr();
Console.Read();

async void CookBreakfast()
{
    var cup = PourCoffee();
    "coffee is ready".PrintGreen();

    var eggs = await FryEggsAsync(2);
    "eggs are ready".PrintGreen();

    var bacon = await FryBaconAsync(3);
    "bacon is ready".PrintGreen();

    var toast = await ToastBreadAsync(2);
    ApplyButter(toast);
    ApplyJam(toast);
    "toast is ready".PrintGreen();

    var oj = PourOj();
    "oj is ready".PrintGreen();
    "Breakfast is ready!".PrintMagenta();
}

Juice PourOj()
{
    "Pouring orange juice".PrintYellow();
    return new Juice();
}

void ApplyJam(Toast toast) =>
    "Putting jam on the toast".PrintYellow();

void ApplyButter(Toast toast) =>
    "Putting butter on the toast".PrintYellow();

async Task<Toast> ToastBreadAsync(int slices)
{
    for (int slice = 0; slice < slices; slice++)
    {
        "Putting a slice of bread in the toaster".PrintYellow();
    }

    "Start toasting...".PrintYellow();
    await Task.Delay(3000);
    "Remove toast from toaster".PrintYellow();

    return new Toast();
}

async Task<Bacon> FryBaconAsync(int slices)
{
    $"putting {slices} slices of bacon in the pan".PrintYellow();
    "cooking first side of bacon...".PrintYellow();
    await Task.Delay(3000);
    for (int slice = 0; slice < slices; slice++)
    {
        "flipping a slice of bacon".PrintYellow();
    }

    "cooking the second side of bacon...".PrintYellow();
    await Task.Delay(3000);
    "Put bacon on plate".PrintYellow();

    return new Bacon();
}

async Task<Egg> FryEggsAsync(int howMany)
{
    "Warming the egg pan...".PrintYellow();
    await Task.Delay(3000);
    $"cracking {howMany} eggs".PrintYellow();
    "cooking the eggs ...".PrintYellow();
    await Task.Delay(3000);
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