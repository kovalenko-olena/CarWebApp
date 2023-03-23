using System.Security.Claims;

namespace CarWebApp.Models
{
	public static class ClaimsStore
	{
		public static List<Claim> AllClaims = new List<Claim>()
		{
			// Roles
			new Claim("Create Role", "Create Role"),
			new Claim("Edit Role", "Edit Role"),
			new Claim("Delete Role", "Delete Role"),


			// data access
			// full access - settings(create, edit, delete), waybills(create, edit, delete)
            new Claim("Administrator", "Administrator"),

			// access - settings(index view), waybills(create, edit, delete)
            new Claim("Master", "Master"),

			// access - waybills(create, edit)
            new Claim("Operator", "Operator")
		};
	}
}
