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


        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            //önce TDto'yu TEntity'e dönüştür
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity); //Dto'dan dönüşmüş bir entity nesnesi oluştu
            await _genericRepository.AddAsync(newEntity); 
            await _unitOfWork.CommitAsync(); //tek bir transaction'da data kayıt oldu.

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Response<TDto>.Fail("Id not found", 404, true);
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id) //TDto alırsak id üzerinden silinecek olan datanın var olup olmadığını bilemeyebiliriz.
        {
            var isExistEntity = await _genericRepository.GetByIdAsync(id);
            if (isExistEntity == null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            _genericRepository.Remove(isExistEntity); //ilgili datayı memory state'te deleted olarak işaretlemiş olduk
            await _unitOfWork.CommitAsync(); //ilgili datanın memory state'teki deleted işlemini veri tabanına yansıtmış olduk.
            return Response<NoDataDto>.Success(200); //geriye herhangi bir data dönmeyecek, 200 kodunu göndermemiz client'lar için yeterli
        }

        public Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Response<TDto>> where(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
