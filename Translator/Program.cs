using System.Diagnostics;
using System.Security.Principal;
using Translator;
using Translator.Services;
using Translator.Types;
using Translator.Utils;
using static Translator.Services.Logger;

Console.BackgroundColor = ConsoleColor.Black;
Console.Title = @"Samsung Translator v1.0";

var adbIsInstalled = await Adb.IsInstalled();
if (!adbIsInstalled || !AppIsInstalled())
{
    if (!IsAdministrator())
    {
        Println(":: Some requirements is missing. The app will restart to install it.", LogLevel.Error);
        await Task.Delay(1000);
        RelaunchAsAdministrator(args);
        return;
    }

    Println(":: 'Android Debug Bridge' is not installed!", LogLevel.Error);
    Println(":: Download 'Android Debug Bridge'...");
    
    await AdbDownloader.DownloadAndInstall();
    
    // install app globally as cli tool
    InstallApp();
}


if (!await CommandsRunner.Run(args)) return;

Constants.Copyright();
await Constants.InitializeAdbServer();
await TranslatorUtils.Translate();

static bool AppIsInstalled()
{
    string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;
    string destinationDirectory = $"c:\\{Path.GetFileNameWithoutExtension(executablePath)}";
    return Directory.Exists(destinationDirectory);
}

static bool InstallApp()
{
    string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;
    string executableName = Path.GetFileName(executablePath)!;
    string destinationDirectory = $"c:\\{Path.GetFileNameWithoutExtension(executablePath)}";
    if (!Directory.Exists(destinationDirectory))
    {
        Directory.CreateDirectory(destinationDirectory);
        string destinationPath = Path.Combine(destinationDirectory, executableName);
        File.Copy(executablePath!, destinationPath, true);
        string batFile = Path.Combine(destinationDirectory, "t.bat");
        File.WriteAllText(batFile, $"""
                                    @echo off
                                    setlocal

                                    set "appPath={executableName}"

                                    if "%~1"=="" (
                                        rem echo Usage: t.bat [arguments]
                                        rem exit /b 1
                                        "%appPath%"
                                    )

                                    "%appPath%" %*
                                    endlocal
                                    """);

        EnvironmentUtil.AddPathToEnvironment(destinationDirectory);
        return File.Exists(destinationPath);
    }

    return true;
}

static bool IsAdministrator()
{
    WindowsIdentity identity = WindowsIdentity.GetCurrent();
    WindowsPrincipal principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
}

static void RelaunchAsAdministrator(string[] args)
{
    ProcessStartInfo startInfo = new();
    startInfo.UseShellExecute = true;
    startInfo.WorkingDirectory = Environment.CurrentDirectory;
    startInfo.FileName = Process.GetCurrentProcess().MainModule?.FileName;
    startInfo.Verb = "runas"; // This requests admin privileges

    if (args.Length > 0)
    {
        startInfo.Arguments = string.Join(" ", args);
    }

    try
    {
        Process.Start(startInfo);
    }
    catch (System.ComponentModel.Win32Exception)
    {
        Console.WriteLine(@"The application was not relaunched with administrator privileges.");
    }

    // Exit the current instance of the application
    Environment.Exit(0);
}
