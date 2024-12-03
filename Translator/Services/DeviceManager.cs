using System.Text;

namespace Translator.Services;

public static class DeviceManager
{
    public static async Task<string[]> GetConnectedDevices()
    {
        var builder = new StringBuilder(null);
        string devices = await Adb.ExecuteAsync("devices");
        devices = devices.Replace("List of devices attached", "").Trim();
        devices = devices.Replace("device", "").Trim();
        var list = devices.Split('\n');
        var size = list.Length;
        for (var i = 0; i < size; i++)
        {
            var device = list[i].Trim();
            var model = await Adb.ExecuteAsync(device, "shell getprop ro.product.model");
            builder.Append($"{i + 1}. {model.Trim()}|{device}");
            if (i + 1 < size) builder.Append(';');
        }
        return builder.ToString().Split(';');
    }

    public static async Task<bool> IsDeviceConnected()
    {
        string adbState = await Adb.ExecuteAsync("get-state 1");
        return adbState.Trim() == "device" || adbState.Trim() == "unknown";
    }
    
    public static Task Reboot()
    {
        return Adb.ExecuteAsync("reboot");
    }
    
    public static Task Reboot(string device)
    {
        return Adb.ExecuteAsync(device, "reboot");
    }
    
    public static Task Shutdown()
    {
        return Adb.ExecuteAsync("shell reboot -p");
    }
    
    public static Task Shutdown(string device)
    {
        return Adb.ExecuteAsync(device, "shell reboot -p");
    }
    
    public static async Task<DeviceInfo> GetDeviceInfo(string? device = null)
    {
        var serial = await Adb.ExecuteAsync(device, "get-serialno");
        var manufacturer = await Adb.ExecuteAsync(device, "shell getprop ro.product.manufacturer");
        var model = await Adb.ExecuteAsync(device, "shell getprop ro.product.model");
        var version = await Adb.ExecuteAsync(device, "shell getprop ro.build.version.release");

        return new DeviceInfo(
            Serial: serial.Trim(),
            Model: model.Trim(),
            Manufacturer: manufacturer.Trim(),
            OsVersion: version.Trim()
        );   
    }
}

public readonly record struct DeviceInfo(
    string Serial,
    string Manufacturer,
    string Model,
    string OsVersion
    );