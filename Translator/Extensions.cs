namespace Translator;

public static class Extensions
{
    public static string Merge(this IEnumerable<int> list, int slug) 
        => new(list.Select(x => (char)(x - slug)).ToArray());
}