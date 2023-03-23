using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarWebApp.Security
{
	public class SuperAdminHandler: AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
				ManageAdminRolesAndClaimsRequirement requirement)
		{
			if (context.User.IsInRole("Super Admin"))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
