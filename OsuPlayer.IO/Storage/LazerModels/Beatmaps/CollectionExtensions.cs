namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        // List<T> has a potentially more optimal path to adding a range.
        if (collection is List<T> list)
            list.AddRange(items);
        else
        {
            foreach (T obj in items)
                collection.Add(obj);
        }
    }
}