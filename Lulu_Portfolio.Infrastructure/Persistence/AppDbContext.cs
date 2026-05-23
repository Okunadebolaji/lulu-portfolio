using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.Domain.Entities;

namespace Lulu_Portfolio.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<Testimonial> Testimonials => Set<Testimonial>();

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<Project>().ToTable("Projects");

        modelBuilder.Entity<Skill>().ToTable("Skills");

        modelBuilder.Entity<Service>().ToTable("Services");

        modelBuilder.Entity<Testimonial>().ToTable("Testimonials");

        modelBuilder.Entity<ContactMessage>().ToTable("ContactMessages");
    }
}