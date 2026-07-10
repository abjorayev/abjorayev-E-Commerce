using ECommerce.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Repository
{
    public class ECommerceRepository<T> : IECommerceRepository<T> where T : class
    {
        private readonly ApplicationContext _context;

        public ECommerceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
           // await _context.SaveChangesAsync();
        }

        public async Task AddRange(List<T> values)
        {
            await _context.Set<T>().AddRangeAsync(values);
           // await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
          //  await _context.SaveChangesAsync();
        }

        public async Task DeleteRange(List<T> values)
        {
             _context.Set<T>().RemoveRange(values);
           // await _context.SaveChangesAsync();
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }

        public async Task Update(T entity)
        {
            _context.Set<T>().Update(entity);
          //  await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(List<T> values)
        {
            _context.Set<T>().UpdateRange(values);
           // await _context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
