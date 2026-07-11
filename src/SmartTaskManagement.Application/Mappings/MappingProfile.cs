using AutoMapper;
using SmartTaskManagement.Application.DTOs.Projects;
using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Domain.Entities;

namespace SmartTaskManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.TaskCount,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted)))
                .ForMember(dest => dest.CompletedTasks,
                    opt => opt.MapFrom(src => src.Tasks.Count(t => !t.IsDeleted && t.Status == Domain.Enums.TaskItemStatus.Completed)))
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.MapFrom(src => src.CreatedByUser));

            CreateMap<User, UserDto>();

            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateProjectDto, Project>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());
        }
    }
}