using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using CarWebApp.ViewModels;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CarWebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<Models.ApplicationUser> userManager;
		private readonly SignInManager<Models.ApplicationUser> signInManager;
		private readonly ILogger<AccountController> logger;

		public AccountController(RoleManager<IdentityRole> roleManager,
			UserManager<Models.ApplicationUser> userManager,
			SignInManager<Models.ApplicationUser> signInManager,
			ILogger<AccountController> logger
			)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await this.signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			return View();
		}

		[AcceptVerbs("Get", "Post")]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailInUse(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return Json(true);
			}
			else
			{
				return Json($"Email {email} is already in use");
			}
		}


		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new Models.ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					Tn = model.TN,
					Name = model.Name
				};

				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

					var confirmationLink = Url.Action("ConfirmEmail", "Account",
						new { userId = user.Id, token = token }, Request.Scheme);

					logger.Log(LogLevel.Information, confirmationLink);

					if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
					{
						return RedirectToAction("ListUsers", "Administration");
					}
					/*await signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("index", "home");*/
					ViewBag.ErrorTitle = "Registration successful";
					ViewBag.ErrorMessage = "Before you can login, please confirm your " +
						"email, by clicking on the confirmation link we have emailed you " +
						confirmationLink?.ToString();
					return View("ErrorHandler");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			if (userId == null || token == null)
			{
				return RedirectToAction("Index", "Home");
			}
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.ErrorMessage = $"The User ID {userId} is invalid.";
				return View("NotFound");
			}
			var result = await userManager.ConfirmEmailAsync(user, token);

			if (result.Succeeded)
			{
				return View();
			}
			ViewBag.ErrorTitle = "Email cannot be confirmed";
			return View("Error");
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl)
		{
			LoginViewModel model = new LoginViewModel
			{
				ReturnUrl = returnUrl,
				ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
			};

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
		{
			model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);
				if (user != null && !user.EmailConfirmed &&
					(await userManager.CheckPasswordAsync(user, model.Password)))
				{
					ModelState.AddModelError(string.Empty, "Email not confirmed yet");
					return View(model);
				}

				var result = await signInManager.PasswordSignInAsync(model.Email,
					model.Password, model.RememberMe, true);
				if (result.Succeeded)
				{
					// if first user, then Role = Admin and Claims = all claims for Administrator
					var existingUserClaims = await userManager.GetClaimsAsync(user);
					var existinguserRoles = await userManager.GetRolesAsync(user);

					if (existingUserClaims.Count == 0 &&
						signInManager.UserManager.Users.Count() == 1)
					{
						for (int i = 0; i < 4; i++)
						{
							var identityClaim = ClaimsStore.AllClaims[i];
							await userManager.AddClaimAsync(user, identityClaim);
							await userManager.UpdateAsync(user);
						}
					}
					if (existinguserRoles.Count() == 0 &&
						signInManager.UserManager.Users.Count() == 1)
					{
						CreateRoleViewModel createRoleViewModel = new CreateRoleViewModel();
						createRoleViewModel.RoleName = "Admin";
						IdentityRole identityRole = new IdentityRole()
						{
							Name = createRoleViewModel.RoleName
						};
						IdentityResult resultRole = await roleManager.CreateAsync(identityRole);
						await userManager.AddToRoleAsync(user, "Admin");
					}

					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return LocalRedirect(returnUrl);
					}
					else
					{
						return RedirectToAction("index", "home");
					}
				}
				else if (result.IsLockedOut)
				{
					return View("AccountLocked");
				}
				else
				{
					// in the event of a login failure we make sure to take care of any null values and refresh the external login
					LoginViewModel failmodel = new LoginViewModel
					{
						Email = model.Email,
						Password = model.Password,
						RememberMe = model.RememberMe,
						ReturnUrl = returnUrl,
						ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
					};
					ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
					return View(failmodel);
				}
			}
			return View(model);
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult ExternalLogin(string provider, string? returnUrl)
		{
			var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
				new { ReturnUrl = returnUrl });
			var properties = signInManager.ConfigureExternalAuthenticationProperties(
				provider, redirectUrl);
			return new ChallengeResult(provider, properties);
		}

		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			LoginViewModel loginViewModel = new LoginViewModel
			{
				ReturnUrl = returnUrl,
				ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
			};
			if (remoteError != null)
			{
				ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
				return View("Login", loginViewModel);
			}
			var info = await signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				ModelState.AddModelError(string.Empty, "Error loading external login information");
				return View("Login", loginViewModel);
			}
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			ApplicationUser? user = null;

			if (email != null)
			{
				user = await userManager.FindByEmailAsync(email);
				if (user != null && !user.EmailConfirmed)
				{
					ModelState.AddModelError(string.Empty, "Email not confirmed yet");
					return View("Login", loginViewModel);
				}
			}

			var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
				info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

			if (signInResult.Succeeded)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				if (email != null)
				{
					if (user == null)
					{
						user = new Models.ApplicationUser
						{
							Email = info.Principal.FindFirstValue(ClaimTypes.Email),
							UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
							Name = info.Principal.FindFirstValue(ClaimTypes.Email)
						};
						await userManager.CreateAsync(user);

						var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
						var confirmationLink = Url.Action("ConfirmEmail", "Account",
							new { userId = user.Id, token = token }, Request.Scheme);
						logger.Log(LogLevel.Information, confirmationLink);
						ViewBag.ErrorTitle = "Registration successful";
						ViewBag.ErrorMessage = "Before you can login, please confirm your " +
						"email, by clicking on the confirmation link we have emailed you " +
						confirmationLink?.ToString();
						return View("ErrorHandler");
					}
					await userManager.AddLoginAsync(user, info);
					await signInManager.SignInAsync(user, isPersistent: false);
					return LocalRedirect(returnUrl);
				}
				ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
				ViewBag.ErrorMessage = "Please contact support team";
				return View("ErrorHandler");
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}


		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);

				if (user != null && await userManager.IsEmailConfirmedAsync(user))
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var passwordResetLink = Url.Action("ResetPassword", "Account",
						new { email = model.Email, token = token }, Request.Scheme);
					logger.Log(LogLevel.Information, passwordResetLink);
					ViewBag.ErrorTitle = "ForgotPasswordConfirmation";
					ViewBag.ErrorMessage = "If you have an account with us, we have sent an email " +
											"with the instructions to reset your password " +
											passwordResetLink?.ToString();
					return View("ErrorHandler");

					//return View("ForgotPasswordConfirmation");
				}
				return View("ForgotPasswordConfirmation");
			}
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string email, string token)
		{
			if (token == null || email == null)
			{
				ModelState.AddModelError("", "Invalid password reset token");
			}
			return View();
		}


		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					var result = await userManager.ResetPasswordAsync(user,
						model.Token, model.Password);
					if (result.Succeeded)
					{
						if (await userManager.IsLockedOutAsync(user))
						{
							await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
						}
						return View("ResetPasswordConfirmation");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View(model);
				}
				return View("ResetPasswordConfirmation");
			}
			return View(model);
		}


		[HttpGet]
		public async Task<IActionResult> AddPassword()
		{
			var user = await userManager.GetUserAsync(User);

			var userHasPassword = await userManager.HasPasswordAsync(user);

			if (userHasPassword) return RedirectToAction("ChangePassword");
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.GetUserAsync(User);
				var result = await userManager.AddPasswordAsync(user, model.NewPassword);

				if (!result.Succeeded)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
					return View();
				}
				await signInManager.RefreshSignInAsync(user);
				return View("AddPasswordConfirmation");
			}
			return View(model);
		}


		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await userManager.GetUserAsync(User);

			var userHasPassword = await userManager.HasPasswordAsync(user);

			if (!userHasPassword) return RedirectToAction("AddPassword");
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.GetUserAsync(User);

				if (user == null)
				{
					return RedirectToAction("Login");
				}

				var result = await userManager.ChangePasswordAsync(user,
					model.CurrentPassword, model.NewPassword);
				if (!result.Succeeded)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
					return View();
				}
				await signInManager.RefreshSignInAsync(user);
				return View("ChangePasswordConfirmation");
			}
			return View(model);
		}
	}
}
