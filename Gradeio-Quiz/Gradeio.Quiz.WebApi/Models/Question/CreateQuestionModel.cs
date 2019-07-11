using System.Collections.Generic;

namespace Gradeio.Quiz.WebApi.Models.Question
{
    public class CreateQuestionModel
    {
        public string QuestionText { get; set; }

        public int Score { get; set; }

        public int Time { get; set; }

        public ICollection<AnswerModel> AnswerList { get; set; }
    }
}
