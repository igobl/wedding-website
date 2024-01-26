using System.ComponentModel.DataAnnotations;

namespace Wedding.ef.Entities
{
    public class Invitation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string SendTo { get; set; }
        [Required] 
        public DateTime? SentTimeStamp { get; set; }

        public virtual ICollection<Attendee> Attendees { get; set; }
    }
}
