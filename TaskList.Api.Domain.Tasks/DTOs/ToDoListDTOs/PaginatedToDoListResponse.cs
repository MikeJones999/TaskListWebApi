namespace TaskList.Api.Domain.Tasks.DTOs.ToDoListDTOs
{
    public class PaginatedToDoListResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int TotalItemCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public List<ToDoItemSummary> Tasks { get; set; } = new List<ToDoItemSummary>();
    }
}
