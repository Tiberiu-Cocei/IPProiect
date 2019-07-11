using MongoDB.Bson.Serialization.Attributes;

namespace Gradeio.Quiz.Domain.Entities
{
    public class Answer : BaseEntity
    {
        [BsonElement("AnswerText")]
        [BsonRequired]
        public string AnswerText { get; set; }

        [BsonElement("IsCorrect")]
        [BsonRequired]
        public bool IsCorrect { get; set; }
    }
}
