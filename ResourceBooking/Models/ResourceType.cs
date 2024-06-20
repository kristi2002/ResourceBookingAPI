using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBooking.Models
{
    [Table("ResourceTypeInfo")]
    public class ResourceType
    {
        [Key]
        public int ResourceTypeId { get; set; }

        [Required]
        public string TypeName { get; set; }

        public ICollection<Resource> Resources { get; set; }

    }
}
