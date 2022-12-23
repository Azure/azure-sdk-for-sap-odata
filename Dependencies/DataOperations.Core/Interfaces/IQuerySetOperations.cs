using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataOperations
{
    public interface IQuerySetOperations<T> where T : IBaseDTOWithIDAndETag
    {
        Task<T> CreateAsync(T entity);
        Task DeleteAsync(string ID, string eTag);
        Task<T> GetAsync(string Key = null, QueryExpand expand = null, QuerySelect select = null);
        Task<IEnumerable<T>> GetListAsync(QueryTop top = null, QuerySkip skip = null, QueryOrderBy orderBy = null, QueryFilter filter = null, QueryExpand expand = null, QuerySelect select = null);
        Task UpdateAsync(T entity);
    }
}