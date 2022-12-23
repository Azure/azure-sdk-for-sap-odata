public class QueryCount
{
    // Represents the odata count construct type int?

    // ctor
    public QueryCount(int? Count = null)
    {
        this.Count = Count;
    }
    public int? Count { get; set; }
    public static QueryCount CountFactory(int Count)
    {
        return new QueryCount(Count);
    }
}