
using System.Collections.Generic;

public class GenericListStringParam
{
    
    // create and assign a new instance of this class to the property of the class
    public GenericListStringParam(string GenericListString)
    {
        // ToDo: Check the property is actually orderable by this parameter
        
        this.GenericListString.Add(GenericListString);

    }

    // constructor overload that takes a list of strings
    public GenericListStringParam(List<string> GenericListString)
    {
        this.GenericListString = GenericListString;
    }

    public GenericListStringParam(params string[] GenericListString)
    {
        foreach (string s in GenericListString)
        {
            this.GenericListString.Add(s);
        }
    }
    public void Add(string ODataParam)
    {
        this.GenericListString.Add(ODataParam);
    }
    public void Delete(string ODataParam)
    {
        this.GenericListString.Remove(ODataParam);
    }
    public List<string> GenericListString { get; set; } = new List<string>();
}