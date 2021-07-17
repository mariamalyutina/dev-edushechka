﻿using System;
using System.Globalization;
using AutoMapper;
using DevEdu.API.Models.InputModels;
using DevEdu.API.Models.OutputModels;
using DevEdu.DAL.Models;

namespace DevEdu.API.Configuration
{
    public class MapperProfile : Profile
    {
        private const string _dateFormat = "dd.MM.yyyy";
        public MapperProfile()
        {
            CreateMappingToDto();
            CreateMappingFromDto();
        }

        private void CreateMappingToDto()
        { 
            CreateMap<AbsenceReasonInputModel, StudentLessonDto>();
            CreateMap<AttendanceInputModel, StudentLessonDto>();
            CreateMap<CourseInputModel, CourseDto>();
            CreateMap<CourseTopicInputModel, CourseTopicDto>();
            CreateMap<CommentAddInputModel, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDto { Id = src.UserId }));
            CreateMap<CommentUpdateInputModel, CommentDto>();
            CreateMap<FeedbackInputModel, StudentLessonDto>();
            CreateMap<GroupInputModel, GroupDto>();
            CreateMap<MaterialInputModel, MaterialDto>();
            CreateMap<NotificationAddInputModel, NotificationDto>();
            CreateMap<NotificationUpdateInputModel, NotificationDto>();
            CreateMap<StudentAnswerOnTaskInputModel, StudentAnswerOnTaskDto>();
            CreateMap<LessonInputModel, LessonDto>()
                .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => new UserDto { Id = src.TeacherId }))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => Convert.ToDateTime(src.Date)));
            CreateMap<LessonUpdateInputModel, LessonDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => Convert.ToDateTime(src.Date)));
            CreateMap<TagInputModel, TagDto>();
            CreateMap<TaskInputModel, TaskDto>();
            CreateMap<TopicInputModel, TopicDto>();
            CreateMap<UserInsertInputModel, UserDto>();
            CreateMap<UserUpdateInputModel, UserDto>();
            CreateMap<AbsenceReasonInputModel, StudentLessonDto>();
            CreateMap<AttendanceInputModel, StudentLessonDto>();
            CreateMap<FeedbackInputModel, StudentLessonDto>();
        }

        private void CreateMappingFromDto()
        {
            CreateMap<CourseDto, CourseInfoOutputModel>();
            CreateMap<TopicDto, TopicOutputModel>();
            CreateMap<CommentDto, CommentInfoOutputModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString(_dateFormat)));
            CreateMap<CourseTopicDto, CourseTopicOutputModel>();
            CreateMap<UserDto, UserInfoOutputModel>();
            CreateMap<UserDto, UserInfoShortOutputModel>(); 
            CreateMap<CourseDto, CourseInfoShortOutputModel>();
            CreateMap<TaskDto, TaskInfoOutputModel>();
            CreateMap<TaskDto, TaskInfoWithCoursesOutputModel>();
            CreateMap<TaskDto, TaskInfoWithCoursesAndAnswersOutputModel>();
            CreateMap<TaskDto, TaskInfoWithAnswersOutputModel>();
            CreateMap<TagDto, TagInfoOutputModel>();
            CreateMap<StudentAnswerOnTaskForTaskDto, StudentAnswerOnTaskInfoOutputModel>();
            CreateMap<StudentAnswerOnTaskDto, StudentAnswerOnTaskInfoOutputModel>();
            CreateMap<LessonDto, LessonInfoOutputModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString(_dateFormat)));
            CreateMap<LessonDto, LessonInfoWithCourseOutputModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString(_dateFormat)));
            CreateMap<LessonDto, LessonInfoWithCommentsOutputModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString(_dateFormat)));
            CreateMap<LessonDto, LessonInfoWithStudentsAndCommentsOutputModel>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString(_dateFormat)));
            CreateMap<StudentLessonDto, StudentLessonOutputModel>();
        }
    }
}