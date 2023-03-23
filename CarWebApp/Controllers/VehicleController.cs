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
using System.Security.Cryptography;

namespace CarWebApp.Controllers
{
	[Authorize]
	public class VehicleController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ISprRepository<VehicleSpr> _repository;
		private readonly ILogger<VehicleController> _logger;
		private readonly IDataProtector protector;

		public VehicleController(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			ISprRepository<VehicleSpr> repository,
			ILogger<VehicleController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_context = context;
			_userManager = userManager;
			_repository = repository;
			_logger = logger;
			protector = dataProtectionProvider
				 .CreateProtector(dataProtectionPurposeStrings.VehicleIdRouteValue);
		}

		// GET: Vehicle
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Index()
		{
			List<VehicleViewModel> model = new List<VehicleViewModel>();
			var spr = await _repository.GetAllItems();

			foreach (VehicleSpr item in spr)
			{
				model.Add(new VehicleViewModel(item, protector.Protect(item.Id.ToString())));
			};

			int count = model.ToList().Count();
			TempData["count"] = count;
			return View(model);
		}

		// GET: Vehicle
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
				return View("VehicleNotFound", decryptedId);
			}
			VehicleViewModel model = new VehicleViewModel(spr, id);
			return View(model);
		}

		// GET: Vehicle/Create
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create()
		{
			VehicleViewModel model = new VehicleViewModel();
			model.GarNumber = await _repository.GetLastCd() + 1;

			var drivers = await _context.DriverSpr
				.Select(s => new
				{
					s.Id,
					Name = s.FirstName + " " + s.LastName
				})
				.ToListAsync();
			ViewData["Model"] = new SelectList(_context.ModelSpr, "Id", "Name");
			ViewData["Driver"] = new SelectList(drivers, "Id", "Name");
			return View(model);
		}

		// POST: Vehicle/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Create([Bind("GarNumber, RegNumber, Norm, DriverSpr, ModelSpr")] VehicleViewModel model)
		{
			if (ModelState["GarNumber"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["RegNumber"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["Norm"]?.ValidationState == ModelValidationState.Valid)
			{
				DriverSpr driverSpr = model?.DriverSpr ?? new DriverSpr();
				ModelSpr modelSpr = model?.ModelSpr ?? new ModelSpr();
				VehicleSpr spr = new VehicleSpr
				{
					Id = await _repository.GetLastId() + 1,
					GarNumber = model?.GarNumber,
					RegNumber = model?.RegNumber ?? string.Empty,
					Norm = model?.Norm ?? 0,
					DriverSpr = await _context.DriverSpr.FirstOrDefaultAsync(c => c.Id == driverSpr.Id),
					ModelSpr = await _context.ModelSpr.FirstOrDefaultAsync(c => c.Id == modelSpr.Id),

					CreatedByUser = await _userManager.GetUserAsync(User),
					CreateDate = DateTime.Now,
				};

				await _repository.Add(spr);
				TempData["success"] = "Vehicle created successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		// GET: Vehicle/Edit/
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			var model = new VehicleViewModel(await _repository.GetItem(decryptedId), id);
			var drivers = await _context.DriverSpr
				.Select(s => new
				{
					s.Id,
					Name = s.FirstName + " " + s.LastName
				})
				.ToListAsync();
			ViewData["Model"] = new SelectList(_context.ModelSpr, "Id", "Name");
			ViewData["Driver"] = new SelectList(drivers, "Id", "Name");
			return View(model);
		}

		// POST: 
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrator")]
		public async Task<IActionResult> Edit(string id, [Bind("GarNumber, RegNumber, Norm, DriverSpr, ModelSpr")] VehicleViewModel sprNew)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			var sprCurrent = await _repository.GetItem(decryptedId);
			if (sprCurrent == null || sprNew == null)
			{
				return NotFound();
			}

			if (ModelState["GarNumber"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["RegNumber"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["Norm"]?.ValidationState == ModelValidationState.Valid)
			{
				try
				{
					DriverSpr driver = sprNew?.DriverSpr ?? new DriverSpr();
					ModelSpr model = sprNew?.ModelSpr ?? new ModelSpr();
					VehicleSpr spr = new()
					{
						Id = decryptedId,
						GarNumber = sprNew?.GarNumber,
						RegNumber = sprNew?.RegNumber ?? string.Empty,
						Norm = sprNew?.Norm,
						DriverSpr = _context.DriverSpr.FirstOrDefault(c => c.Id == driver.Id),
						ModelSpr = _context.ModelSpr.FirstOrDefault(c => c.Id == model.Id),

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
				TempData["success"] = "Vehicle edited successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(sprNew);
		}

		// GET: Vehicle/Delete/
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
				var model = new VehicleViewModel(await _repository.GetItem(decryptedId));
				if (model == null)
				{
					return NotFound();
				}

				return View(model);
			}
		}

		// POST: Vehicle/Delete/
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
				return Problem("Entity set 'Vehicle'  is null.");
			}
			var spr = await _repository.GetItem(id);
			if (spr != null)
			{
				await _repository.Delete(id);
			}

			TempData["success"] = "Vehicle deleted successfully";
			return RedirectToAction(nameof(Index));
		}


	}
}
