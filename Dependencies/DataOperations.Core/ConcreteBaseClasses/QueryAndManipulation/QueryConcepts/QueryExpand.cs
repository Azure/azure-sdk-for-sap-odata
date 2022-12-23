using System.Collections.Generic;

public class QueryExpand : GenericListStringParam
{
    // Derived property class for ODataExpand, inherit the base ctor and override the property
    public QueryExpand(string Expand) : base(Expand)
    {
    }
    public QueryExpand(params string[] ParamAndSortExpand) : base(ParamAndSortExpand)
    {
    }
    public QueryExpand(List<string> Expand) : base(Expand)
    {
    }
    public List<string> Expand { get { return base.GenericListString; } set { base.GenericListString = value;}}
    // expandfactory
    public static QueryExpand ExpandFactory(string Expand)
    {
        return new QueryExpand(Expand);
    }
    //expandfactory params
    public static QueryExpand ExpandFactory(params string[] ParamAndSortExpand)
    {
        return new QueryExpand(ParamAndSortExpand);
    }
    //expandfactory list
    public static QueryExpand ExpandFactory(List<string> Expand)
    {
        return new QueryExpand(Expand);
    }
    
}