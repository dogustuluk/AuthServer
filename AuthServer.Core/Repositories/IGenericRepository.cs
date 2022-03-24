using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync(); //IEnumerable yaptık çünkü tüm data gelince herhangi bir sorgu yapmak istemiyorum.
        IQueryable<TEntity>where(Expression<Func<TEntity,bool>>predicate); //Iqueryable olmasının sebebi;
        //IQueryable'da istenildiği kadar where şartı yazılabilir, bunlar hemen veri tabanına hemen yansımaz taaki ne zaman tolist denir, o zaman yansır.
        //product = productRepository.where (x => x.id >=5);
        //product.any();
        //...
        //product.Tol; >>>>>>>>>>>>>>>dedikten sonra yazılan tüm sorgular artık tek bir sorguya dönüştürülür ve ardından çağrılır.
        
        
        //Delegate'lere çalış


        Task AddAsync(TEntity entity); //memory'e direkt olarak data eklediğinden dolayı async metoduna sahiptir. Yapılan her bir işlem memory'de
                                       //track ediliyor.
        void Remove(TEntity entity); //products.remove(product) >> burada remove ile ilgili entity'nin state'i deleted olur ve save
                                     //changes yapmayana değin silinmiş olmaz. Dolayısıyla bunun bir asyn metodu yoktur.
        TEntity Update(TEntity entity);

    }
}
