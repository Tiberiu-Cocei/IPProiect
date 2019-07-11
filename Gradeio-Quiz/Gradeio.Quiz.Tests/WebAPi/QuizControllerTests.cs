using AutoMapper;
using FluentAssertions;
using Gradeio.Quiz.Business.Quiz;
using Gradeio.Quiz.Domain.Entities;
using Gradeio.Quiz.WebApi.Controllers;
using Gradeio.Quiz.WebApi.Models.Question;
using Gradeio.Quiz.WebApi.Models.Quiz;
using Gradeio.Quiz.WebApi.UserExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Gradeio.Quiz.WebApi.ModelValidators.Quiz;
using System.Text;
using Gradeio.Quiz.WebApi.ModelValidators.Question;

namespace Gradeio.Quiz.Tests.WebAPi
{
    [TestClass]
    public class QuizControllerTests : BaseUnitTest<QuizzesController>
    {
        private Mock<IQuizService> _mockQuizService;
        private Mock<IUserExtension> _mockUserValidation;
        private Mock<IMapper> _mockMapper;

        private MockRepository _mockRepository;

        public override QuizzesController CreateSystemUnderTest()
        {
            return new QuizzesController(_mockQuizService.Object, _mockMapper.Object, _mockUserValidation.Object);
        }

        public override void SetupMocking()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _mockQuizService = _mockRepository.Create<IQuizService>();
            _mockMapper = _mockRepository.Create<IMapper>();
            _mockUserValidation = _mockRepository.Create<IUserExtension>();

            Mock<HttpContext> _mockHttpContext = _mockRepository.Create<HttpContext>();

            _mockUserValidation.Setup(x => x.GetUserIdFromClaims(It.IsAny<HttpContext>())).Returns(Guid.NewGuid());
        }

        public void AddQuizMocking(QuizEntity quizEntity, QuizModel quizModel)
        {
            _mockMapper.Setup(x => x.Map<QuizEntity>(It.IsAny<CreateQuizModel>()))
                .Returns(quizEntity);
            _mockMapper.Setup(x => x.Map<QuizModel>(It.IsAny<QuizEntity>()))
                .Returns(quizModel);

            _mockQuizService.Setup(x => x.GetQuizAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(quizEntity));
            _mockQuizService.Setup(x => x.CreateAsync(It.IsAny<QuizEntity>()))
                .Returns(Task.CompletedTask);
        }

        public void UpdateQuizMocking(QuizEntity quizEntity)
        {
            _mockMapper.Setup(x => x.Map<QuizEntity>(It.IsAny<CreateQuizModel>()))
                .Returns(quizEntity);

            _mockQuizService.Setup(x => x.GetQuizAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(quizEntity));
            _mockQuizService.Setup(x => x.UpdateAsync(quizEntity))
                .Returns(Task.CompletedTask);
        }

