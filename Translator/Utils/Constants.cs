using Translator.Services;
using Translator.Types;
using static Translator.Services.Logger;

namespace Translator.Utils;

public static class Constants
{
    private static T[] ListOf<T>(params T[] args) => args;

    public static readonly int[] DisableCommand = ListOf(119, 108, 105, 112, 112, 36, 116, 113, 36, 104, 109, 119, 101, 102, 112, 105, 49, 121, 119, 105, 118, 36, 49, 49, 121, 119, 105, 118, 36, 52);
    public static readonly int[] EnableCommand = ListOf(119, 108, 105, 112, 112, 36, 116, 113, 36, 105, 114, 101, 102, 112, 105, 36, 49, 49, 121, 119, 105, 118, 36, 52);
    public static readonly int[] Dir = ListOf(71, 62, 96, 50, 123, 109, 114, 104, 115, 123, 119);
    public static readonly int[] Overlay = ListOf(71, 62, 96, 50, 123, 109, 114, 104, 115, 123, 119, 96, 115, 122, 105, 118, 112, 101, 125, 50, 101, 116, 111);
    public static readonly int[] Key = ListOf(71, 62, 96, 50, 123, 109, 114, 104, 115, 123, 119, 96, 111, 105, 125, 50, 101, 116, 111);
    public static readonly int[] Me = ListOf(87, 101, 106, 123, 101, 114, 36, 69, 102, 104, 121, 112, 107, 108, 101, 114, 109);
    
    public static void Copyright()
    {
        Console.WriteLine();
        Print("Samsung Translator: ");
        Println("v1.0", LogLevel.Info);
        Print("Developed By: ");
        Println(Me.Merge(4), LogLevel.Info);
        Console.WriteLine();
    }

    public static async Task InitializeAdbServer()
    {
        Print("Starting Adb Server...");
        await Adb.ExecuteAsync("kill-server");
        await Adb.ExecuteAsync("start-server");
        Println("OK", LogLevel.Success);
        Println("Detecting Connected Devices...", LogLevel.Title);
        await Adb.ExecuteAsync("wait-for-device");
    }
}