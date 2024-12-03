using System.IO.Compression;
using System.Net.NetworkInformation;
using Translator.Types;
using Translator.Utils;

namespace Translator.Services
{
    public static class AdbDownloader
    {
        private const string AdbUrl = @"https://github.com/safwa1/adb-latest/raw/main/adb.zip";
        private const string DownloadPath = "adb.zip";
        private const string ExtractPath = @"C:\adb";
        
        public static async Task DownloadAndInstall()
        {
            if (!await IsInternetConnectedAsync())
            {
                Console.WriteLine(AppResources.no_internet);
                return;
            }
            
            using (HttpClient httpClient = new())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(AdbUrl, HttpCompletionOption.ResponseHeadersRead))
                await using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    long? contentLength = response.Content.Headers.ContentLength;

                    if (contentLength.HasValue)
                    {
                        await using var fileStream = new FileStream(DownloadPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        long bytesDownloaded = 0;
                        DateTime startTime = DateTime.Now;

                        do
                        {
                            bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                bytesDownloaded += bytesRead;
                                DateTime currentTime = DateTime.Now;
                                TimeSpan elapsedTime = currentTime - startTime;
                                double downloadSpeed = bytesDownloaded / elapsedTime.TotalSeconds;
                                Logger.Print($"\r:: Download Progress: {bytesDownloaded}/{contentLength} bytes " +
                                               $"({(double)bytesDownloaded / contentLength * 100:F2}%), " +
                                               $"Speed: {FormatBytes(downloadSpeed)}/s");
                            }
                        } while (bytesRead > 0);
                    }
                }
            }

            ExtractFile(DownloadPath, ExtractPath);
        }

        private static void ExtractFile(string zipPath, string extractPath)
        {
            try
            {
                Logger.Print("\n:: Installing 'Android Debug Bridge'...");
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                EnvironmentUtil.AddPathToEnvironment(extractPath);
                Logger.Println("OK", LogLevel.Success);
            }
            catch (Exception ex)
            {
                Logger.Println($"Error extracting file: {ex.Message}", LogLevel.Error);
            }
            finally
            {
                // Clean up the downloaded zip file.
                File.Delete(zipPath);
            }
        }

        static async Task<bool> IsInternetConnectedAsync()
        {
            try
            {
                using Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync("8.8.8.8", 1000); // Google's DNS server
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string FormatBytes(double bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            while (bytes >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                bytes /= 1024;
                suffixIndex++;
            }
            return $"{bytes:F2} {suffixes[suffixIndex]}";
        }
    }
}
