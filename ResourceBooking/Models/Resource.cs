using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace ResourceBooking.Models
{
    [Table("ResourceInfo")]
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ResourceTypeId { get; set;}

        [ForeignKey("ResourceTypeId")]
        public ResourceType ResourceType { get; set; }

        //Navigation property
        public ICollection<Booking> Bookings { get; set; }

    }
}
