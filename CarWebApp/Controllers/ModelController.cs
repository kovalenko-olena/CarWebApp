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
using CarWebApp.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CarWebApp.Repository;
using Microsoft.AspNetCore.Identity;
using CarWebApp.ViewModels;

namespace CarWebApp.Controllers
{
	[Authorize]
	public class ModelController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ISprRepository<ModelSpr> _repository;
		private readonly ILogger<ModelController> _logger;
		private readonly IDataProtector protector;

		public ModelController(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			ISprRepository<ModelSpr> repository,
			ILogger<ModelController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_context = context;
			_userManager = userManager;
			_repository = repository;
			_logger = logger;
			protector = dataProtectionProvider
				 .CreateProtector(dataProtectionPurposeStrings.ModelIdRouteValue);
		}

		// GET: Model
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Index()
		{
			List<ModelViewModel> model = new List<ModelViewModel>();
			var spr = await _repository.GetAllItems();

			foreach (ModelSpr item in spr)
			{
				model.Add(new ModelViewModel(item, protector.Protect(item.Id.ToString())));
			};

			int count = model.ToList().Count();
			TempData["count"] = count;
			return View(model);
		}

		// GET: Model
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Details(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));

			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}

			var spr = await _repository.GetItem(decryptedId);

			if (spr == null)
			{
				Response.StatusCode = 404;
				return View("ModelNotFound", decryptedId);
			}
			ModelViewModel model = new ModelViewModel(spr, id);
			return View(model);
		}

		// GET: Model/Create
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create()
		{
			ModelViewModel model = new ModelViewModel();

			model.Cd = await _repository.GetLastCd() + 1;

			ViewBag.Fuel = new SelectList(await _context.FuelSpr.ToListAsync(), "Id", "Name");
			ViewBag.Brand = new SelectList(await _context.BrandSpr.ToListAsync(), "Id", "Name");
			return View(model);
		}

		// POST: Model/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create([Bind("Cd, Name, FuelSpr, BrandSpr")] ModelViewModel model)
		{
			// if (ModelState.IsValid)
			if (ModelState["Cd"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["Name"]?.ValidationState == ModelValidationState.Valid)
			{
				FuelSpr fuel = model?.FuelSpr ?? new FuelSpr();
				BrandSpr brand = model?.BrandSpr ?? new BrandSpr();

				ModelSpr spr = new ModelSpr
				{
					Id = await _repository.GetLastId() + 1,
					Cd = model?.Cd ?? 0,
					Name = model?.Name ?? string.Empty,
					FuelSpr = _context.FuelSpr.FirstOrDefault(c => c.Id == fuel.Id),
					BrandSpr = _context.BrandSpr.FirstOrDefault(c => c.Id == brand.Id),
					CreatedByUser = await _userManager.GetUserAsync(User),
					CreateDate = DateTime.Now,
				};

				await _repository.Add(spr);
				TempData["success"] = "Model created successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		// GET: Model/Edit/
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			var model = new ModelViewModel(await _repository.GetItem(decryptedId), id);

			ViewBag.Fuel = new SelectList(await _context.FuelSpr.ToListAsync(), "Id", "Name");
			ViewBag.Brand = new SelectList(await _context.BrandSpr.ToListAsync(), "Id", "Name");
			return View(model);
		}

		// POST: 
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id, [Bind("Cd, Name, FuelSpr, BrandSpr")] ModelViewModel sprNew)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			var sprCurrent = await _repository.GetItem(decryptedId);
			if (sprCurrent == null || sprNew == null)
			{
				return NotFound();
			}

			if (ModelState["Cd"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["Name"]?.ValidationState == ModelValidationState.Valid)
			{
				try
				{
					FuelSpr fuel = sprNew?.FuelSpr ?? new FuelSpr();
					BrandSpr brand = sprNew?.BrandSpr ?? new BrandSpr();
					ModelSpr spr = new ModelSpr
					{
						Id = decryptedId,
						Cd = sprNew?.Cd??0,
						Name = sprNew?.Name??string.Empty,
						FuelSpr = _context.FuelSpr.FirstOrDefault(c => c.Id == fuel.Id),
						BrandSpr = _context.BrandSpr.FirstOrDefault(c => c.Id == brand.Id),

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
				TempData["success"] = "Model edited successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(sprNew);
		}

		// GET: Model/Delete/
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
				var model = new ModelViewModel(await _repository.GetItem(decryptedId));
				if (model == null)
				{
					return NotFound();
				}

				return View(model);
			}
			
		}

		// POST: Model/Delete/
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			ApplicationUser? applicationUser = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == _userManager.GetUserId(User));
			_logger.LogInformation("Deleted by: UserTn  = " + applicationUser?.Tn.ToString());
			_logger.LogInformation("Deleted by: UserId  = " + applicationUser?.Id.ToString());
			_logger.LogInformation("Deleted by: UserEmail  = " + applicationUser?.Email.ToString());
			if (await _repository.GetAllItems() == null)
			{
				return Problem("Entity set 'Model'  is null.");
			}
			var spr = await _repository.GetItem(id);
			if (spr != null)
			{
				await _repository.Delete(id);
			}

			TempData["success"] = "Model deleted successfully";
			return RedirectToAction(nameof(Index));
		}


	}
}
