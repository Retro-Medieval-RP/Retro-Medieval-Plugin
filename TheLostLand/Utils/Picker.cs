namespace TheLostLand.Utils;

public class Picker<T>
{
    private struct Entry
    {
        public double AccumulatedWeight;
        public T Item;
    }

    private readonly List<Entry> Entries = [];
    private double AccumulatedWeight;
    private Random Random { get; set; } = new();

    public void AddEntry(T item, double weight)
    {
        AccumulatedWeight += weight;
        Entries.Add(new Entry { Item = item, AccumulatedWeight = AccumulatedWeight });
    }

    public T GetRandom()
    {
        var r = Random.NextDouble() * AccumulatedWeight;

        foreach (var entry in Entries.Where(entry => entry.AccumulatedWeight >= r))
        {
            return entry.Item;
        }

        return default;
    }
}