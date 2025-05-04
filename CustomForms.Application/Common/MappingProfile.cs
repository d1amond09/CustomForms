using AutoMapper;
using CustomForms.Application.Common.DTOs;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Users;

namespace CustomForms.Application.Common;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<User, UserDto>();
		CreateMap<UserForRegistrationDto, User>();

		CreateMap<User, UserSummaryDto>();
		CreateMap<User, UserDetailsDto>()
			.ForMember(dest => dest.Roles, opt => opt.Ignore());

		CreateMap<Topic, TopicDto>();
		CreateMap<Tag, TagDto>();
		CreateMap<UpdateQuestionDto, Question>();


		CreateMap<Question, QuestionDto>();

		CreateMap<Template, TemplateBriefDto>()
			.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : null))
			.ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.Name : null))
			.ForMember(dest => dest.FormCount, opt => opt.MapFrom(src => src.Forms != null ? src.Forms.Count : 0))
			.ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
			.ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments != null ? src.Comments.Count : 0));

		CreateMap<Template, TemplateDetailsDto>()
			.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : null))
			.ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.Name : null))
			.ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
			.ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.OrderBy(q => q.Order)))
			.ForMember(dest => dest.CanCurrentUserManage, opt => opt.Ignore())
			.ForMember(dest => dest.CanCurrentUserFill, opt => opt.Ignore())
			.ForMember(dest => dest.LikedByCurrentUser, opt => opt.Ignore())
			.ForMember(dest => dest.AllowedUsers, opt => opt.Ignore())
			.ForMember(dest => dest.Comments, opt => opt.Ignore());

		CreateMap<Answer, AnswerDto>()
			.ForMember(dest => dest.QuestionTitle, opt => opt.MapFrom(src => src.Question != null ? src.Question.Title : null))
			.ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Question != null ? src.Question.Type : default));

		CreateMap<Form, FormBriefDto>()
			.ForMember(dest => dest.TemplateTitle, opt => opt.MapFrom(src => src.Template != null ? src.Template.Title : null))
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null));

		CreateMap<Form, FormDetailsDto>()
			.ForMember(dest => dest.TemplateTitle, opt => opt.MapFrom(src => src.Template != null ? src.Template.Title : null))
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
			.ForMember(dest => dest.CanCurrentUserManage, opt => opt.Ignore());

		CreateMap<Comment, CommentDto>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null))
			.ForMember(dest => dest.CanCurrentUserDelete, opt => opt.Ignore());
	}
}

