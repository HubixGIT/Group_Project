using Microsoft.EntityFrameworkCore;
using Web.Api.Entities;

namespace Web.Api.Database;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions options) : base(options) 
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id)
                .UseIdentityColumn();
        });
        
        
        modelBuilder.Entity<UserProject>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id)
                .UseIdentityColumn();
            entity.HasIndex(x => x.UserId);
            entity.HasOne(x => x.User)
                .WithMany(x => x.UserProjects)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ProjectId);
            entity.HasOne(x => x.Project)
                .WithMany(x => x.UserProjects)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
}