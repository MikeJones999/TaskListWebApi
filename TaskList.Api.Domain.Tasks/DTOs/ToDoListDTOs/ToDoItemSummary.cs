namespace TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs
{
    public class ToDoItemSummary
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
