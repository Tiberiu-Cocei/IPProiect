using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gradeio.Quiz.DataAccess
{
    public interface IBaseRepository<TEntity>
    {
        Task<ICollection<TEntity>> GetAsync(Guid userId);

        Task<TEntity> GetByIdAsync(Guid id);

        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(Guid id);
    }
}
