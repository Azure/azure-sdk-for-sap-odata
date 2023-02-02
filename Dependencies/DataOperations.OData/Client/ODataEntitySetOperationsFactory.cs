namespace DataOperations.OData
{
    // This is the factory that is injected into the DI container to create the IODataEntitySetOperations<T> for each type of DTO
    public class ODataEntitySetOperationsFactory : IQuerySetOperationsFactory
    {
        private IOperationsDispatcher _dispatcher;
        public ODataEntitySetOperationsFactory(IOperationsDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IQuerySetOperations<T> Create<T>() where T : IBaseDTOWithIDAndETag
        {
            return new ODataEntitySetOperations<T>(_dispatcher);
        }
    }

}