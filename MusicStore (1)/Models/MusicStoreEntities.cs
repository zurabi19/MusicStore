using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MusicStore__1_.Models
{
    public class MusicStoreEntities :IdentityDbContext<ApplicationUser>
    {
        public MusicStoreEntities(DbContextOptions<MusicStoreEntities> options)
           : base(options)
        {
        }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}