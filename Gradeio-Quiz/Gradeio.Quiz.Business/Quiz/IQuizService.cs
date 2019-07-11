using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gradeio.Quiz.Domain.Entities;

namespace Gradeio.Quiz.Business.Quiz
{
    public interface IQuizService
    {
        Task<ICollection<QuizEntity>> ListAsync(Guid userId);

        Task<QuizEntity> GetQuizAsync(Guid id);

        Task CreateAsync(QuizEntity quiz);

        Task UpdateAsync(QuizEntity quiz);

        Task DeleteAsync(Guid id);
    }
}
