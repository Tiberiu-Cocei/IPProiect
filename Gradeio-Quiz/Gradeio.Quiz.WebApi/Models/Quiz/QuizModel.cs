using System;
using System.Collections.Generic;
using Gradeio.Quiz.WebApi.Models.Question;

namespace Gradeio.Quiz.WebApi.Models.Quiz
{
    public sealed class QuizModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsRanked { get; set; }

        public DateTime CreationDate { get; set; }

        public int NumberOfQuestions { get; set; }

        public ICollection<QuestionModel> Questions { get; set;  }
    }
}
