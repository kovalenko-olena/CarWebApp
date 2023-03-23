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
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISprRepository<DriverSpr> _repository;
        private readonly ILogger<DriverController> _logger;
        private readonly IDataProtector protector;

        public DriverController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ISprRepository<DriverSpr> repository,
            ILogger<DriverController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
            _logger = logger;
            protector = dataProtectionProvider
                 .CreateProtector(dataProtectionPurposeStrings.DriverIdRouteValue);
        }

        // GET: Driver
        [Authorize(Policy = "AdministratorOrMaster")]
        public async Task<IActionResult> Index()
        {
            List<DriverViewModel> model = new List<DriverViewModel>();
            var spr = await _repository.GetAllItems();

            foreach (DriverSpr item in spr)
            {
                model.Add(new DriverViewModel(item, protector.Protect(item.Id.ToString())));
            };

            int count = model.ToList().Count();
            TempData["count"] = count;
            return View(model);
        }

        // GET: Driver
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
                return View("DriverNotFound", decryptedId);
            }
            DriverViewModel model = new DriverViewModel(spr, id);
            return View(model);
        }

        // GET: Driver/Create
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Create()
        {
            DriverViewModel model = new DriverViewModel();
            model.Tn = await _repository.GetLastCd() + 1;

            return View(model);
        }

        // POST: Driver/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Create([Bind("Tn, FirstName, LastName")] DriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                DriverSpr spr = new DriverSpr
                {
                    Id = await _repository.GetLastId() + 1,
                    Tn = model.Tn,
                    FirstName = model?.FirstName??string.Empty,
                    LastName = model?.LastName??string.Empty,

                    CreatedByUser = await _userManager.GetUserAsync(User),
                    CreateDate = DateTime.Now,
                };

                await _repository.Add(spr);
                TempData["success"] = "Driver created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Driver/Edit/
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Edit(string id)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(id));
            if (id == null || await _repository.GetAllItems() == null)
            {
                return NotFound();
            }
            var model = new DriverViewModel(await _repository.GetItem(decryptedId), id);

            return View(model);
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Edit(string id, [Bind("Tn, FirstName, LastName")] DriverViewModel sprNew)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(id));
            var sprCurrent = await _repository.GetItem(decryptedId);
            if (sprCurrent == null || sprNew == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DriverSpr spr = new DriverSpr
                    {
                        Id = decryptedId,
                        Tn = sprNew.Tn,
                        FirstName = sprNew?.FirstName ?? string.Empty,
                        LastName = sprNew?.LastName ?? string.Empty,

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
                TempData["success"] = "Driver edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(sprNew);
        }

        // GET: Driver/Delete/
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
                var model = new DriverViewModel(await _repository.GetItem(decryptedId));

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }
        }

        // POST: Driver/Delete/
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
                return Problem("Entity set 'Driver'  is null.");
            }
            var spr = await _repository.GetItem(id);
            if (spr != null)
            {
                await _repository.Delete(id);
            }

            TempData["success"] = "Driver deleted successfully";
            return RedirectToAction(nameof(Index));
        }


    }
}
