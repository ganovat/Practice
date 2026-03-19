using Microsoft.EntityFrameworkCore;
using UniPal_API.Models;

namespace UniPal_API.Database
{


    public class UniPalDbContext: DbContext
    {
        public UniPalDbContext(DbContextOptions<UniPalDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    
    }
}
