namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Site")]
    public class Site
    {
        [Key]
        [Required]
        [Column("SiteID")]
        public int ID { get; set; }

        [Required]
        [Column("Site")]
        [StringLength(40)]
        public string Name { get; set; }
    }
}