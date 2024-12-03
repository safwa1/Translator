using System.IO.Compression;

namespace Translator.Utils;

public static class Zip
{
    public static readonly string Dir = Constants.Dir.Merge(4);
    public static readonly string Overlay = Constants.Overlay.Merge(4);
    public static readonly string Key = Constants.Key.Merge(4);
    
    public static bool Extract()
    {
        if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
        
        if (!File.Exists(Overlay))
        {
            var name = Path.Combine(Dir, Guid.NewGuid().ToString("N").ToUpper() + ".zip");
            File.WriteAllBytes(name, AppResources.files);
            ZipFile.ExtractToDirectory(name, Dir);
            File.Delete(name);
        }
        
        return File.Exists(Overlay);
    }
}