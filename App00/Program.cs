// See https://aka.ms/new-console-template for more information

using System.Globalization;
using Mar.Console;

"Hello, World!".PrintMagenta();

const float data = 12.213563135f;

Math.Round(data, 1).ToString(CultureInfo.InvariantCulture).PrintGreen();