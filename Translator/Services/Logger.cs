using Translator.Types;

namespace Translator.Services;

public static class Logger
{
    public static void Print(object? value, LogLevel type = LogLevel.None)
    {
        var color = type switch
        {
            LogLevel.Title => ConsoleColor.DarkBlue,
            LogLevel.Yellow => ConsoleColor.Yellow,
            LogLevel.Magenta => ConsoleColor.Magenta,
            LogLevel.Success => ConsoleColor.DarkGreen,
            LogLevel.Info => ConsoleColor.Blue,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.None => ConsoleColor.White,
            _ => default
        };

        Console.ForegroundColor = color;
        Console.Write(value);
        Console.ResetColor();
    }

    public static void Println(object? value, LogLevel type = LogLevel.None)
    {
        Print($"{value}\n", type);
    }
}