using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gradeio.Quiz.Domain.Entities;
using MongoDB.Driver;
using Vanguard;

namespace Gradeio.Quiz.DataAccess.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly IMongoCollection<QuizEntity> _quizzes;

        public QuizRepository(DatabaseContext databaseContext)
        {
            Guard.ArgumentNotNull(databaseContext, nameof(databaseContext));

            _quizzes = databaseContext.QuizEntities;
        }

        public async Task<ICollection<QuizEntity>> GetAsync(Guid userId)
        {
            var quizzes = await _quizzes.FindAsync(q => q.QuizCreatorId == userId).ConfigureAwait(false);

            return quizzes.ToList();
        }

        public async Task AddAsync(QuizEntity entity)
        {
            await _quizzes.InsertOneAsync(entity).ConfigureAwait(false);
        }

        public async Task<QuizEntity> GetByIdAsync(Guid id)
        {
            var quiz = await _quizzes.FindAsync(x => x.Id == id).ConfigureAwait(false);

            return quiz.FirstOrDefault();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _quizzes.DeleteOneAsync(x => x.Id == id).ConfigureAwait(false);
        }

        public async Task UpdateAsync(QuizEntity quiz)
        {
            await _quizzes.FindOneAndReplaceAsync(q => q.Id == quiz.Id, quiz).ConfigureAwait(false);
        }
    }
}
