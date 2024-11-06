using Microsoft.EntityFrameworkCore;
using WebApp.Domain;

namespace WebApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Table> Tables { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}