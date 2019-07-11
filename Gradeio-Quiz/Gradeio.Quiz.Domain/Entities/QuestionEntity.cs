using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Gradeio.Quiz.Domain.Entities
{
    public class QuestionEntity : BaseEntity
    {
        public QuestionEntity()
            : base()
        {
        }

        [BsonElement("QuestionText")]
        [BsonRequired]
        public string QuestionText { get; set; }

        [BsonElement("QuestionScore")]
        [BsonRequired]
        public int QuestionScore { get; set; }

        [BsonElement("QuestionTime")]
        [BsonRequired]
        public int QuestionTime { get; set; }

        [BsonElement("NumberOfCorrectAnswers")]
        [BsonRequired]
        public int NumberOfCorrectAnswers { get; set; }

        [BsonElement("AnswerList")]
        [BsonRequired]
        public ICollection<Answer> AnswerList { get; set; }
    }
}
