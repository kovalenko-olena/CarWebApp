using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebApp.Models
{
    [Table("WayBill")]
    public partial class WayBill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int Id { get; set; }

        public int? Cd { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DtGive { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DtReturn { get; set; }

        public DateTime? DtOut { get; set; }

        public DateTime? DtIn { get; set; }

        public int? SpdOut { get; set; }

        public int? SpdIn { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? FuelBalOut { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? FuelBalIn { get; set; }

        public decimal? FuelFillUp { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? FuelConsumNorm { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? FuelConsumFact { get; set; }

        public DriverSpr? DriverSpr { get; set; }

        public FuelSpr? FuelSpr { get; set; }

        public VehicleSpr? VehicleSpr { get; set; }

        [Required]
        public ApplicationUser CreatedByUser { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public ApplicationUser? EditedByUser { get; set; }
        public DateTime? EditDate { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
