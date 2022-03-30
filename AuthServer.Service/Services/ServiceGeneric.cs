using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
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
        //save changes, unit of work, reporsitory'i çağıracağımız class. veri tabanı ile iletişim kurulacak yer.
        private readonly IUnitOfWork _unitOfWork; //başında "_" olması private olduğu için bir yazım kuralıdır.
        private readonly IGenericRepository<TEntity> _genericRepository;
        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _genericRepository = genericRepository;
            _unitOfWork = unitOfWork;
        }


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
