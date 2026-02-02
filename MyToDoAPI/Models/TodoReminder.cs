using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyToDoAPI.Models
{
    public class TodoReminder
    {
        public int Id { get; set; }

        // Foreign Key to TodoItem
        public int TodoId { get; set; }
        [ForeignKey("TodoId")]
        public TodoItem Todo { get; set; }  // Navigation property

        // Foreign Key to User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }      // Navigation property

        public DateTime ReminderTime { get; set; }
        public bool IsSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
