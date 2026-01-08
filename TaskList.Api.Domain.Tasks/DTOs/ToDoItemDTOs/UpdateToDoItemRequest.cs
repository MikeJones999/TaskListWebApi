namespace TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs
{
    public class UpdateToDoItemRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public int ToDoListId { get; set; }
    }
}