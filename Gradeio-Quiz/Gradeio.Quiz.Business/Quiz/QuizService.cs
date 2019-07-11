using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gradeio.Quiz.DataAccess.Repositories;
using Gradeio.Quiz.Domain.Entities;
using Vanguard;

namespace Gradeio.Quiz.Business.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repository;

        public QuizService(IQuizRepository repository)
        {
            Guard.ArgumentNotNull(repository, nameof(repository));

            _repository = repository;
        }

        public async Task<ICollection<QuizEntity>> ListAsync(Guid userId)
        {
            return await _repository.GetAsync(userId).ConfigureAwait(false);
        }

        public async Task<QuizEntity> GetQuizAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id).ConfigureAwait(false);
        }

        public async Task CreateAsync(QuizEntity quiz)
        {
            Guard.ArgumentNotNull(quiz, nameof(quiz));

            await _repository.AddAsync(quiz).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid id)
        {
            var quiz = await _repository.GetByIdAsync(id).ConfigureAwait(false);
            if (quiz != null)
            {
                await _repository.DeleteAsync(id).ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(QuizEntity quiz)
        {
            Guard.ArgumentNotNull(quiz, nameof(quiz));

            var insertedQuiz = await _repository.GetByIdAsync(quiz.Id).ConfigureAwait(false);
            if (insertedQuiz != null)
            {
                await _repository.UpdateAsync(quiz).ConfigureAwait(false);
            }
        }
    }
}