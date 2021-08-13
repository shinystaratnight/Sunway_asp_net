namespace SiteBuilder.Domain.Models.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interface;

    [Table("Content")]
    public class Content : IContent
    {
        [Key]
        [Required]
        [Column("ContentID")]
        public int ID { get; set; }

        [Required]
        public int SiteID { get; set; }

        [Required]
        public int InstanceID { get; set; }

        [Required]
        public int EntityID { get; set; }

        [Required]
        public int LanguageID { get; set; }

        [StringLength(200)]
        public string Context { get; set; }

        [Required]
        [Column("Content")]
        public string Value { get; set; }

        [Required]
        [Column("LastUpdate")]
        public DateTime ActionDate { get; set; }

        [Required]
        public int UserID { get; set; }
    }
}