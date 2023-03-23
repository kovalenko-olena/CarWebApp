using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CarWebApp.Security
{

	public class CanEditOnlyOtherAdminRolesAndClaimsHandler:
		AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		public CanEditOnlyOtherAdminRolesAndClaimsHandler(
				IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
				ManageAdminRolesAndClaimsRequirement requirement)
		{

			var loggedInAdminId = context.User.Claims
				.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value.ToString();

			var adminIdBeingEdited = httpContextAccessor.HttpContext
				.Request.Query["userId"].ToString();

			//userId==IdUserToChange and Role=Admin => cannot change roles
			//userId!=IdUserToChange => can change roles
			if (context.User.IsInRole("Admin")
				 && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true")
				 && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
			{
				context.Succeed(requirement);
			}
			

			return Task.CompletedTask;
		}
	}
}
