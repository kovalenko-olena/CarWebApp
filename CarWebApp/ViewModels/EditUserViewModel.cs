using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.ViewModels
{
	public class EditUserViewModel
	{
		public EditUserViewModel()
		{
			Claims=new List<string>();
			Roles=new List<string>();
		}
		[Required]
		public string UserName { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public int TN { get; set; }
		public string Id { get; set; }
		public List<string> Claims { get; set; }
		public List<string> Roles { get; set; }
	}
}
