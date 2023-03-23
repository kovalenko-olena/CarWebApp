using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.Models
{
    [Table("DriverSpr")]
    public partial class DriverSpr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DriverSpr()
        {
            VehicleSprs = new List<VehicleSpr>();
            WayBills = new List<WayBill>();
            FirstName = "";
            LastName= "";
            CreatedByUser= new ApplicationUser();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Tn { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<VehicleSpr> VehicleSprs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<WayBill> WayBills { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
