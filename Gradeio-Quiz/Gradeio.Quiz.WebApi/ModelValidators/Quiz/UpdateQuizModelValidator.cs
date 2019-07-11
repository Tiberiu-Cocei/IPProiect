using FluentValidation;
using Gradeio.Quiz.WebApi.Models.Quiz;
using Gradeio.Quiz.WebApi.ModelValidators.Question;

namespace Gradeio.Quiz.WebApi.ModelValidators.Quiz
{
    public class UpdateQuizModelValidator : AbstractValidator<UpdateQuizModel>
    {
        public UpdateQuizModelValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotEmpty().Length(1, 50);
            RuleFor(x => x.Description).Length(0, 200);
            RuleFor(x => x.IsRanked).NotNull();
            RuleFor(x => x.Questions).NotEmpty();
            RuleForEach(x => x.Questions).SetValidator(new CreateQuestionModelValidator());
        }
    }
}
