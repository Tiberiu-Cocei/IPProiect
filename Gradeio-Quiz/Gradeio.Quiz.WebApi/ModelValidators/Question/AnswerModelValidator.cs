using FluentValidation;
using Gradeio.Quiz.WebApi.Models.Question;

namespace Gradeio.Quiz.WebApi.ModelValidators.Question
{
    public class AnswerModelValidator : AbstractValidator<AnswerModel>
    {
        public AnswerModelValidator()
        {
            RuleFor(x => x.AnswerText).NotEmpty().Length(1, 50);
            RuleFor(x => x.IsCorrect).NotNull();
        }
    }
}
