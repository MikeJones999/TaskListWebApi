namespace TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs
{
    public class UpdateToDoItemStatusRequest
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}