using System.Collections.Generic;
using Gradeio.Quiz.WebApi.Models.Question;

namespace Gradeio.Quiz.WebApi.Models.Quiz
{
    public class CreateQuizModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsRanked { get; set; }

        public ICollection<CreateQuestionModel> Questions { get; set; }
    }
}