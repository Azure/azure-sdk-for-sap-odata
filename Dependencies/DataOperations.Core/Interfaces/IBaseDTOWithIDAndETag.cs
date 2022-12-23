using System.Collections.Generic;

namespace DataOperations
{
    public interface IBaseDTOWithIDAndETag
    {
        string eTag { get; set; }
        IEnumerable<string> GetPrimaryKey();
        Dictionary<string, object> GetPrimaryKeyValues();
        List<string> GetReferenceKeys(string ReferenceName);
        Dictionary<string, object> GetReferenceValues(string ReferenceName);
        void AttachDispatcher<T>(IOperationsDispatcher dispatcher) where T : IBaseDTOWithIDAndETag;
    }
}
