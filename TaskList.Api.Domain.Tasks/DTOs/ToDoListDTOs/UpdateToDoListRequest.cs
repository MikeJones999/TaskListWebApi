namespace TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs
{
    public class UpdateToDoListRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}