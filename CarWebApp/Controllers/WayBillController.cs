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
using NuGet.Protocol.Core.Types;
using CarWebApp.Infrastructure;
using FastReport.Web;

namespace CarWebApp.Controllers
{
	[Authorize]
	public class WayBillController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWayBillRepository _repository;
		private readonly ILogger<WayBillController> _logger;
		private readonly IDataProtector protector;
		public int PageSize = 4;

		public WayBillController(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			IWayBillRepository repository,
			ILogger<WayBillController> logger,
			IDataProtectionProvider dataProtectionProvider,
			DataProtectionPurposeStrings dataProtectionPurposeStrings)
		{
			_context = context;
			_userManager = userManager;
			_repository = repository;
			_logger = logger;
			protector = dataProtectionProvider
				 .CreateProtector(dataProtectionPurposeStrings.WayBillIdRouteValue);
		}

		// GET: WayBill
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Index(int itemPage = 1)
		{
			List<WayBillViewModel> model = new List<WayBillViewModel>();
			var spr = await _repository.GetAllItems();

			foreach (WayBill item in spr)
			{
				model.Add(new WayBillViewModel(item, protector.Protect(item.Id.ToString())));
			};

			int count = model.ToList().Count();
			TempData["count"] = count;

			//return View(model);
			return View(new WayBillListViewModel
			{
				WayBillViews = model
				   .OrderBy(p => p.Cd)
				   .Skip((itemPage - 1) * PageSize)
				   .Take(PageSize),
				PagingInfo = new PagingInfo
				{
					CurrentPage = itemPage,
					ItemsPerPage = PageSize,
					TotalItems = count
				}
			});


		}

