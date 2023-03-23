using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.Models
{
    [Table("FuelSpr")]
    public partial class FuelSpr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FuelSpr()
        {
            ModelSprs = new List<ModelSpr>();
            WayBills = new List<WayBill>();
            Name = "";
            CreatedByUser = new ApplicationUser();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public int Cd { get; set; }

        [Required]
		[MaxLength(50, ErrorMessage = "Fuel name cannot exceed 50 characters")]
		[MinLength(1, ErrorMessage = "Fuel name cannot be shorter than 1 character")]
		public string Name { get; set; }
        
        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ModelSpr> ModelSprs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<WayBill> WayBills { get; set; }


        [NotMapped]
		public string? EncryptedId { get; set; }
	}
    public partial class FuelSpr
    {
        public override string ToString()
        {
            return $"{ this.Name}";
        }
    }
}
