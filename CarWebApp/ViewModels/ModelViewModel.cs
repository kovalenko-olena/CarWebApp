using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class ModelViewModel
	{
		public ModelViewModel()
		{
			VehicleSprs = new List<VehicleSpr>();
			BrandSpr = new BrandSpr();
			FuelSpr = new FuelSpr();
		}
		public ModelViewModel(ModelSpr spr) : this(spr, null)
		{
		}
		public ModelViewModel(ModelSpr spr, string? encryptedId)
		{
			Id = spr.Id;
			Cd = spr.Cd;
			Name = spr.Name;
			VehicleSprs = spr.VehicleSprs;
			BrandSpr = spr.BrandSpr == null ? new BrandSpr() : spr.BrandSpr;
			FuelSpr = spr.FuelSpr == null ? new FuelSpr() : spr.FuelSpr;
			EncryptedId = encryptedId ?? spr.EncryptedId;
			FuelSprName = spr.FuelSpr == null ? "" : spr.FuelSpr.Name;
			BrandSprName = spr.BrandSpr == null ? "" : spr.BrandSpr.Name;
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
		public int Cd { get; set; }

		[Required]
		[Display(Name = "Model name")]
		[MaxLength(50, ErrorMessage = "Model name cannot exceed 50 characters")]
		[MinLength(1, ErrorMessage = "Model name cannot be shorter than 1 character")]
		public string? Name { get; set; }

		[Display(Name = "Brand")]
		public BrandSpr BrandSpr { get; set; }

		[Display(Name = "Fuel")]
		public FuelSpr FuelSpr { get; set; }

		[Display(Name = "Fuel")]
		public string? FuelSprName { get; set; }

		[Display(Name = "Brand")]
		public string? BrandSprName { get; set; }


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

		public List<VehicleSpr> VehicleSprs { get; set; }

		[NotMapped]
		public string? EncryptedId { get; set; }
	}
}
