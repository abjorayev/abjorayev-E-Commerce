using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.ApplicationService
{
    public interface IApplicationService<T>
    {
        Task<int> Create(T entity);
        Task Delete(int id);
        Task Update(T entity);
    }
}