        public String stringOfLength(int length)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append('i');
            }

            return stringBuilder.ToString();
        }

        public int randomBetween(int begin, int end)
        {
            var r = new Random();
            var result = begin + r.Next() % (end - begin);

            return result;
        }

        [TestMethod]
        public void When_ConstructorIsCalledWithNullParamether_Then_ShouldThrowArgumentNullException()
        {
            // Arrange

            // Act
            Action action = () => new QuizzesController(null, null, null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "quizService");
        }

        [TestMethod]
        public async Task When_AddMethodIsCalledWithValidParameter_Then_ShouldReturnCreatedAt()
        {
            // Arrange
            CreateQuizModel entity = new CreateQuizModel
            {
                Name = "valid name",
                Description = "valid description",
                Questions = new List<CreateQuestionModel>
                {
                   new CreateQuestionModel()
                },
                IsRanked = false
            };

            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            var quizModel = new QuizModel
            {
            };

            AddQuizMocking(quizEntity, quizModel);

            // Act
            var result = await SystemUnderTest.AddQuiz(entity);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();
            _mockQuizService.Verify(x => x.CreateAsync(It.IsAny<QuizEntity>()), Times.Exactly(1));
            _mockQuizService.Verify(x => x.GetQuizAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_GetMethodIsCalledWithValidParameter_Then_ShouldReturnOkObjectResult()
        {
            // Arrange
            _mockMapper.Setup(x => x.Map<IEnumerable<QuizModel>>(It.IsAny<QuizEntity>()))
                .Returns(It.IsAny<IEnumerable<QuizModel>>);

            _mockQuizService.Setup(x => x.ListAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(It.IsAny<ICollection<QuizEntity>>()));

            // Act
            var result = await SystemUnderTest.Get();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockQuizService.Verify(x => x.ListAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_GetQuizByIdMethodIsCalledCorrectly_Then_ShouldReturnOkObjectResult()
        {
            // Arrange
            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            _mockMapper.Setup(x => x.Map<QuizModel>(It.IsAny<QuizEntity>()))
                .Returns(It.IsAny<QuizModel>);

            _mockQuizService.Setup(x => x.GetQuizAsync(quizEntity.QuizCreatorId))
                .Returns(Task.FromResult(quizEntity));

            _mockUserValidation.Setup(x => x.GetUserIdFromClaims(It.IsAny<HttpContext>())).Returns(quizEntity.QuizCreatorId);

            // Act
            var result = await SystemUnderTest.GetQuizById(quizEntity.QuizCreatorId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockQuizService.Verify(x => x.GetQuizAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_GetQuizByIdMethodIsCalledWithDifferentUserId_Then_ShouldReturnUnauthorizedResult()
        {
            // Arrange
            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            _mockQuizService.Setup(x => x.GetQuizAsync(quizEntity.QuizCreatorId))
                .Returns(Task.FromResult(quizEntity));

            // Act
            var result = await SystemUnderTest.GetQuizById(quizEntity.QuizCreatorId);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [TestMethod]
        public async Task When_GetQuizByIdMethodIsCalledWithNoQuizFound_Then_ShouldReturnNotFound()
        {
            // Arrange
            _mockQuizService.Setup(x => x.GetQuizAsync(new Guid()))
                .Returns(Task.FromResult((QuizEntity)null));

            // Act
            var result = await SystemUnderTest.GetQuizById(new Guid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task When_DeleteQuizMethodIsCalledCorrectly_Then_ShouldReturnNoContentResult()
        {
            // Arrange
            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            _mockMapper.Setup(x => x.Map<QuizModel>(It.IsAny<QuizEntity>()))
                .Returns(It.IsAny<QuizModel>);

            _mockQuizService.Setup(x => x.GetQuizAsync(quizEntity.QuizCreatorId))
                .Returns(Task.FromResult(quizEntity));
            _mockQuizService.Setup(x => x.DeleteAsync(quizEntity.QuizCreatorId))
                .Returns(Task.CompletedTask);

            _mockUserValidation.Setup(x => x.GetUserIdFromClaims(It.IsAny<HttpContext>())).Returns(quizEntity.QuizCreatorId);

            // Act
            var result = await SystemUnderTest.DeleteQuiz(quizEntity.QuizCreatorId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockQuizService.Verify(x => x.GetQuizAsync(It.IsAny<Guid>()), Times.Exactly(1));
            _mockQuizService.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_DeleteQuizMethodIsCalledWithNoQuizFound_Then_ShouldReturnNotFound()
        {
            // Arrange
            _mockQuizService.Setup(x => x.GetQuizAsync(new Guid()))
                .Returns(Task.FromResult((QuizEntity)null));

            // Act
            var result = await SystemUnderTest.DeleteQuiz(new Guid());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task When_DeleteQuizMethodIsCalledWithDifferentUserId_Then_ShouldReturnUnauthorizedResult()
        {
            // Arrange
            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            _mockMapper.Setup(x => x.Map<QuizModel>(It.IsAny<QuizEntity>()))
                .Returns(It.IsAny<QuizModel>);

            _mockQuizService.Setup(x => x.GetQuizAsync(quizEntity.QuizCreatorId))
                .Returns(Task.FromResult(quizEntity));
            _mockQuizService.Setup(x => x.DeleteAsync(quizEntity.QuizCreatorId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SystemUnderTest.DeleteQuiz(quizEntity.QuizCreatorId);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [TestMethod]
        public async Task When_UpdateMethodIsCalledWithValidParameter_Then_ShouldReturnNoContent()
        {
            // Arrange
            UpdateQuizModel entity = new UpdateQuizModel
            {
                Id = new Guid(),
                Name = "valid name",
                Description = "valid description",
                Questions = new List<CreateQuestionModel>
                {
                   new CreateQuestionModel()
                },
                IsRanked = false
            };

            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            UpdateQuizMocking(quizEntity);

            _mockUserValidation.Setup(x => x.GetUserIdFromClaims(It.IsAny<HttpContext>())).Returns(quizEntity.QuizCreatorId);

            // Act
            var result = await SystemUnderTest.UpdateQuiz(entity.Id, entity);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockQuizService.Verify(x => x.UpdateAsync(It.IsAny<QuizEntity>()), Times.Exactly(1));
            _mockQuizService.Verify(x => x.GetQuizAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_UpdateMethodIsCalledWithNoAuthorization_Then_ShouldReturnUnauthorizedResult()
        {
            // Arrange
            UpdateQuizModel entity = new UpdateQuizModel
            {
                Id = new Guid(),
                Name = "valid name",
                Description = "valid description",
                Questions = new List<CreateQuestionModel>
                {
                   new CreateQuestionModel()
                },
                IsRanked = false
            };

            var quizEntity = new QuizEntity
            {
                QuizCreatorId = Guid.NewGuid()
            };

            UpdateQuizMocking(quizEntity);

            // Act
            var result = await SystemUnderTest.UpdateQuiz(entity.Id, entity);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [TestMethod]
        public async Task When_UpdateMethodIsCalledWithNoQuizFound_Then_ShouldReturnNotFound()
        {
            // Arrange
            UpdateQuizModel entity = new UpdateQuizModel
            {
                Id = new Guid(),
                Name = "valid name",
                Description = "valid description",
                Questions = new List<CreateQuestionModel>
                {
                   new CreateQuestionModel()
                },
                IsRanked = false
            };

            _mockQuizService.Setup(x => x.GetQuizAsync(new Guid()))
                .Returns(Task.FromResult((QuizEntity)null));

            // Act
            var result = await SystemUnderTest.UpdateQuiz(entity.Id, entity);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public async Task When_UpdateMethodIsCalledWithDifferentIds_Then_ShouldReturnBadRequest()
        {
            // Arrange
            UpdateQuizModel entity = new UpdateQuizModel
            {
                Id = new Guid(),
                Name = "valid name",
                Description = "valid description",
                Questions = new List<CreateQuestionModel>
                {
                   new CreateQuestionModel()
                },
                IsRanked = false
            };

            // Act
            var result = await SystemUnderTest.UpdateQuiz(Guid.Parse("ce17d9f9-449c-4c78-b9eb-94c14f020eb2"), entity);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_NameIsNull_Should_HaveError()
        {
            // Arrange

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, null as string);
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_NameIsEmpty_Should_HaveError()
        {
            // Arrange

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, "" as string);
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_NameIsTooLong_Should_HaveError()
        {
            // Arrange
            var tooLongString = stringOfLength(51);

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, tooLongString);
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_NameIsPerfect_Should_NotHaveError()
        {
            // Arrange

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldNotHaveValidationErrorFor(x => x.Name, "good name");
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_DescriptionIsTooLong_Should_HaveError()
        {
            // Arrange
            var tooLongString = stringOfLength(201);

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Description, tooLongString);
        }

        [TestMethod]
        public void CreateQuizModelValidator_When_DescriptionIsPerfect_Should_NotHaveError()
        {
            // Arrange

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldNotHaveValidationErrorFor(x => x.Description, "good description");
        }

        //public void CreateQuizModelValidator_ShouldHaveErrorWhenIsRankedIsNull()

        [TestMethod]
        public void CreateQuizModelValidator_When_QuestionsIsEmpty_Should_HaveError()
        {
            // Arrange
            var questions = new List<CreateQuestionModel> { };

            // Act
            CreateQuizModelValidator createQuizModelValidator = new CreateQuizModelValidator();

            // Assert
            createQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Questions, questions);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_QuestionTextIsEmpty_Should_HaveError()
        {
            // Arrange

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.QuestionText,
                new CreateQuestionModel
                {
                    QuestionText = "" as string,
                    AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
                }
                );
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_QuestionTextIsTooLong_Should_HaveError()
        {
            // Arrange        
            var tooLongString = stringOfLength(101);

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.QuestionText,
                 new CreateQuestionModel
                 {
                     QuestionText = tooLongString,
                     AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
                 });
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_QuestionTextIsTooShort_Should_HaveError()
        {
            // Arrange
            var tooshortString = stringOfLength(2);

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.QuestionText,
                 new CreateQuestionModel
                 {
                     QuestionText = tooshortString,
                     AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
                 }
                );
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_QuestionTextIsPerfect_Should_NotHaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                QuestionText = stringOfLength(randomBetween(5, 100)),
                AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldNotHaveValidationErrorFor(x => x.QuestionText, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_TheScoreIsTooLow_Should_HaveError_()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                QuestionText = stringOfLength(randomBetween(5, 100)),
                Score = 50,
                AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.Score, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_TheScoreIsEmpty_Should_HaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                QuestionText = stringOfLength(randomBetween(5, 100)),
                AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.Score, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_TheTimeIsEmpty_Should_HaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.Time, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_TheTimeIsTooShort_Should_HaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                Time = 4,
                AnswerList = new List<AnswerModel>
                    {
                       new AnswerModel()
                    }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.Time, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_AnswerListIsEmpty_Should_HaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                AnswerList = new List<AnswerModel>
                {
                }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.AnswerList, question);
        }

        [TestMethod]
        public void CreateQuestionModelValidator_When_AnswerListDoesNotContainValidAnswers_Should_HaveError()
        {
            // Arrange
            var question = new CreateQuestionModel
            {
                AnswerList = new List<AnswerModel>
                {
                    new AnswerModel()
                    {
                        IsCorrect = false,
                        AnswerText = "valid text"
                    }
                }
            };

            // Act
            CreateQuestionModelValidator createQuestionModelValidator = new CreateQuestionModelValidator();

            // Assert
            createQuestionModelValidator.ShouldHaveValidationErrorFor(x => x.AnswerList, question);
        }

        [TestMethod]
        public void AnswerModelValidator_When_AnswerTextIsEmpty_Should_HaveError()
        {
            // Arrange

            // Act
            AnswerModelValidator answerModelValidator = new AnswerModelValidator();

            // Assert
            answerModelValidator.ShouldHaveValidationErrorFor(x => x.AnswerText, "" as string);
        }

        [TestMethod]
        public void AnswerModelValidator_When_AnswerTextIsTooLong_Should_HaveError()
        {
            // Arrange
            var tooLongString = stringOfLength(51);

            // Act
            AnswerModelValidator answerModelValidator = new AnswerModelValidator();

            // Assert
            answerModelValidator.ShouldHaveValidationErrorFor(x => x.AnswerText, tooLongString);
        }

        [TestMethod]
        public void AnswerModelValidator_When_AnswerTextIsTooShort_Should_HaveError()
        {
            // Arrange
            var tooShortString = stringOfLength(0);

            // Act
            AnswerModelValidator answerModelValidator = new AnswerModelValidator();

            // Assert
            answerModelValidator.ShouldHaveValidationErrorFor(x => x.AnswerText, tooShortString);
        }

        [TestMethod]
        public void AnswerModelValidator_When_AnswerTextPerfect_Should_NotHaveError()
        {
            // Arrange
            var answerText = stringOfLength(randomBetween(1, 50));

            // Act
            AnswerModelValidator answerModelValidator = new AnswerModelValidator();

            // Assert
            answerModelValidator.ShouldNotHaveValidationErrorFor(x => x.AnswerText, answerText);
        }

        [TestMethod]
        public void AnswerModelValidator_When_IsCorrectIsNotNull_Should_NotHaveError()
        {
            // Arrange
            bool valueOfTruth = (randomBetween(0, 1) == 1);

            // Act
            AnswerModelValidator answerModelValidator = new AnswerModelValidator();

            // Assert
            answerModelValidator.ShouldNotHaveValidationErrorFor(x => x.IsCorrect, valueOfTruth);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_NameIsEmpty_Then_Should_HaveError()
        {
            // Arrange

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, "" as string);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_NameIsTooShort_Then_Should_HaveError()
        {
            // Arrange
            var tooShortString = stringOfLength(0);

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, tooShortString);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_NameIsTooLong_Then_Should_HaveError()
        {
            // Arrange
            var tooLongString = stringOfLength(51);

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Name, tooLongString);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_NameIsPerfect_Then_Should_NotHaveError()
        {
            // Arrange
            var validName = stringOfLength(randomBetween(1, 50));

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldNotHaveValidationErrorFor(x => x.Name, validName);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_DescriptionIsTooLong_Then_Should_HaveError()
        {
            // Arrange
            var tooLongString = stringOfLength(201);

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Description, tooLongString);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_DescriptionIsPerfect_Then_Should_NotHaveError()
        {
            // Arrange
            var validDescriptione = stringOfLength(randomBetween(0, 200));

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldNotHaveValidationErrorFor(x => x.Description, validDescriptione);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_IsRankedIsNotNull_Then_Should_NotHaveError()
        {
            // Arrange
            bool valueOfTruth = (randomBetween(0, 1) == 1);

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldNotHaveValidationErrorFor(x => x.IsRanked, valueOfTruth);
        }

        [TestMethod]
        public void UpdateQuizModelValidator_When_QuestionsListIsEmpty_Then_Should_HaveError()
        {
            // Arrange
            var questions = new List<CreateQuestionModel> { };

            // Act
            UpdateQuizModelValidator updateQuizModelValidator = new UpdateQuizModelValidator();

            // Assert
            updateQuizModelValidator.ShouldHaveValidationErrorFor(x => x.Questions, questions);
        }
    }
}