namespace MyToDoAPI.DTOs
{
    public class UpdateTodoDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
    }
}
