using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
