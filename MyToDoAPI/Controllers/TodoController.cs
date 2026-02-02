using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDoAPI.Data;
using MyToDoAPI.DTOs;
using MyToDoAPI.Models;
using System.Security.Claims;

namespace MyToDoAPI.Controllers
{
    [ApiController]
    [Route("api/todo")]
    [Authorize] // JWT required
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================
        // CREATE TODO
        // ======================
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var todo = new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId,
                StatusId = dto.StatusId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo created successfully", todoId = todo.Id });
        }

        // ======================
        // GET ALL TODOS
        // ======================
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            IQueryable<TodoItem> query = _context.TodoItems
                .Include(t => t.Status)
                .Include(t => t.User);

            if (role == "User")
                query = query.Where(t => t.UserId == userId);

            var todos = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            return Ok(todos.Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.UserId,
                UserName = t.User.FullName,
                Status = t.Status.StatusName,
                t.CreatedAt
            }));
        }

        // ======================
        // GET TODO BY ID
        // ======================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var todo = await _context.TodoItems
                .Include(t => t.Status)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null) return NotFound("Todo not found");
            if (role == "User" && todo.UserId != userId) return Forbid("Access denied");

            return Ok(new
            {
                todo.Id,
                todo.Title,
                todo.Description,
                todo.UserId,
                UserName = todo.User.FullName,
                Status = todo.Status.StatusName,
                todo.CreatedAt
            });
        }

        // ======================
        // UPDATE TODO
        // ======================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound("Todo not found");

            if (role == "User" && todo.UserId != userId)
                return Forbid("You cannot update others' todos");

            todo.Title = dto.Title ?? todo.Title;
            todo.Description = dto.Description ?? todo.Description;
            todo.StatusId = dto.StatusId ?? todo.StatusId;
            todo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo updated successfully" });
        }

        // ======================
        // DELETE TODO
        // ======================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null) return NotFound("Todo not found");

            if (role == "User" && todo.UserId != userId)
                return Forbid("You cannot delete others' todos");

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Todo deleted successfully" });
        }
    }
}
