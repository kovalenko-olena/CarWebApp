using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace CarWebApp.Models
{
    [Table("ModelSpr")]
    public partial class ModelSpr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModelSpr()
        {
            VehicleSprs = new List<VehicleSpr>();
            CreatedByUser = new ApplicationUser();
            Name = "";
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public int Cd { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Model name cannot exceed 50 characters")]
        [MinLength(1, ErrorMessage = "Model name cannot be shorter than 1 character")]
        public string Name { get; set; }

        public BrandSpr? BrandSpr { get; set; }

        public FuelSpr? FuelSpr { get; set; }

        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<VehicleSpr> VehicleSprs { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }



}
