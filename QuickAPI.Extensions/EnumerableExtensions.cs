namespace QuickAPI.Extensions;

public static class EnumerableExtensions
{
    public static void Execute<TSource>(this IEnumerable<TSource> sources, Action<TSource> executeAction)
    {
        foreach (var source in sources)
        {
            executeAction(source);
        }
    }
}