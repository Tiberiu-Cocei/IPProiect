using FluentAssertions;
using Gradeio.Quiz.Business.Quiz;
using Gradeio.Quiz.DataAccess.Repositories;
using Gradeio.Quiz.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gradeio.Quiz.Tests.Business
{
    [TestClass]
    public class QuizServiceTests : BaseUnitTest<QuizService>
    {
        private Mock<IQuizRepository> _mockQuizRepository;

        public override QuizService CreateSystemUnderTest()
        {
            return new QuizService(_mockQuizRepository.Object);
        }

        public override void SetupMocking()
        {
            _mockQuizRepository = new Mock<IQuizRepository>();
        }

        [TestMethod]
        public void When_ConstructorIsCalledWithNullParamether_The_ShouldThrowArgumentNullException()
        {
            // Arrange

            // Act
            Action action = () => new QuizService(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "repository");
        }

        [TestMethod]
        public void When_CallCreateAsyncWithNullQuiz_Then_ShouldThrowArgumentNullException()
        {
            // Arrange

            // Act
            Func<Task> action = async () => await SystemUnderTest.CreateAsync(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "quiz");
        }

        
        [TestMethod]
        public async Task When_CallDeleteQuizByIdAndTheQuizIsInDataBase_Then_ShouldDeleteTheQuizAsync()
        {
            // Arrange
            var entity = new QuizEntity();

            //-- nu merge sa returnezze entitatea.--
            _mockQuizRepository.Setup(x => x.GetByIdAsync(entity.Id)).Returns(Task.FromResult(entity));

            // Act
           // var result = await SystemUnderTest.CreateAsync(entity);
            await SystemUnderTest.DeleteAsync(entity.Id);

            // Assert
            _mockQuizRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.AtMost(1));
            _mockQuizRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }


        [TestMethod]
        public async Task When_CallUpdateQuiz_Then_ShouldUpdate()
        {
            // Arrange
            QuizEntity quizEntityOriginal = new QuizEntity
            {
                QuizName = "Nume intrebare de inlocuit",
                QuizDescription = "Descriere de inlocuit",
                QuestionList = new List<QuestionEntity> {
                    new QuestionEntity()
                },
                QuizIsRanked = false,
                CreationDate = new DateTime(),
                QuizCreatorId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            QuizEntity quizEntityNew = new QuizEntity
            {
                QuizName = "Nume inlocuit",
                QuizDescription = "Descriere inlocuita",
                QuestionList = new List<QuestionEntity> {
                    new QuestionEntity()
                },
                QuizIsRanked = true,
                CreationDate = quizEntityOriginal.CreationDate,
                QuizCreatorId = quizEntityOriginal.QuizCreatorId,
                Id = quizEntityOriginal.Id
            };

            _mockQuizRepository.Setup(x => x.GetByIdAsync(quizEntityOriginal.Id)).Returns(Task.FromResult(quizEntityNew));

            // Act
            await SystemUnderTest.UpdateAsync(quizEntityNew);

            // Assert
            _mockQuizRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task When_CallAddQuizAndTheQuizIsNotPresentInTheDatabase_Then_ShouldCreate()
        {
            // Arrange
            var entity = new QuizEntity();

            // Act
            await SystemUnderTest.CreateAsync(entity);

            // Assert
            _mockQuizRepository.Verify(x => x.AddAsync(It.IsAny<QuizEntity>()), Times.Once);
        }
    }
}
