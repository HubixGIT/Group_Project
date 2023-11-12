using Microsoft.EntityFrameworkCore;
using Web.Api.Entities;

namespace Web.Api.Database;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions options) : base(options) 
    {
        
    }

    public DbSet<User> Users { get; set; }
}