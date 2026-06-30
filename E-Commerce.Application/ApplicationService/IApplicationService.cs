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
        Task<bool> Delete(int id);
        Task<bool> Update(T entity);
    }
}
