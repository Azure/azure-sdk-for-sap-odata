using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataOperations
{
    public interface IBatchChangeItemSet
    {
        BatchChangeItemSet AddToBatch(BatchChangeItem bci);
        Task<BatchResult> SendBatchAsync();
    }
    public class BatchChangeItemSet : List<BatchChangeItem>, IEnumerable<BatchChangeItem>, IBatchChangeItemSet
    {
        public virtual BatchChangeItemSet AddToBatch(BatchChangeItem bci)
        {
            Add(bci);
            return this;
        }
        public virtual async Task<BatchResult> SendBatchAsync()
        {
            // Send the batch request
            throw new NotImplementedException();
            //return Task.FromResult(new BatchResult());
        }
    }
}