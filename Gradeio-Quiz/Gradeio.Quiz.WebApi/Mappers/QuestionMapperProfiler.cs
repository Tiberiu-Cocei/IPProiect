using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Gradeio.Quiz.Domain.Entities;
using Gradeio.Quiz.WebApi.Models.Question;

namespace Gradeio.Quiz.WebApi.Mappers
{
    public class QuestionMapperProfiler : Profile
    {
        public QuestionMapperProfiler()
        {
            CreateMap<AnswerModel, Answer>()
                .ForMember(dest => dest.AnswerText, map => map.MapFrom(src => src.AnswerText))
                .ForMember(dest => dest.IsCorrect, map => map.MapFrom(src => src.IsCorrect))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Answer, AnswerModel>()
                .ForMember(dest => dest.AnswerText, map => map.MapFrom(src => src.AnswerText))
                .ForMember(dest => dest.IsCorrect, map => map.MapFrom(src => src.IsCorrect))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateQuestionModel, QuestionEntity>()
                .ForMember(dest => dest.QuestionText, map => map.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.QuestionScore, map => map.MapFrom(src => src.Score))
                .ForMember(dest => dest.QuestionTime, map => map.MapFrom(src => src.Time))
                .ForMember(dest => dest.NumberOfCorrectAnswers, map => map.MapFrom(src => src.AnswerList.Count(x => x.IsCorrect == true)))
                .ForMember(dest => dest.AnswerList, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Mapper.Map<ICollection<AnswerModel>>(src.AnswerList)))

                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<QuestionEntity, QuestionModel>()
                .ForMember(dest => dest.QuestionText, map => map.MapFrom(src => src.QuestionText))
                .ForMember(dest => dest.Score, map => map.MapFrom(src => src.QuestionScore))
                .ForMember(dest => dest.Time, map => map.MapFrom(src => src.QuestionTime))
                .ForMember(dest => dest.NumberOfCorrectAnswers, map => map.MapFrom(src => src.NumberOfCorrectAnswers))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Mapper.Map<ICollection<AnswerModel>>(src.AnswerList)))

                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
