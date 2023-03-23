using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "Password and Confirm Password must mutch")]
		public string ConfirmPassword { get; set;}

		public string Token { get; set; }
	}
}
