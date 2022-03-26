using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        //burda yapılan işlemler memory'de tutulur, veri tabanına kaydedilmez. 
        //async metotlar ile thread bloklanmaz.

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        //public IQueryable<TEntity> GetAllAsync()
        //{
        //    return _dbSet.AsQueryable();
        //}
        public async Task<IEnumerable<TEntity>> GetAllAsync() //tam performans almak için IQueryable dönmek lazım. üstteki satır
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached; //detached metodu ile track işlemi yapılmaz.
            }
            return entity;
        }

        //async metot vermeye gerek yoktur, direkt olarak ilgili alanda işlem uygulanacaktır.
        public void Remove(TEntity entity)
        {
            //_context.Entry(entity).State = EntityState.Deleted
            _dbSet.Remove(entity); //üstteki komut ile aynı şekilde çalışır
        }

        public TEntity Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> where(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
