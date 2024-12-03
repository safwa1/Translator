using Translator.Services;
using Translator.Types;
using static Translator.Services.Logger;

namespace Translator.Utils;

public static class TranslatorUtils
{
    public static async Task<DeviceInfo?> ReadDeviceInfo(string? device = null)
    {
    var info = await DeviceManager.GetDeviceInfo(device);
        Print("Device Serial: ");
        Println(info.Serial, LogLevel.Info);

        Print("Manufacturer: ");
        Println(info.Manufacturer, LogLevel.Info);

        Print("Device Model: ");
        Println(info.Model, LogLevel.Info);

        Print("Android Version: ");
        Println(info.OsVersion, LogLevel.Info);

        if (int.Parse(info.OsVersion) < 9)
        {
            Println("This Version Not Supported Yet!", LogLevel.Error);
            Console.ReadKey();
            Environment.Exit(0);
        }

        return info;
    }
    
    public static async Task Translate(string? device = null)
    {
        var deviceInfo = await ReadDeviceInfo(device);
        Console.WriteLine();
        Print(":: Translating ", LogLevel.Magenta);
        Print($" {deviceInfo?.Model} ", LogLevel.Success);
        Print("...");
        var decode = Zip.Extract();
        if (!decode)
        {
            Print("Fail\n", LogLevel.Error);
            return;
        }

        await GalaxyThemeApp.OpenThemeStore(device);

        var install = await Install(device);
        if (!install)
        {
            Print("Fail\n", LogLevel.Error);
            return;
        }

        Directory.Delete(Zip.Dir, true);
        await Task.Delay(2000);
        await GalaxyThemeApp.StopThemeStore(device);
        await GalaxyThemeApp.StopThemeCenter(device);
        await GalaxyThemeApp.OpenInstalledThemesPage(device);

        Println("\n:: Apply Theme and press any key to continue...", LogLevel.Yellow);
        Console.ReadKey();

        Print(":: Finalize the translation process...");
        await GalaxyThemeApp.DisableThemeCenter(device);
        await AddArabicLang(device);
        await InputMethod.Install(device);
        await AddMobileDataIcon();
        Println("OK", LogLevel.Success);
        await Adb.ExecuteAsync((bool)deviceInfo?.OsVersion.Contains("13") ? "shell reboot -p" : "reboot");
        Println(":: Success, All Done ...", LogLevel.Success);
        Println("Press any key to exit...", LogLevel.Title);
        Console.ReadKey();
        Environment.Exit(0);
    }

    private static async Task<bool> Install(string? device = null)
    {
        var install = await PackageManager.Install(device, Zip.Overlay);
        return install;
    }

    private static Task<string> AddArabicLang(string? device = null)
    {
        return Adb.ExecuteAsync(device, "shell settings put system system_locales 'ar-MA,en-US'");
    }
    
    public static async Task<bool> AddMobileDataIcon(string? device = null) {
        await Adb.ExecuteAsync(device, "shell settings put secure sysui_qs_tiles \"Wifi,MobileData,Bluetooth,$(settings get secure sysui_qs_tiles),Hotspot\"");
        string confirm = await Adb.ExecuteAsync(device, "shell settings get secure sysui_qs_tiles");
        return confirm.Contains("MobileData");
    }
}