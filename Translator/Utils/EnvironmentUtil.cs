namespace Translator.Utils;

public static class EnvironmentUtil
{
    public static bool AddPathToEnvironment(string pathToAdd)
    {
        try
        {
            string? currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (currentPath != null && !currentPath.Contains(pathToAdd, StringComparison.OrdinalIgnoreCase))
            {
                string newPath = currentPath + ";" + pathToAdd;
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Machine);
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}