public class QuerySkip
{
    // represents the odata skip construct
    public QuerySkip(int? skip = null)
    {
        this.Skip = skip;
    }
    public int? Skip { get; private set; }

    // skipfactory
    public static QuerySkip SkipFactory(int skip)
    {
        return new QuerySkip(skip);
    }
}