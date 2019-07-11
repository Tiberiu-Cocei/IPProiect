using System.Collections.Generic;

namespace Gradeio.Quiz.WebApi.Models.Question
{
    public class QuestionModel
    {
        public string QuestionText { get; set; }

        public int Score { get; set; }

        public int Time { get; set; }

        public int NumberOfCorrectAnswers { get; set; }

        public ICollection<AnswerModel> Answers { get; set; }
    }
}
