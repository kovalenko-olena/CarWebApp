using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarWebApp.Models
{
    [Table("BrandSpr")]
    public partial class BrandSpr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BrandSpr()
        {
            ModelSprs = new List<ModelSpr>();
            CreatedByUser = new ApplicationUser();
        }
        
        //[BindNever]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public int Cd { get; set; }

        
        [Required]
        [MaxLength(50, ErrorMessage = "Brand name cannot exceed 50 characters")]
        [MinLength(1, ErrorMessage = "Brand name cannot be shorter than 1 character")]
        public string? Name { get; set; }

        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ModelSpr> ModelSprs { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }

    }
}
