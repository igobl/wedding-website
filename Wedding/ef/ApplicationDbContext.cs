using Microsoft.EntityFrameworkCore;
using Wedding.ef.Entities;

namespace Wedding.ef
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<Attendee> Attendees { get; set; }
    }
}
