namespace SiteBuilder.Domain.Models.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Language")]
    public class Language
    {
        [Key]
        [Required]
        [Column("LanguageID")]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string LanguageCode { get; set; }

        [Required]
        public int SiteID { get; set; }
    }
}