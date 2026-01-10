namespace TaskList.Api.Domain.Tasks.DTOs.ToDoItemDTOs
{
    public class DashboardResponse
    {
        public int TaskListCount { get; set; }
        public int TotalTasksCount { get; set; }

        //progress count
        public int TasksInProgressCount { get; set; } = 0;
        public int TasksDoneCount { get; set; } = 0;
        public int TasksNotStartedCount { get; set; } = 0;

        //priority count
        public int TasksPriorityLow { get; set; } = 0;
        public int TasksPriorityMedium { get; set; } = 0;
        public int TasksPriorityHigh { get; set; } = 0;

    }
}