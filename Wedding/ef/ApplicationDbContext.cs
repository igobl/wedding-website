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
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Rsvp> Rsvps { get; set; }
    }
}
