namespace TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs
{
    public class ToDoListResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int ToDoItemCount { get; set; }
        public List<ToDoItemSummary> Tasks { get; set; } = new List<ToDoItemSummary>();
    }
}