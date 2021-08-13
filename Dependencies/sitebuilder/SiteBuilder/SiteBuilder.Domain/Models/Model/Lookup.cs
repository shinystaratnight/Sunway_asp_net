namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Lookup")]
    public class Lookup
    {
        [Key]
        [Required]
        [Column("LookupID")]
        public int ID { get; set; }

        [Required]
        public int InstanceID { get; set; }

        [Required]
        [Column("Lookup")]
        [StringLength(30)]
        public string Value { get; set; }

        [Required]
        [StringLength(200)]
        public string Query { get; set; }

        [Required]
        [StringLength(200)]
        public string ConnectString { get; set; }
    }
}