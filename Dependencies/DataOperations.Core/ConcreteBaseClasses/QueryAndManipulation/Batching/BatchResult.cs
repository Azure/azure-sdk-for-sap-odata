using System.Collections.Generic;

namespace DataOperations
{
    public class BatchResult : List<IBaseDTOWithIDAndETag>, IEnumerable<IBaseDTOWithIDAndETag>
    {
        
    }
}