using AutoMapper;
using TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Application.MappingProfiles
{
    public class ToDoItemMappingProfiles : Profile
    {
        public ToDoItemMappingProfiles()
        {
            CreateMap<ToDoItem, ToDoItemResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))
                .ForMember(dest => dest.ToDoListTitle, opt => opt.MapFrom(src => src.ToDoList.Title));

            CreateMap<CreateToDoItemRequest, ToDoItem>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : string.Empty))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Trim()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.IsDefined(typeof(ProgressStatus), src.Status) ? (ProgressStatus)src.Status : ProgressStatus.NotStarted))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => Enum.IsDefined(typeof(PriorityStatus), src.Priority) ? (PriorityStatus)src.Priority : PriorityStatus.Low))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoList, opt => opt.Ignore());

            CreateMap<UpdateToDoItemRequest, ToDoItem>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : string.Empty))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Trim()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.IsDefined(typeof(ProgressStatus), src.Status) ? (ProgressStatus)src.Status : ProgressStatus.NotStarted))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => Enum.IsDefined(typeof(PriorityStatus), src.Priority) ? (PriorityStatus)src.Priority : PriorityStatus.Low))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoList, opt => opt.Ignore());
        }
    }
}