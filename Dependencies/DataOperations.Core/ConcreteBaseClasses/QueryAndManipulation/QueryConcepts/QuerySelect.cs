using System.Collections.Generic;

public class QuerySelect : GenericListStringParam
{
    // Derived property class for ODataSelect, inherit the base ctor and override the property
    public QuerySelect(string Select) : base(Select)
    {
    }
    public QuerySelect(params string[] ParamAndSortSelect) : base(ParamAndSortSelect)
    {
    }
    public QuerySelect(List<string> Select) : base(Select)
    {
    }
    public List<string> Select { get { return base.GenericListString; } set { base.GenericListString = value;}}

    // selectfactory
    public static QuerySelect SelectFactory(string Select)
    {
        return new QuerySelect(Select);
    }

    //selectfactory params
    public static QuerySelect SelectFactory(params string[] ParamAndSortSelect)
    {
        return new QuerySelect(ParamAndSortSelect);
    }
    //selectfactory list
    public static QuerySelect SelectFactory(List<string> Select)
    {
        return new QuerySelect(Select);
    }
    
}
