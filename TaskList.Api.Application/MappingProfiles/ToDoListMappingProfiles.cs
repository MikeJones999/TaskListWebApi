using AutoMapper;
using TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs;
using TaskList.Api.Domain.Tasks.Models;

namespace TaskList.Api.Application.MappingProfiles
{
    public class ToDoListMappingProfiles : Profile
    {
        public ToDoListMappingProfiles()
        {
            CreateMap<ToDoList, ToDoListResponse>()
                .ForMember(dest => dest.ToDoItemCount, opt => opt.MapFrom(src => src.ToDoItems.Count))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.ToDoItems));

            CreateMap<ToDoItem, ToDoItemSummary>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CompletedOnDate, opt => opt.MapFrom(src => src.CompletedAt))
                ;


            CreateMap<CreateToDoListRequest, ToDoList>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : string.Empty))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoItems, opt => opt.Ignore());

            CreateMap<UpdateToDoListRequest, ToDoList>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description != null ? src.Description.Trim() : string.Empty))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoItems, opt => opt.Ignore());
        }
    }
}