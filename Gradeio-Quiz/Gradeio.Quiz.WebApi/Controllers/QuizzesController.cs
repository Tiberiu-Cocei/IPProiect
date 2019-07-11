using AutoMapper;
using Gradeio.Quiz.Business.Quiz;
using Gradeio.Quiz.Domain.Entities;
using Gradeio.Quiz.WebApi.Models.Quiz;
using Gradeio.Quiz.WebApi.UserExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vanguard;

namespace Gradeio.Quiz.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IMapper _mapper;
        private readonly IUserExtension _userValidation;

        public QuizzesController(IQuizService quizService, IMapper mapper, IUserExtension userValidation)
        {
            Guard.ArgumentNotNull(quizService, nameof(quizService));
            Guard.ArgumentNotNull(mapper, nameof(mapper));
            Guard.ArgumentNotNull(userValidation, nameof(userValidation));

            _quizService = quizService;
            _mapper = mapper;
            _userValidation = userValidation;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<QuizModel>), 200)]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserIdFromToken();
            var quizzes = await _quizService.ListAsync(userId).ConfigureAwait(false);

            return Ok(_mapper.Map<IEnumerable<QuizModel>>(quizzes));
        }

        [HttpGet("{id:guid}", Name = "GetQuizById")]
        [ProducesResponseType(typeof(QuizModel), 200)]
        public async Task<IActionResult> GetQuizById([FromRoute] Guid id)
        {
            var quiz = await _quizService.GetQuizAsync(id).ConfigureAwait(false);
            if (quiz == null)
            {
                return NotFound();
            }

            var userId = GetUserIdFromToken();
            //if (quiz.QuizCreatorId != userId)
            //{
            //    return Unauthorized();
            //}

            return Ok(_mapper.Map<QuizModel>(quiz));
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz([FromBody] CreateQuizModel quiz)
        {
            var quizEntity = _mapper.Map<QuizEntity>(quiz);
            quizEntity.QuizCreatorId = GetUserIdFromToken();

            await _quizService.CreateAsync(quizEntity).ConfigureAwait(false);

            var quizResult = await _quizService.GetQuizAsync(quizEntity.Id).ConfigureAwait(false);

            return CreatedAtRoute("GetQuizById", new { id = quizResult.Id }, _mapper.Map<QuizModel>(quizResult));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateQuiz([FromRoute] Guid id, [FromBody] UpdateQuizModel updatedQuiz)
        {
            if (updatedQuiz.Id != id)
            {
                return BadRequest();
            }

            var quiz = await _quizService.GetQuizAsync(id).ConfigureAwait(false);
            if (quiz == null)
            {
                return NotFound();
            }

            var userId = GetUserIdFromToken();
            if (quiz.QuizCreatorId != userId)
            {
                return Unauthorized();
            }

            var entity = _mapper.Map<QuizEntity>(updatedQuiz);
            entity.QuizCreatorId = userId;
            await _quizService.UpdateAsync(entity).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteQuiz([FromRoute] Guid id)
        {
            var quiz = await _quizService.GetQuizAsync(id).ConfigureAwait(false);
            if (quiz == null)
            {
                return NotFound();
            }

            var userId = GetUserIdFromToken();
            if (quiz.QuizCreatorId != userId)
            {
                return Unauthorized();
            }

            await _quizService.DeleteAsync(id).ConfigureAwait(false);

            return NoContent();
        }

        private Guid GetUserIdFromToken()
        {
            return _userValidation.GetUserIdFromClaims(HttpContext);
        }
    }
}
 