		// GET: WayBill
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
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
				return View("WayBillNotFound", decryptedId);
			}
			WayBillViewModel model = new WayBillViewModel(spr, id);
			return View(model);
		}

		// GET: WayBill/Create
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Create()
		{
			WayBillViewModel model = new WayBillViewModel();
			model.Cd = await _repository.GetLastCd() + 1;
			model.DtGive = DateTime.Now;
			model.DtReturn = DateTime.Now;
			model.DtOut= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
				DateTime.Now.Hour, DateTime.Now.Minute, 0);
			model.DtIn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
				DateTime.Now.Hour, DateTime.Now.Minute, 0);
				
			var drivers = await _context.DriverSpr
				.Select(s => new
				{
					s.Id,
					Name = s.FirstName + " " + s.LastName
				})
				.ToListAsync();
			ViewData["Vehicle"] = new SelectList(_context.VehicleSpr, "Id", "RegNumber");
			ViewData["Fuel"] = new SelectList(_context.FuelSpr, "Id", "Name");
			ViewData["Driver"] = new SelectList(drivers, "Id", "Name");
			return View(model);
		}

		// POST: WayBill/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Create([Bind("Cd, DtGive, DtReturn, DtOut, DtIn, " +
			"SpdOut, SpdIn, FuelBalOut, FuelBalIn, FuelFillUp, FuelConsumFact, " +
			"DriverSpr, FuelSpr, VehicleSpr")] WayBillViewModel model)
		{
			if (ModelState["Cd"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["DtGive"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["DtReturn"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["DtOut"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["DtIn"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["SpdOut"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["SpdIn"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["FuelBalOut"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["FuelBalIn"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["FuelFillUp"]?.ValidationState == ModelValidationState.Valid &&
				ModelState["FuelConsumFact"]?.ValidationState == ModelValidationState.Valid
				)
			{
				DriverSpr driver = model?.DriverSpr ?? new DriverSpr();
				FuelSpr fuel = model?.FuelSpr ?? new FuelSpr();
				VehicleSpr vehicle = model?.VehicleSpr ?? new VehicleSpr();

				var norm = _context.VehicleSpr
				.Select(s => new
				{
					Id = s.Id,
					Norm = s.Norm
				})
				.FirstOrDefault(c => c.Id == vehicle.Id)?.Norm;

				WayBill spr = new WayBill
				{
					Id = await _repository.GetLastId() + 1,
					Cd = model?.Cd ?? 0,
					DtGive = model?.DtGive,
					DtReturn = model?.DtReturn,
					DtOut = model?.DtOut,
					DtIn = model?.DtIn,
					SpdOut = model?.SpdOut,
					SpdIn = model?.SpdIn,
					FuelBalOut = model?.FuelBalOut,
					FuelBalIn = model?.FuelBalIn,
					FuelFillUp = model?.FuelFillUp,

					FuelConsumNorm = ((model?.SpdIn??0) - (model?.SpdOut??0)) * norm/100,
					FuelConsumFact = model?.FuelConsumFact,
					DriverSpr = _context.DriverSpr.FirstOrDefault(c => c.Id == driver.Id),
					FuelSpr = _context.FuelSpr.FirstOrDefault(c => c.Id == fuel.Id),
					VehicleSpr = _context.VehicleSpr.FirstOrDefault(c => c.Id == vehicle.Id),


					CreatedByUser = await _userManager.GetUserAsync(User),
					CreateDate = DateTime.Now,
				};

				await _repository.Add(spr);
				TempData["success"] = "WayBill created successfully";
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		// GET: WayBill/Edit/
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Edit(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			var model = new WayBillViewModel(await _repository.GetItem(decryptedId), id);

			var drivers = _context.DriverSpr
				.Select(s => new
				{
					s.Id,
					Name = s.FirstName + " " + s.LastName
				})
				.ToList();

			ViewData["Vehicle"] = new SelectList(_context.VehicleSpr, "Id", "RegNumber");
			ViewData["Fuel"] = new SelectList(_context.FuelSpr, "Id", "Name");
			ViewData["Driver"] = new SelectList(drivers, "Id", "Name");
			return View(model);
		}

		// POST: 
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Edit(string id, [Bind("Cd, DtGive, DtReturn, DtOut, DtIn, " +
			"SpdOut, SpdIn, FuelBalOut, FuelBalIn, FuelFillUp, FuelConsumFact, " +
			"DriverSpr, FuelSpr, VehicleSpr")] WayBillViewModel sprNew)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			var sprCurrent = await _repository.GetItem(decryptedId);
			if (sprCurrent == null || sprNew == null)
			{
				return NotFound();
			}

			if (ModelState["Cd"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["DtGive"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["DtReturn"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["DtOut"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["DtIn"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["SpdOut"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["SpdIn"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["FuelBalOut"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["FuelBalIn"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["FuelFillUp"]?.ValidationState == ModelValidationState.Valid &&
					   ModelState["FuelConsumFact"]?.ValidationState == ModelValidationState.Valid
					   )
			{
				try
				{
					DriverSpr driver = sprNew?.DriverSpr ?? new DriverSpr();
					FuelSpr fuel = sprNew?.FuelSpr ?? new FuelSpr();
					VehicleSpr vehicle = sprNew?.VehicleSpr ?? new VehicleSpr();

					var norm = _context.VehicleSpr
				.Select(s => new
				{
					Id = s.Id,
					Norm = s.Norm
				})
				.FirstOrDefault(c => c.Id == vehicle.Id)?.Norm;

					WayBill spr = new WayBill
					{
						Id = decryptedId,
						Cd = sprNew?.Cd,
						DtGive = sprNew?.DtGive,
						DtReturn = sprNew?.DtReturn,
						DtOut = sprNew?.DtOut,
						DtIn = sprNew?.DtIn,
						SpdOut = sprNew?.SpdOut,
						SpdIn = sprNew?.SpdIn,
						FuelBalOut = sprNew?.FuelBalOut,
						FuelBalIn = sprNew?.FuelBalIn,
						FuelFillUp = sprNew?.FuelFillUp,

						FuelConsumNorm = ((sprNew?.SpdIn??0) - (sprNew?.SpdOut??0)) * norm/100,
						FuelConsumFact = sprNew?.FuelConsumFact,
						DriverSpr = _context.DriverSpr.FirstOrDefault(c => c.Id == driver.Id),
						FuelSpr = _context.FuelSpr.FirstOrDefault(c => c.Id == fuel.Id),
						VehicleSpr = _context.VehicleSpr.FirstOrDefault(c => c.Id == vehicle.Id),
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

		// GET: WayBill/Delete/
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> Delete(string? id)
		{
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}
			else
			{
				int decryptedId = Convert.ToInt32(protector.Unprotect(id));
				var model = new WayBillViewModel(await _repository.GetItem(decryptedId));
				if (model == null)
				{
					return NotFound();
				}

				return View(model);
			}
		}

		// POST: WayBill/Delete/
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdministratorOrMaster")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			ApplicationUser? applicationUser = await _userManager.Users.FirstOrDefaultAsync(m => m.Id == _userManager.GetUserId(User));
			_logger.LogInformation("Deleted by: UserTn  = " + applicationUser?.Tn.ToString());
			_logger.LogInformation("Deleted by: UserId  = " + applicationUser?.Id.ToString());
			_logger.LogInformation("Deleted by: UserEmail  = " + applicationUser?.Email.ToString());

			if (await _repository.GetAllItems() == null)
			{
				return Problem("Entity set 'WayBill'  is null.");
			}
			var spr = await _repository.GetItem(id);
			if (spr != null)
			{
				await _repository.Delete(id);
			}

			TempData["success"] = "WayBill deleted successfully";
			return RedirectToAction(nameof(Index));
		}

		// GET: WayBill/Print/
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> Print(string id)
		{
			int decryptedId = Convert.ToInt32(protector.Unprotect(id));
			if (id == null || await _repository.GetAllItems() == null)
			{
				return NotFound();
			}

			//FastReport.Utils.Config.WebMode = true;
			FastReport.Web.WebReport wr = new FastReport.Web.WebReport();
			wr.Report.Load("wwwroot/lib/report/report.frx");
			wr.Report.SetParameterValue("id", decryptedId);

			return View(wr);
		}

		// GET: WayBill/Print/
		[Authorize(Policy = "AdministratorOrMasterOrOperator")]
		public async Task<IActionResult> PrintReport(DateTime dt1, DateTime dt2)
		{
			
			if (await _repository.GetAllItems() == null)
			{
				return NotFound();
			}

			//FastReport.Utils.Config.WebMode = true;
			FastReport.Web.WebReport wr = new FastReport.Web.WebReport();
			

			wr.Report.Load("wwwroot/lib/report/reportAll.frx");
			if (dt1.Year<1900 || dt2.Year < 1900)
			{
				dt1 = DateTime.Today;
				dt2 = DateTime.Today;
			}
			wr.Report.SetParameterValue("dt1", dt1);
			wr.Report.SetParameterValue("dt2", dt2);
			ViewData["dt1"] = dt1.ToString("yyyy-MM-dd");
			ViewData["dt2"] = dt2.ToString("yyyy-MM-dd");
			ViewBag.WebReport = wr; //pass the report to View
			return View();
		}

	}
}
