using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CarWebApp.ViewModels
{
	public class UserClaimsViewModel
	{
		public UserClaimsViewModel()
		{
			Claims=new List<UserClaim>();
		}
		public string UserId { get; set; }
		public List<UserClaim> Claims { get;set; }
	}
}
