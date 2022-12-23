using Microsoft.Azure.WebJobs;
namespace DataOperations.Bindings
{
    public class BaseDTOAsyncCollector<T> : IAsyncCollector<T> where T : IBaseDTOWithIDAndETag
    {
        private readonly IOperationsDispatcher _dispatcher;
        public BaseDTOAsyncCollector(IOperationsDispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }
        public async Task AddAsync(T saveableObject, CancellationToken cancellationToken = default(CancellationToken)) 
        {
            if(saveableObject.eTag == null)
            {
                // If the ETag is null, then we're creating a new entity.
                saveableObject = await _dispatcher.CreateAsync(saveableObject);
            }
            else
            {
                // If the ETag is not null, then we're updating an existing entity.
                await _dispatcher.UpdateAsync(saveableObject);
            }
        }
        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}