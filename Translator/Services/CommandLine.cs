using System.Diagnostics;
using System.Text;

namespace Translator.Services;

public static class CommandLine
{
    private static async Task<string> RunAsync(string app, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = app,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.WhenAll(
                process.WaitForExitAsync(),
                Task.Run(() => process.WaitForExit())
            ).ConfigureAwait(false);

            if (process.ExitCode != 0)
            {
                //Console.WriteLine($"Error: {errorBuilder}");
            }

            return outputBuilder.ToString();
        }
        finally
        {
            process.Close();
        }
    }

    public static async Task<string> RunAsync(string arguments)
    {
        return await RunAsync("cmd.exe", $"/c {arguments}").ConfigureAwait(false);
    }
}