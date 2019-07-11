using Gradeio.Quiz.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Gradeio.Quiz.DataAccess
{
    public class DatabaseContext
    {
        public IMongoCollection<QuestionEntity> QuestionEntities { get; set; }
        public IMongoCollection<QuizEntity> QuizEntities { get; set; }

        public DatabaseContext(IConfiguration config)
        {
            var connectionString = config.GetSection("ConnectionStrings").Value;
            var databaseName = config.GetSection("DatabaseName").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            QuestionEntities = database.GetCollection<QuestionEntity>("QuestionEntities");
            QuizEntities = database.GetCollection<QuizEntity>("QuizEntities");
        }
    }
}
