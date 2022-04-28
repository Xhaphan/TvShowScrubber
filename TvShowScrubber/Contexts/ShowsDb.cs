using Microsoft.EntityFrameworkCore;
using TvShowScrubber.Models;

namespace TvShowScrubber.Contexts
{
    public class ShowsDb : DbContext
    {
        public ShowsDb(DbContextOptions<ShowsDb> options) : base(options) { }

        public DbSet<Show> Shows { get; set; }

        public DbSet<Cast> Casts { get; set; }
    }
}
