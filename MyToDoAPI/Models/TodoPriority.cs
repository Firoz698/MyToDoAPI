namespace MyToDoAPI.Models
{
    public class TodoPriority
    {
        public int Id { get; set; }
        public string PriorityName { get; set; }

        // Navigation property
        public ICollection<TodoItem> Todos { get; set; }
    }
}
