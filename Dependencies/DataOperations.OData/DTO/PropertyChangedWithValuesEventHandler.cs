using System.ComponentModel;

namespace DataOperations.OData
{
public class PropertyChangedWithValuesEventArgs<T> : PropertyChangedEventArgs
{
    public virtual T OldVal { get; private set; }
    public virtual T NewVal { get; private set; }

    public PropertyChangedWithValuesEventArgs(string propertyName, T oldVal, T newVal) : base(propertyName)
    {
        OldVal = oldVal;
        NewVal = newVal;
    }
}
}