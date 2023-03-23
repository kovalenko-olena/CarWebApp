using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class CreateRoleViewModel
	{
		[Required]
		public string RoleName { get; set; }
	}
}
