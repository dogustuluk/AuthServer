using AuthServer.Core.Services;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto> : IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        public Task<Response<TDto>> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<TDto>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<NoDataDto>> Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response<NoDataDto>> Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Response<TDto>> where(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
