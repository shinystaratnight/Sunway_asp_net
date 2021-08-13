namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ContextBridge")]
    public class ContextBridge
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public string OldContext { get; set; }

        [Required]
        public string CurrentContext { get; set; }

        [Required]
        public int SiteID { get; set; }

        [Required]
        public int InstanceID { get; set; }

        [Required]
        public int EntityID { get; set; }

    }

}