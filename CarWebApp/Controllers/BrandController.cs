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
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISprRepository<BrandSpr> _repository;
        private readonly ILogger<BrandController> _logger;
        private readonly IDataProtector protector;

        public BrandController(ApplicationDbContext context,
            UserManager<Models.ApplicationUser> userManager,
            ISprRepository<BrandSpr> repository,
            ILogger<BrandController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
            _logger = logger;
            protector = dataProtectionProvider
                 .CreateProtector(dataProtectionPurposeStrings.BrandIdRouteValue);
        }

        // GET: Brand
        [Authorize(Policy = "AdministratorOrMaster")]
        public async Task<IActionResult> Index()
        {
            List<BrandViewModel> model = new List<BrandViewModel>();
            var spr = await _repository.GetAllItems();

            foreach (BrandSpr item in spr)
            {
                model.Add(new BrandViewModel(item, protector.Protect(item.Id.ToString())));
            };

            int count = model.ToList().Count();
            TempData["count"] = count;
            return View(model);
        }

        // GET: Brand
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
                return View("BrandNotFound", decryptedId);
            }
            BrandViewModel model = new BrandViewModel(spr, id);
            return View(model);
        }

        // GET: Brand/Create
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Create()
        {
            BrandViewModel model = new BrandViewModel();
            model.Cd =  await _repository.GetLastCd() + 1;

            return View(model);
        }

        // POST: Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Create([Bind("Cd,Name")] BrandViewModel model)
        {
            if (ModelState.IsValid)
            {
                BrandSpr spr = new BrandSpr
                {
                    Id = await _repository.GetLastId() + 1,
                    Cd = model.Cd,
                    Name = model.Name,
                    CreatedByUser = await _userManager.GetUserAsync(User),
                    CreateDate = DateTime.Now,
                };

                await _repository.Add(spr);
                TempData["success"] = "Brand created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Brand/Edit/
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Edit(string id)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(id));
            if (id == null || await _repository.GetAllItems() == null)
            {
                return NotFound();
            }
            var model = new BrandViewModel(await _repository.GetItem(decryptedId), id);

            return View(model);
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Edit(string id, [Bind("Cd,Name")] BrandViewModel sprNew)
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
                    BrandSpr spr = new BrandSpr
                    {
                        Id = decryptedId,
                        Cd = sprNew.Cd,
                        Name = sprNew.Name,
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
                TempData["success"] = "Brand edited successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(sprNew);
        }

        // GET: Brand/Delete/
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
				var model = new BrandViewModel(await _repository.GetItem(decryptedId));
				if (model == null)
				{
					return NotFound();
				}

				return View(model);
			}
			
        }

        // POST: Brand/Delete/
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
                return Problem("Entity set 'BrandSpr'  is null.");
            }
            var spr = await _repository.GetItem(id);
            if (spr != null)
            {
                await _repository.Delete(id);
            }

            TempData["success"] = "Brand deleted successfully";
            return RedirectToAction(nameof(Index));
        }


    }
}
