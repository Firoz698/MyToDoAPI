namespace MyToDoAPI.DTOs
{
    public class TodoCreateDto
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PriorityId { get; set; }
        public DateTime DueDateTime { get; set; }
    }
}
