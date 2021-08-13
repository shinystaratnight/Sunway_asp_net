namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Instance")]
    public class Instance
    {
        [Key]
        [Required]
        [Column("InstanceID")]
        public int ID { get; set; }

        [Required]
        public int SiteID { get; set; }

        [Required]
        [Column("Instance")]
        [StringLength(20)]
        public string Value { get; set; }
    }
}