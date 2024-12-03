using Translator.Utils;

namespace Translator.Services;

public static class GalaxyThemeApp
{
    private const string Themestore = "com.samsung.android.themestore";
    private const string Themecenter = "com.samsung.android.themecenter";

    public static Task<string> OpenThemeStore(string? device = null)
    {
        return Adb.ExecuteAsync(device ,"shell am start -a \"android.intent.action.VIEW\" -d \"themestore://MainPage\"");
    }
    
    public static Task<string> OpenInstalledThemesPage(string? device = null)
    {
        return Adb.ExecuteAsync(device, "shell am start -a \"android.intent.action.VIEW\" -d \"themestore://MyTheme\"");
    }

    public static Task StopThemeStore(string? device = null) => PackageManager.Stop(device, Themestore);

    public static Task StopThemeCenter(string? device = null) => PackageManager.Stop(device, Themecenter);
    
    public static async Task DisableThemeCenter(string? device = null)
    {
        await PackageManager.UninstallSystemApp(device, Themecenter);
        await PackageManager.Disable(device,Themecenter);
    }
    
    public static async Task EnableThemeCenter(string? device = null)
    {
        await PackageManager.RestoreSystemApp(device, Themecenter);
        await PackageManager.Enable(device, Themecenter);
    }
    
    public static async Task EnableThemeStore(string? device = null)
    {
        await PackageManager.RestoreSystemApp(device, Themestore);
        await PackageManager.Enable(device, Themecenter);
    }

}