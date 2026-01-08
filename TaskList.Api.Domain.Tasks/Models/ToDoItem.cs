using TaskList.Api.Domain.Tasks.Enums;

namespace TaskList.Api.Domain.Tasks.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Status Status { get; set; }
        public PriorityStatus Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        
        public ToDoList ToDoList  { get; set; }
        public int ToDoListId { get; set; }
    }
}
