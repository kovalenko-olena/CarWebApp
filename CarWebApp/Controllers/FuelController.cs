using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarWebApp.Data;
using CarWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.DataProtection;
using CarWebApp.Security;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage;
using CarWebApp.ViewModels;
using CarWebApp.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CarWebApp.Controllers
{
	[Authorize]
	public class FuelController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ISprRepository<FuelSpr> _repository;
		private readonly ILogger<FuelController> _logger;
		private readonly IDataProtector protector;
		public FuelController(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			ISprRepository<FuelSpr> repository,
			ILogger<FuelController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_context = context;
			_userManager = userManager;
			_repository = repository;
			_logger = logger;
			protector = dataProtectionProvider
				.CreateProtector(dataProtectionPurposeStrings.FuelIdRouteValue);
		}

		// GET: Fuel
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Index()
		{
			List<FuelViewModel> model = new List<FuelViewModel>();
			var spr = await _repository.GetAllItems();

			foreach (FuelSpr item in spr)
			{
				model.Add(new FuelViewModel(item, protector.Protect(item.Id.ToString())));
			};

			int count = model.ToList().Count();
			TempData["count"] = count;
			return View(model);
		}

		// GET: Fuel/Details/5
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Details(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));

			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}

			var spr = await _repository.GetItem(decryptedId);
			/*var sprCreatedUser = _repository.GetCreatedUser(decryptedId);
            var sprEditedUser = _repository.GetEditedUser(decryptedId);
            spr.CreatedByUser = sprCreatedUser;
            spr.EditedByUser = sprEditedUser;*/
			if (spr == null)
			{
				Response.StatusCode = 404;
				return View("FuelNotFound", decryptedId);
			}
			FuelViewModel model = new FuelViewModel(spr, id);
			return View(model);
		}

		// GET: Fuel/Create
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create()
		{
			FuelViewModel model = new FuelViewModel();
			model.Cd = await _repository.GetLastCd() + 1;

			return View(model);
		}

		// POST: Fuel/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create([Bind("Cd,Name")] FuelViewModel model)
		{
			if (ModelState.IsValid)
			{
				FuelSpr spr = new FuelSpr
				{
					Id = await _repository.GetLastId() + 1,
					Cd = model.Cd,
					Name = model?.Name ?? string.Empty,
					CreatedByUser = await _userManager.GetUserAsync(User),
					CreateDate = DateTime.Now,
				};

				await _repository.Add(spr);
				TempData["success"] = "Fuel created successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		// GET: Fuel/Edit/5
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			var model = new FuelViewModel(await _repository.GetItem(decryptedId), id);

			return View(model);
		}

		// POST: 
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id, [Bind("Cd,Name")] FuelViewModel sprNew)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			var sprCurrent = await _repository.GetItem(decryptedId);
			if (sprCurrent == null || sprNew == null)
			{
				return NotFound();
			}
			/*if (ModelState["Cd"].ValidationState == ModelValidationState.Valid &&
                ModelState["Name"].ValidationState == ModelValidationState.Valid)*/
			if (ModelState.IsValid)
			{
				try
				{
					FuelSpr spr = new FuelSpr
					{
						Id = decryptedId,
						Cd = sprNew.Cd,
						Name = sprNew?.Name ?? string.Empty,
						EditedByUser = await _userManager.GetUserAsync(User),
						EditDate = DateTime.Now,
					};
					await _repository.Update(spr);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!await _repository.FindItem(sprCurrent.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				TempData["success"] = "Fuel edited successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(sprNew);
		}

		// GET: Fuel/Delete/5
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Delete(string? id)
		{
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			else
			{
				int decryptedId = Convert.ToInt32(protector.Unprotect(id));
				var model = new FuelViewModel(await _repository.GetItem(decryptedId));
				if (model == null)
				{
					return NotFound();
				}

				return View(model);
			}
		}

		// POST: Fuel/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			// _logger.LogTrace("Trace Log");
			// _logger.LogDebug("Debug Log");
			// _logger.LogWarning("Warning Log");
			// _logger.LogError("Error Log");
			// _logger.LogCritical("Critical Log");
			ApplicationUser? applicationUser = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == _userManager.GetUserId(User));
			_logger.LogInformation("Deleted by: UserTn  = " + applicationUser?.Tn.ToString());
			_logger.LogInformation("Deleted by: UserId  = " + applicationUser?.Id.ToString());
			_logger.LogInformation("Deleted by: UserEmail  = " + applicationUser?.Email.ToString());


			if (await _repository.GetAllItems() == null)
			{
				return Problem("Entity set 'FuelSpr'  is null.");
			}
			var spr = await _repository.GetItem(id);
			if (spr != null)
			{
				await _repository.Delete(id);
			}

			TempData["success"] = "Fuel deleted successfully";
			return RedirectToAction(nameof(Index));
		}


	}
}
