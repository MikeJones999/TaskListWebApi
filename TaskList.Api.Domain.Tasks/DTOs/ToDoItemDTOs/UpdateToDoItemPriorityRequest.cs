namespace TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs
{
    public class UpdateToDoItemPriorityRequest
    {
        public int Id { get; set; }
        public int Priority { get; set; }
    }
}