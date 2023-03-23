using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CarWebApp.Models
{
    [Table("VehicleSpr")]
    public partial class VehicleSpr
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VehicleSpr()
        {
            WayBills = new List<WayBill>();
            CreatedByUser = new ApplicationUser();
            RegNumber = "";
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(20)]
        public string RegNumber { get; set; }
        public int? GarNumber { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? Norm { get; set; }

        public DriverSpr? DriverSpr { get; set; }

        public ModelSpr? ModelSpr { get; set; }

        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<WayBill> WayBills { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
