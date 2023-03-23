using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
    public class BrandViewModel
    {
        public BrandViewModel()
        {
            ModelSprs = new List<ModelSpr>();
        }
        public BrandViewModel(BrandSpr spr) : this(spr, null)
        {
        }
        public BrandViewModel(BrandSpr spr, string? encryptedId)
        {
            Id = spr.Id;
            Cd = spr.Cd;
            Name = spr.Name;
            ModelSprs = spr.ModelSprs;
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
        public int Cd { get; set; }

        [Required]
        [Display(Name = "Brand name")]
        [MaxLength(50, ErrorMessage = "Brand name cannot exceed 50 characters")]
        [MinLength(1, ErrorMessage = "Brand name cannot be shorter than 1 character")]
        public string? Name { get; set; }

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
        public List<ModelSpr>? ModelSprs { get; set; }


        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
