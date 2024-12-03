using System.Reflection;
using static Translator.Services.Logger;
using Translator.Services;
using Translator.Types;

namespace Translator.Utils;

public static class CommandsRunner
{
    public static async Task<bool> Run(string[] arguments)
    {
        if (arguments.Has("-v") || arguments.Has("--version"))
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            Version? version = assembly?.GetName().Version;
            Println($"v{version}");
            return false;
        }

        if (arguments.Has("-h") || arguments.Has("--help"))
        {
            Println(GetHelpText());
            return false;
        }
        
        if (arguments.Has("--reset"))
        {
            Constants.Copyright();
            await Constants.InitializeAdbServer();
            await TranslatorUtils.ReadDeviceInfo();
            Print("Removing translation waiting...");
            await GalaxyThemeApp.EnableThemeStore();
            await GalaxyThemeApp.EnableThemeCenter();
            await GalaxyThemeApp.OpenInstalledThemesPage();
            Println("OK", LogLevel.Success);
            return false;
        }
        
        if (arguments.Has("--list"))
        {
            Println(":: Connected Devices List: \n");
            var devices = await DeviceManager.GetConnectedDevices();
            foreach (var device in devices)
            {
                Println("\t" + device);
            }

            Console.WriteLine();
            ReadDeviceId:
            Print(":: Enter device number to translate it: ");
            string? number = Console.ReadLine();
            if (number == null || !number.All(char.IsNumber))
            {
                Print("\nPlease enter a numeric value. Your previous input was not valid. Please try again.\n", LogLevel.Error);
                goto ReadDeviceId;
            }

            var index = int.Parse(number) - 1;

            if (index > devices.Length - 1)
            {
                Print($"\nNot found any device with number '{number}'. Please try again.\n", LogLevel.Error);
                goto ReadDeviceId;
            }
            
            var serial = devices[index].Split('|').Last();
            await TranslatorUtils.Translate(serial);
            
            return false;
        }

        if (arguments.Has("--i"))
        {
            Constants.Copyright();
            await Constants.InitializeAdbServer();
            await TranslatorUtils.Translate();
            return false;
        }
        
        if (arguments.Has("--k"))
        {
            Constants.Copyright();
            await Constants.InitializeAdbServer();
            await TranslatorUtils.ReadDeviceInfo();
            Print("Installing Keyboard...");
            await InputMethod.Install();
            Println("OK", LogLevel.Success);
            return false;
        }
        
        if (arguments.Has("--data-icon"))
        {
            Constants.Copyright();
            await Constants.InitializeAdbServer();
            await TranslatorUtils.ReadDeviceInfo();
            Print("Adding MobileData Icon...");
            await TranslatorUtils.AddMobileDataIcon();
            Println("OK", LogLevel.Success);
            return false;
        }
        
        return true;
    }
    
    private static bool Has(this string[] arguments, string arg)
    {
        return arguments.Any(a => string.Equals(a, arg, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetHelpText()
    {
        return """
               Samsung Translator CLI v1.0
               
               DESCRIPTION:
                 This CLI tool provides a set of commands for managing and using the Samsung Translator application. It offers features like automatic download and installation of adb, setting up global environment variables, and performing translations.
               
               USAGE:
                 Simply connect your device and open Translator.exe to use the application.
               
               CLI COMMANDS:
                 - Get help: 't --help' or 't -h'
                 - Check tool version: 't -v' or 't --version'
                 - List connected devices: 't --list'
                 - Install translation: 't --i' or 't'
                 - Install the keyboard: 't --k'
                 - Remove translation: 't --reset'
                 - Add mobile data icon: 't --data-icon'
               
               
               NOTE:
                 For detailed help on each command, use 't [command] --help'.
               
               ADDITIONAL INFORMATION:
                 - Full translation is supported from 9 to 13.
               
               EXAMPLES:
                 - Check tool version:
                   $ t -v
               
                 - View connected devices:
                   $ t --list
               
                 - Install translation:
                   $ t --i
               
                 - Install the keyboard:
                   $ t --k
               
                 - Remove translation:
                   $ t --reset
               
                 - Add the mobile data icon:
                   $ t --data-icon
               """;
    }
}
