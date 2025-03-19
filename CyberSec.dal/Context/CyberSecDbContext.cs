using CyberSec.dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace CyberSec.dal.Context;

public class CyberSecDbContext :DbContext
{
    public CyberSecDbContext(DbContextOptions<CyberSecDbContext> options) : base(options)
    {
        
    }

    public DbSet<Message> Messages { get; set; } 
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<User> Users { get; set; }
    
}