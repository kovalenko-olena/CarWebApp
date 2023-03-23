using CarWebApp.Models;
using CarWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Security.Policy;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarWebApp.Controllers
{
	[Authorize(Policy = "AdminRolePolicy")]
	//[Authorize(Roles="Admin")]
	public class AdministrationController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<AdministrationController> logger;

		public AdministrationController(RoleManager<IdentityRole> roleManager,
			UserManager<ApplicationUser> userManager,
			ILogger<AdministrationController> logger)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.logger = logger;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> ManageUserClaims(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
				return View("NotFound");
			}
			var existingUserClaims = await userManager.GetClaimsAsync(user);
			var model = new UserClaimsViewModel
			{
				UserId = userId
			};
			foreach (Claim claim in ClaimsStore.AllClaims)
			{
				UserClaim userClaim = new UserClaim
				{
					ClaimType = claim.Type
				};
				if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
				{
					userClaim.IsSelected = true;
				}
				model.Claims.Add(userClaim);
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
		{
			var user = await userManager.FindByIdAsync(model.UserId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
				return View("NotFound");
			}
			var claims = await userManager.GetClaimsAsync(user);
			var result = await userManager.RemoveClaimsAsync(user, claims);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove user existing claims");
				return View(model);
			}
			result = await userManager.AddClaimsAsync(user,
				model.Claims.Select(y => new Claim(y.ClaimType, y.IsSelected ? "true" : "false")));
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected claims to user");
				return View(model);
			}
			return RedirectToAction("EditUser", new { id = model.UserId });
		}


		[HttpGet]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRoles(string userId)
		{
			ViewBag.userId = userId;
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={userId} cannot be found";
				return View("NotFound");
			}
			var model = new List<UserRolesViewModel>();
			foreach (var role in roleManager.Roles)
			{
				var userRolesViewModel = new UserRolesViewModel
				{
					RoleName = role.Name,
					RoleId = role.Id
				};
				if (await userManager.IsInRoleAsync(user, role.Name))
				{
					userRolesViewModel.IsSelected = true;
				}
				else
				{
					userRolesViewModel.IsSelected = false;
				}
				model.Add(userRolesViewModel);
			}
			return View(model);
		}

		[HttpPost]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={userId} cannot be found";
				return View("NotFound");
			}
			var roles = await userManager.GetRolesAsync(user);
			var result = await userManager.RemoveFromRolesAsync(user, roles);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove user existing roles");
				return View(model);
			}
			result = await userManager.AddToRolesAsync(user,
				model.Where(x => x.IsSelected).Select(y => y.RoleName));
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected roles to user");
				return View(model);
			}
			return RedirectToAction("EditUser", new { id = user.Id });
		}



		[HttpPost]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id={id} cannot be found";
				return View("NotFound");
			}
			else
			{
				var result = await userManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View("ListUsers");
			}
		}

		[HttpGet]
		public IActionResult ListUsers()
		{
			var users = userManager.Users;
			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with id={id} cannot be found";
				return View("NotFound");
			}

			var userClaims = await userManager.GetClaimsAsync(user);
			var userRoles = await userManager.GetRolesAsync(user);

			var model = new EditUserViewModel
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				TN = user.Tn,
				Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
				Roles = userRoles.ToList()
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(EditUserViewModel model)
		{
			var user = await userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with id={model.Id} cannot be found";
				return View("NotFound");
			}
			else
			{
				user.Email = model.Email;
				user.UserName = model.UserName;
				user.Tn = model.TN;
				var result = await userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}
		}

		[HttpGet]
		[Authorize(Policy = "CreateRolePolicy")]
		public IActionResult CreateRole()
		{
			return View();
		}
		[HttpPost]
		[Authorize(Policy = "CreateRolePolicy")]
		public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				IdentityRole identityRole = new IdentityRole()
				{
					Name = model.RoleName
				};
				IdentityResult result = await roleManager.CreateAsync(identityRole);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles", "Administration");
				}
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}
			return View(model);
		}

		[HttpGet]
		public IActionResult ListRoles()
		{
			var roles = roleManager.Roles.ToList();
			return View(roles);
		}

		[HttpGet]
		//[Authorize(Policy ="EditRolePolicy")]
		public async Task<IActionResult> EditRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
				return View("NotFound");
			}
			else
			{
				var model = new EditRoleViewModel
				{
					Id = role.Id,
					RoleName = role.Name,
				};
				foreach (var user in userManager.Users)
				{
					if (await userManager.IsInRoleAsync(user, role.Name))
					{
						model.Users.Add(user.UserName);
					}
				}
				return View(model);
			}

		}

		[HttpPost]
		//[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> EditRole(EditRoleViewModel model)
		{
			var role = await roleManager.FindByIdAsync(model.Id);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found";
				return View("NotFound");
			}
			else
			{
				role.Name = model.RoleName;
				var result = await roleManager.UpdateAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);

			}
		}

		[HttpPost]
		[Authorize(Policy = "DeleteRolePolicy")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
				return View("NotFound");
			}
			else
			{
				try
				{
					var result = await roleManager.DeleteAsync(role);
					if (result.Succeeded)
					{
						return RedirectToAction("ListRoles");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View("ListRoles");
				}
				catch (DbUpdateException ex)
				{
					logger.LogError($"Error deleting role{ex}");
					ViewBag.ErrorTitle = $"{role.Name} role is in use";
					ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users " +
						$"in this role. If you want to delete this role, please remove the users from " +
						$"the role and then try to delete";
					return View("ErrorHandler");
				}

			}
		}

		[HttpGet]
		public async Task<IActionResult> EditUsersInRole(string roleId)
		{
			ViewBag.roleId = roleId;
			var role = await roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
				return View("NotFound");
			}

			var model = new List<UserRoleViewModel>();
			foreach (var user in userManager.Users)
			{
				var userRoleViewModel = new UserRoleViewModel
				{
					UserId = user.Id,
					UserName = user.UserName
				};
				if (await userManager.IsInRoleAsync(user, role.Name))
				{
					userRoleViewModel.IsSelected = true;
				}
				else
				{
					userRoleViewModel.IsSelected = false;
				}
				model.Add(userRoleViewModel);
			}
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
		{
			var role = await roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with Id={roleId} cannot be found";
				return View("NotFound");
			}
			for (int i = 0; i < model.Count; i++)
			{
				var user = await userManager.FindByIdAsync(model[i].UserId);
				IdentityResult? result = null;
				if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
				{
					result = await userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
				{
					result = await userManager.RemoveFromRoleAsync(user, role.Name);
				}
				else
				{
					continue;
				}

				if (result.Succeeded)
				{
					if (i < (model.Count - 1))
						continue;
					else
						return RedirectToAction("EditRole", new { Id = roleId });
				}
			}
			return RedirectToAction("EditRole", new { Id = roleId });
		}
	}
}
