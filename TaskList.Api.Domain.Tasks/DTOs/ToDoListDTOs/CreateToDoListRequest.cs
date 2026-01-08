namespace TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs
{
    public class CreateToDoListRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}