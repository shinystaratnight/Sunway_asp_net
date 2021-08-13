namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Entity")]
    public class Entity
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public int SiteID { get; set; }

        [Required]
        [Column("Entity")]
        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        public string JSONSchema { get; set; }

        [Required]
        [Column("EntityType")]
        public string Type { get; set; }
    }
}