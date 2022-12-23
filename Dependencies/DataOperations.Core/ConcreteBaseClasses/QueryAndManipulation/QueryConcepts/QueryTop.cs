public class QueryTop
{
    // represents the odata top construct
    public QueryTop(int? Top = null)
    {
        this.Top = Top;
    }

    public int? Top { get; private set; }
    public static QueryTop TopFactory(int Top)
    {
        return new QueryTop(Top);
    }
}