namespace TaskList.Api.Domain.Tasks.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();

        //navigation Properties
        // FK to Identity user
        public string UserId { get; set; }
    }
}
