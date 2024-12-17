using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Configuration> Configurations { get; init; } = default!;

    public DbSet<SaveGame> GameStates { get; init; } = default!;
}