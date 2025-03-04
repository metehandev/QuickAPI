namespace MY.QuickAPI.Extensions;

internal static class EnumerableExtensions
{
    internal static void Execute<TSource>(this IEnumerable<TSource> sources, Action<TSource> executeAction)
    {
        foreach (var source in sources)
        {
            executeAction(source);
        }
    }
}