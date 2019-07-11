using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Gradeio.Quiz.WebApi.Models.Question;

namespace Gradeio.Quiz.WebApi.ModelValidators.Question
{
    public class CreateQuestionModelValidator : AbstractValidator<CreateQuestionModel>
    {
        public CreateQuestionModelValidator()
        {
            RuleFor(x => x.QuestionText).NotEmpty().Length(5, 100);
            RuleFor(x => x.Score).NotEmpty().GreaterThanOrEqualTo(100);
            RuleFor(x => x.Time).NotEmpty().GreaterThanOrEqualTo(5);
            RuleFor(x => x.AnswerList).NotEmpty();
            RuleForEach(x => x.AnswerList).SetValidator(new AnswerModelValidator());
            RuleFor(x => x.AnswerList).Must(ContainsAtLeastOneCorrectAnswer);
        }

        private bool ContainsAtLeastOneCorrectAnswer(ICollection<AnswerModel> answers)
        {
            return answers.Where(x => x.IsCorrect == true).Count() > 0;
        }
    }
}