using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDoAPI.Data;
using MyToDoAPI.Models;
using System.Security.Claims;

namespace MyToDoAPI.Controllers
{
    [ApiController]
    [Route("api/reminder")]
    [Authorize] // JWT required
    public class ReminderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReminderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create Reminder (Admin can create for any user, User can create only for self)
        [HttpPost]
        public async Task<IActionResult> CreateReminder(int todoId, DateTime reminderTime)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            // If normal User, only allow reminder for their own Todo
            var todo = await _context.TodoItems.FindAsync(todoId);
            if (todo == null) return NotFound("Todo not found");

            if (role == "User" && todo.UserId != userId)
                return Forbid("You cannot create reminder for other users' Todo");

            var reminder = new TodoReminder
            {
                TodoId = todoId,
                UserId = todo.UserId, // always assign owner
                ReminderTime = reminderTime,
                IsSent = false
            };

            _context.TodoReminders.Add(reminder);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reminder Scheduled", reminderId = reminder.Id });
        }

        // Get Pending Reminders
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingReminders()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            IQueryable<TodoReminder> query = _context.TodoReminders
                .Include(r => r.Todo)
                .Where(r => !r.IsSent && r.ReminderTime <= DateTime.Now);

            // If User, only own reminders
            if (role == "User")
                query = query.Where(r => r.UserId == userId);

            var reminders = await query.ToListAsync();

            return Ok(reminders.Select(r => new
            {
                r.Id,
                r.TodoId,
                TodoTitle = r.Todo.Title,
                r.UserId,
                r.ReminderTime,
                r.IsSent
            }));
        }

        // Mark Reminder as Sent
        [HttpPost("{id}/mark-sent")]
        public async Task<IActionResult> MarkReminderSent(int id)
        {
            var reminder = await _context.TodoReminders.FindAsync(id);
            if (reminder == null) return NotFound("Reminder not found");

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (role == "User" && reminder.UserId != userId)
                return Forbid("You cannot update other users' reminder");

            reminder.IsSent = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reminder marked as sent" });
        }
    }
}
