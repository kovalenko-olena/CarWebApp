using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class WayBillViewModel
	{
		public WayBillViewModel()
		{
			DriverSpr = new DriverSpr();
			FuelSpr= new FuelSpr();
			VehicleSpr= new VehicleSpr();
		}
		public WayBillViewModel(WayBill spr) : this(spr, null)
		{
		}
		public WayBillViewModel(WayBill spr, string? encryptedId)
		{
			Id = spr.Id;
			Cd = spr.Cd;
			DtGive = spr.DtGive;
			DtReturn = spr.DtReturn;
			DtOut = spr.DtOut;
			DtIn = spr.DtIn;
			SpdOut = spr.SpdOut;
			SpdIn = spr.SpdIn;
			FuelBalOut = spr.FuelBalOut;
			FuelBalIn = spr.FuelBalIn;
			FuelFillUp = spr.FuelFillUp;
			FuelConsumNorm = spr.FuelConsumNorm;
			FuelConsumFact = spr.FuelConsumFact;
			DriverSpr = spr.DriverSpr == null ? new DriverSpr() : spr.DriverSpr;
			FuelSpr = spr.FuelSpr == null ? new FuelSpr() : spr.FuelSpr;
			VehicleSpr = spr.VehicleSpr == null ? new VehicleSpr() : spr.VehicleSpr;
			DriverSprName = spr.DriverSpr == null ? "" : spr.DriverSpr.FirstName + " " + DriverSpr?.LastName;
			FuelSprName = spr.FuelSpr == null ? "" : spr.FuelSpr.Name;
			VehicleSprName = spr.VehicleSpr == null ? "" : spr.VehicleSpr.RegNumber;
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
		[Required]
		[Display(Name = "ID Number")]
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

		public DriverSpr DriverSpr { get; set; }

		public FuelSpr FuelSpr { get; set; }

		public VehicleSpr VehicleSpr { get; set; }

		[Display(Name = "Driver")]
		public string? DriverSprName { get; set; }

		[Display(Name = "Fuel")]
		public string? FuelSprName { get; set; }

		[Display(Name = "Vehicle")]
		public string? VehicleSprName { get; set; }



		public ApplicationUser? CreatedByUser { get; set; }
		public string? CreatedByUserEmail { get; set; }
		public string? CreatedByUserName { get; set; }
		public string? CreatedByUserPhoneNumber { get; set; }
		public int? CreatedByUserTn { get; set; }

		public DateTime? CreateDate { get; set; }

		public ApplicationUser? EditedByUser { get; set; }
		public string? EditedByUserEmail { get; set; }
		public string? EditedByUserName { get; set; }
		public string? EditedByUserPhoneNumber { get; set; }
		public int? EditedByUserTn { get; set; }
		public DateTime? EditDate { get; set; }


		[NotMapped]
		public string? EncryptedId { get; set; }

	}
}
