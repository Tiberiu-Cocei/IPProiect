using System;
using System.Collections.Generic;
using AutoMapper;
using Gradeio.Quiz.Domain.Entities;
using Gradeio.Quiz.WebApi.Models.Quiz;

namespace Gradeio.Quiz.WebApi.Mappers
{
    public class QuizMapperProfiler : Profile
    {
        public QuizMapperProfiler()
        {
            CreateMap<QuizEntity, QuizModel>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, map => map.MapFrom(src => src.QuizName))
                .ForMember(dest => dest.Description, map => map.MapFrom(src => src.QuizDescription))
                .ForMember(dest => dest.IsRanked, map => map.MapFrom(src => src.QuizIsRanked))
                .ForMember(dest => dest.CreationDate, map => map.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.NumberOfQuestions, map => map.MapFrom(src => src.NumberOfQuestions))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Mapper.Map<ICollection<QuestionEntity>>(src.QuestionList)))

                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateQuizModel, QuizEntity>()
                .ForMember(dest => dest.QuizName, map => map.MapFrom(src => src.Name))
                .ForMember(dest => dest.QuizDescription, map => map.MapFrom(src => src.Description))
                .ForMember(dest => dest.QuizIsRanked, map => map.MapFrom(src => src.IsRanked))
                .ForMember(dest => dest.CreationDate, map => map.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.NumberOfQuestions, map => map.MapFrom(src => src.Questions != null ? src.Questions.Count : 0))
                .ForMember(dest => dest.QuestionList, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Mapper.Map<ICollection<QuestionEntity>>(src.Questions)))

                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UpdateQuizModel, QuizEntity>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizName, map => map.MapFrom(src => src.Name))
                .ForMember(dest => dest.QuizDescription, map => map.MapFrom(src => src.Description))
                .ForMember(dest => dest.QuizIsRanked, map => map.MapFrom(src => src.IsRanked))
                .ForMember(dest => dest.CreationDate, map => map.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.NumberOfQuestions, map => map.MapFrom(src => src.Questions != null ? src.Questions.Count : 0))
                .ForMember(dest => dest.QuestionList, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Mapper.Map<ICollection<QuestionEntity>>(src.Questions)))

                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
