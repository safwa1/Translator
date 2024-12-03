using Translator.Utils;

namespace Translator.Services;

public static class PackageManager
{
    public static async Task<bool> Install(string path)
    {
        string state = await Adb.ExecuteAsync($"install -r {path}");
        return state.Contains("Success");
    }
    
    public static async Task<bool> Install(string? device, string path)
    {
        string state = await Adb.ExecuteAsync(device,$"install -r {path}");
        return state.Contains("Success");
    }
    
    public static async Task<bool> Uninstall(string packageName)
    {
        string state = await Adb.ExecuteAsync($"shell pm uninstall {packageName}");
        return state.Contains("Success");
    }
    
    public static async Task<bool> Uninstall(string? device, string packageName)
    {
        string state = await Adb.ExecuteAsync(device, $"shell pm uninstall {packageName}");
        return state.Contains("Success");
    }
    
    public static async Task<bool> Find(string? device, string packageName)
    {
        string isExist = await Adb.ExecuteAsync(device, $"shell pm list packages {packageName}");
        return !string.IsNullOrEmpty(isExist);
    }
    
    public static async Task<bool> Find(string packageName)
    {
        string isExist = await Adb.ExecuteAsync($"shell pm list packages {packageName}");
        return !string.IsNullOrEmpty(isExist);
    }
    
    public static async Task<bool> UninstallSystemApp(string? device, string packageName)
    {
        string state = await Adb.ExecuteAsync(device, $"shell pm uninstall -k --user 0 {packageName}");
        return state.Contains("Success");
    }

    public static async Task<bool> UninstallSystemApp(string packageName)
    {
        string state = await Adb.ExecuteAsync($"shell pm uninstall -k --user 0 {packageName}");
        return state.Contains("Success");
    }
    
    public static async Task<bool> RestoreSystemApp(string? device, string packageName)
    {
        string res = await Adb.ExecuteAsync(device, $"shell cmd package install-existing {packageName}");
        return res.Contains("Success");
    }
    
    public static async Task<bool> RestoreSystemApp(string packageName)
    {
        string res = await Adb.ExecuteAsync($"shell cmd package install-existing {packageName}");
        return res.Contains("Success");
    }

    public static async Task<bool> ClearData(string? device, string packageName)
    {
        string data = await Adb.ExecuteAsync(device, $"shell pm clear {packageName}");
        return data.Contains("Success");
    }
    
    public static async Task<bool> ClearData(string packageName)
    {
        string data = await Adb.ExecuteAsync($"shell pm clear {packageName}");
        return data.Contains("Success");
    }
    
    public static async Task<bool> Disable(string? device, string packageName) {
        string data = await Adb.ExecuteAsync(device, $"{Constants.DisableCommand.Merge(4)} {packageName}");
        return data.Contains("Success");
    }
    
    public static async Task<bool> Enable(string? device, string packageName) {
        string data = await Adb.ExecuteAsync(device, $"{Constants.EnableCommand.Merge(4)} {packageName}");
        return data.Contains("installed for user: 0");
    }
    
    public static async Task<string> OpenApp(string? device, string packageName) {
        string op = await Adb.ExecuteAsync(device, $"shell monkey -p {packageName} -c android.intent.category.LAUNCHER 1");
        return op;
    }
    
    public static async Task<string> OpenApp(string packageName) {
        string op = await Adb.ExecuteAsync($"shell monkey -p {packageName} -c android.intent.category.LAUNCHER 1");
        return op;
    }
    
    public static async Task<string> StartActivity(string? device, string activity) {
        string sa = await Adb.ExecuteAsync(device, $"shell am start -a android.intent.action.MAIN -n {activity}");
        return sa;
    }

    public static async Task<string> StartActivity(string activity) {
        string sa = await Adb.ExecuteAsync($"shell am start -a android.intent.action.MAIN -n {activity}");
        return sa;
    }

    public static Task Stop(string? device, string packageName)
    {
        return Adb.ExecuteAsync(device, $"shell am force-stop {packageName}");
    }
    
    public static Task Stop(string packageName)
    {
        return Adb.ExecuteAsync($"shell am force-stop {packageName}");
    }
}
