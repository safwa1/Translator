using Translator.Types;
using Translator.Utils;

namespace Translator.Services;

public static class InputMethod
{
    private const string KeyboardPackageName = "inputmethod.latin.perfectkeyboard";
    private const string KeyboardId = "inputmethod.latin.perfectkeyboard/com.android.inputmethod.latin.DictionaryEditor";
    private static readonly List<Permission> KeyboardPermissions =
    [
        new("Vibrate", "android.permission.VIBRATE"),
        new("ReadUserDictionary", "android.permission.READ_USER_DICTIONARY"),
        new("WriteUserDictionary", "android.permission.WRITE_USER_DICTIONARY"),
        new("RecordAudio", "android.permission.RECORD_AUDIO"),
        new("ReadContacts", "android.permission.READ_CONTACTS"),
        new("WriteExternalStorage", "android.permission.WRITE_EXTERNAL_STORAGE"),
        new("ReadUserDictionary", "android.permission.READ_USER_DICTIONARY")
    ];

    public static async Task Install(string? device = null)
    {
        var installed = await PackageManager.Install(Zip.Key);
        if(!installed) return;
        
        foreach (var (_, value) in KeyboardPermissions)
        {
            await Grant(device,KeyboardPackageName, value);
        }
        
        await EnableKeyboard(device, KeyboardId);
    }
    
    private static async Task EnableKeyboard(string? device, string keyboardId) {
        await Adb.ExecuteAsync(device, $"shell ime enable {keyboardId}");
        await Adb.ExecuteAsync(device, $"shell ime set {keyboardId}");
    }
    
    private static  Task Grant(string? device, string package, string permission) {
        return Adb.ExecuteAsync(device, $"shell pm grant {package} {permission}");
    }
    
}