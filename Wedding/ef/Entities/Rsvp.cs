using System.ComponentModel.DataAnnotations;

namespace Wedding.ef.Entities
{
    public class Rsvp
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public bool IsAttending { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public virtual Invitation Invitation { get; set; }
    }
}
