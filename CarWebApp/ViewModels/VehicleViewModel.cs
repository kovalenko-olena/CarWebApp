using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
    public class VehicleViewModel
    {
        public VehicleViewModel()
        {
            WayBills = new List<WayBill>();
            DriverSpr = new DriverSpr();
            ModelSpr= new ModelSpr();   
        }
        public VehicleViewModel(VehicleSpr spr) : this(spr, null)
        {
        }
        public VehicleViewModel(VehicleSpr spr, string? encryptedId)
        {
            Id = spr.Id;
            RegNumber = spr.RegNumber;
            GarNumber = spr.GarNumber;
            WayBills = spr.WayBills;
            Norm = spr.Norm;
            DriverSpr = spr.DriverSpr == null ? new DriverSpr() : spr.DriverSpr;
            ModelSpr = spr.ModelSpr == null ? new ModelSpr() : spr.ModelSpr;
			ModelSprName = spr.ModelSpr == null ? "" : spr.ModelSpr.Name;
            DriverSprName = spr.DriverSpr == null ? "":spr.DriverSpr.FirstName+" "+spr.DriverSpr.LastName;
			EncryptedId = encryptedId ?? spr.EncryptedId;
            CreatedByUser = spr.CreatedByUser;
            CreatedByUserEmail = spr.CreatedByUser == null ? "" : spr.CreatedByUser.Email;
            CreatedByUserName = spr.CreatedByUser == null ? "" : spr.CreatedByUser.Name;
            CreatedByUserPhoneNumber = spr.CreatedByUser == null ? "" : spr.CreatedByUser.PhoneNumber;
            CreatedByUserTn = spr.CreatedByUser == null ? 0 : spr.CreatedByUser.Tn;
            CreateDate = spr.CreateDate;
            EditedByUser = spr.EditedByUser;
            EditedByUserEmail = spr.EditedByUser == null ? "" : spr.EditedByUser.Email;
            EditedByUserName = spr.EditedByUser == null ? "" : spr.EditedByUser.Name;
            EditedByUserPhoneNumber = spr.EditedByUser == null ? "" : spr.EditedByUser.PhoneNumber;
            EditedByUserTn = spr.EditedByUser == null ? 0 : spr.EditedByUser.Tn;
            EditDate = spr.EditDate;
        }

        public int Id { get; set; }
        [StringLength(20)]
        [Display(Name = "Registration Number")]
        [Required]
        public string? RegNumber { get; set; }
        [Display(Name = "Garage Number")]
        public int? GarNumber { get; set; }
        [Column(TypeName = "numeric")]
        [Display(Name = "Fuel Consumption Rate")]

        public decimal? Norm { get; set; }

        [Display(Name = "Driver")]
        public DriverSpr DriverSpr { get; set; }

        [Display(Name = "Model")]
        public ModelSpr ModelSpr { get; set; }

        [Display(Name = "Model")]
        public string? ModelSprName { get; set; }
        [Display(Name ="Driver")]
        public string? DriverSprName { get;set; }

		public ApplicationUser? CreatedByUser { get; set; }
		public string? CreatedByUserEmail { get; set; }
		[Display(Name = "Created by")]
		public string? CreatedByUserName { get; set; }
		public string? CreatedByUserPhoneNumber { get; set; }
		public int? CreatedByUserTn { get; set; }

		[Display(Name = "Сreate")]
		public DateTime? CreateDate { get; set; }

		public ApplicationUser? EditedByUser { get; set; }

		public string? EditedByUserEmail { get; set; }

		[Display(Name = "Edited by")]
		public string? EditedByUserName { get; set; }
		public string? EditedByUserPhoneNumber { get; set; }
		public int? EditedByUserTn { get; set; }

		[Display(Name = "Edit")]
		public DateTime? EditDate { get; set; }

		public List<WayBill>? WayBills { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
