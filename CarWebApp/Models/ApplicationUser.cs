using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CarWebApp.Models
{
	public class ApplicationUser:IdentityUser
	{
		public int Tn { get; set; }
		public string? Name { get; set; }
			
	}

}
