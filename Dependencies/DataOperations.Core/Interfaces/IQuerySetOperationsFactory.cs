namespace DataOperations
{
    public interface IQuerySetOperationsFactory
    {
        IQuerySetOperations<T> Create<T>() where T : IBaseDTOWithIDAndETag;
    }

}