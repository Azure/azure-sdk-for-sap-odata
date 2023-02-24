using System.ComponentModel;

// This is a non sealed interface that we can use to track and revert the changes made to the object.
// We are trying to avoid using the IChangeTracking interface because the IsChanged method is not marked as overridable.
// this approach effectively allows us to undo changes a'la unit-of-work pattern, or abandon changes a'la IReversibleChangeTracking
// or commit them via AcceptChanges a'la IChangeTracking

public interface IWorkTracking : INotifyPropertyChanged
{
    bool IsChanged {get;}
    void RevertChanges();
    void AcceptChanges(); 
    void UndoLastChange();
    void UndoLastChange(string PropertyName);

    Dictionary<string, object> GetChangeLog();
    string GetChangeLogAsJSON();
   
}