using System.Collections.Generic;

public class QueryOrderBy : GenericListStringParam
{
    // Derived property class for ODataOrderBy, inherit the base ctor and override the property
    public QueryOrderBy(string OrderBy) : base(OrderBy)
    {
    }
    public QueryOrderBy(params string[] ParamAndSortOrderBy) : base(ParamAndSortOrderBy)
    {
    }
    public QueryOrderBy(List<string> OrderBy) : base(OrderBy)
    {
    }
    public List<string> OrderBy { get { return base.GenericListString; } set { base.GenericListString = value;}}

    // orderbyfactory
    public static QueryOrderBy OrderByFactory(string OrderBy)
    {
        return new QueryOrderBy(OrderBy);
    }
    
    //orderbyfactory params
    public static QueryOrderBy OrderByFactory(params string[] ParamAndSortOrderBy)
    {
        return new QueryOrderBy(ParamAndSortOrderBy);
    }
}