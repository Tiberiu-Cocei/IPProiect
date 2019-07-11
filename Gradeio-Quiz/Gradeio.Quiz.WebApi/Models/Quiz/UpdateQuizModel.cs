using System;

namespace Gradeio.Quiz.WebApi.Models.Quiz
{
    public class UpdateQuizModel : CreateQuizModel
    {
        public Guid Id { get; set; }
    }
}
