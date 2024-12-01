using Microsoft.EntityFrameworkCore;

namespace MyApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Lägg till DbSet för användare
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
    }
}
