using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gradeio.Quiz.Domain.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public Guid Id { get; set; }
    }
}
