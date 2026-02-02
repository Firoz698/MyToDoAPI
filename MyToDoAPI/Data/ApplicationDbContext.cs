using Microsoft.EntityFrameworkCore;
using MyToDoAPI.Models;

namespace MyToDoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }          // ✅ নতুন
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoReminder> TodoReminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Role relation
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()               // Role থেকে অনেক user যেতে পারে
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete
        }
    }
}
