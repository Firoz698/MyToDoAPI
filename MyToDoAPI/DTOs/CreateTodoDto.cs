namespace MyToDoAPI.DTOs
{
    public class CreateTodoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
    }
}
