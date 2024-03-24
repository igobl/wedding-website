using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Wedding.ef.Entities
{
    public class Invitation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Email")]
        public string SendTo { get; set; }
        [DisplayName("Sent At")]
        public DateTime? SentTimeStamp { get; set; }
        [Required]
        public Guid PublicId { get; set; }

        public virtual ICollection<Attendee> Attendees { get; set; }
        public virtual Entities.Rsvp? Rsvp { get; set; }
    }
}
