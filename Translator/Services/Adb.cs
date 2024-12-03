namespace Translator.Services;

public static class Adb
{
    public static async Task<bool> IsInstalled()
    {
        var v = await ExecuteAsync("--version");
        return v.Contains("Android Debug Bridge");
    }
    
    public static Task<string> ExecuteAsync(string command)
    {
        return CommandLine.RunAsync($"adb {command}");
    }
    
    public static Task<string> ExecuteAsync(string? device, string command)
    {
        var c = device != null ? $"adb -s {device} {command}" : $"adb {command}";
        return CommandLine.RunAsync(c);
    }
}