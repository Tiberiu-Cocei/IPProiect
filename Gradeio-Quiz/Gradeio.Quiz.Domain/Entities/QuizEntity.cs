using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gradeio.Quiz.Domain.Entities
{
    public class QuizEntity : BaseEntity
    {
        public QuizEntity()
            : base()
        {
        }

        [BsonElement("QuizName")]
        [BsonRequired]
        public string QuizName { get; set; }

        [BsonElement("QuizDescription")]
        public string QuizDescription { get; set; }

        [BsonElement("QuestionList")]
        [BsonRequired]
        public ICollection<QuestionEntity> QuestionList { get; set; }

        [BsonElement("QuizIsRanked")]
        [BsonRequired]
        public bool QuizIsRanked { get; set; }

        [BsonElement("CreationDate")]
        [BsonRequired]
        public DateTime CreationDate { get; set; }

        [BsonElement("QuizCreatorId")]
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Guid QuizCreatorId { get; set; }

        [BsonElement("NumberOfQuestions")]
        [BsonRequired]
        public int NumberOfQuestions { get; set; }
    }
}
