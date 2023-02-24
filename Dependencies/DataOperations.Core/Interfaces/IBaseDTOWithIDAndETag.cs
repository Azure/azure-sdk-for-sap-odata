using System.Collections.Generic;
using System.ComponentModel;

namespace DataOperations
{
    public interface IBaseDTOWithIDAndETag : INotifyPropertyChanged, IWorkTracking
    {
        string eTag { get; set; }
        IEnumerable<string> GetPrimaryKey();
        Dictionary<string, object> GetPrimaryKeyValues();
        List<string> GetReferenceKeys(string ReferenceName);
        Dictionary<string, object> GetReferenceValues(string ReferenceName);
        void AttachDispatcher<T>(IOperationsDispatcher dispatcher) where T : IBaseDTOWithIDAndETag;
    }
}
