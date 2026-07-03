using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Repository
{
    public interface IECommerceRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task AddRange(List<T> values);
        Task DeleteRange(List<T> values);
    }
}
