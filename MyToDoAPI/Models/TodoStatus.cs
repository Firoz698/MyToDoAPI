namespace MyToDoAPI.Models
{
    public class TodoStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }

        // Navigation property
        public ICollection<TodoItem> Todos { get; set; }
    }
}
