using Microsoft.EntityFrameworkCore;
using Wedding.ef.Entities;

namespace Wedding.ef
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Rsvp> Rsvps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invitation>()
                .HasIndex(a => a.PublicId)
                .IsUnique();

            modelBuilder.Entity<Invitation>()
                .HasOne(a => a.Rsvp)
                .WithOne(a => a.Invitation)
                .HasForeignKey<Rsvp>(a => a.InvitationId)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
