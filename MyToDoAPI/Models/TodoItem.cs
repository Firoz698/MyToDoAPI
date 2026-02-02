using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyToDoAPI.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        // Foreign Key to Status
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public TodoStatus Status { get; set; }

        // Foreign Key to Priority
        public int PriorityId { get; set; }
        [ForeignKey("PriorityId")]
        public TodoPriority Priority { get; set; }

        public DateTime DueDateTime { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
