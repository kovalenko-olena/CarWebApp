using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarWebApp.Repository
{
	public class VehicleRepository : ISprRepository<VehicleSpr>
	{
		private readonly ApplicationDbContext context;
		public VehicleRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<VehicleSpr>> GetAllItems()
		{
			//return await context.VehicleSpr.ToListAsync();
			var sprQuery = await (from a in context.VehicleSpr

								  select new
								  {
									  Id = a.Id,
									  RegNumber = a.RegNumber,
									  GarNumber = a.GarNumber,
									  WayBills = a.WayBills,
									  Norm = a.Norm,
									  DriverSpr = a.DriverSpr,
									  ModelSpr = a.ModelSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).ToListAsync();

			List<VehicleSpr> spr = new List<VehicleSpr>();
			foreach (var sprItem in sprQuery)
			{
				spr.Add(new VehicleSpr
				{
					Id = sprItem.Id,
					RegNumber = sprItem.RegNumber,
					GarNumber = sprItem.GarNumber,
					WayBills = sprItem.WayBills,
					Norm = sprItem.Norm,
					DriverSpr = sprItem.DriverSpr,
					ModelSpr = sprItem.ModelSpr,
					CreatedByUser = sprItem.CreatedByUser,
					CreateDate = sprItem.CreateDate,
					EditedByUser = sprItem.EditedByUser,
					EditDate = sprItem.EditDate,
					EncryptedId = sprItem.EncryptedId
				});
			}
			return spr;
		}

		public async Task<VehicleSpr> GetItem(int id)
		{
			var sprQuery = await (from a in context.VehicleSpr
								  where a.Id == id
								  select new
								  {
									  Id = a.Id,
									  RegNumber = a.RegNumber,
									  GarNumber = a.GarNumber,
									  Norm = a.Norm,
									  DriverSpr = a.DriverSpr,
									  ModelSpr = a.ModelSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).FirstOrDefaultAsync();
			if(sprQuery != null)
			{
				VehicleSpr spr = new VehicleSpr()
				{
					Id = sprQuery.Id,
					RegNumber = sprQuery.RegNumber,
					GarNumber = sprQuery.GarNumber,
					Norm = sprQuery.Norm,
					DriverSpr = sprQuery.DriverSpr,
					ModelSpr = sprQuery.ModelSpr,
					CreatedByUser = sprQuery.CreatedByUser,
					CreateDate = sprQuery.CreateDate,
					EditedByUser = sprQuery.EditedByUser,
					EditDate = sprQuery.EditDate,
					EncryptedId = sprQuery.EncryptedId
				};

				return spr;
			}
			return new VehicleSpr();
		}

		public async Task<bool> FindItem(int id)
		{
			return await context.VehicleSpr.AnyAsync(e => e.Id == id);
		}

		public async Task<VehicleSpr> Add(VehicleSpr spr)
		{
			context.VehicleSpr.Add(spr);
			await context.SaveChangesAsync();
			return spr;
		}

		public async Task Delete(int id)
		{
			VehicleSpr? spr = await context.VehicleSpr.FirstOrDefaultAsync(m => m.Id == id);
			if (spr != null)
			{
				context.VehicleSpr.Remove(spr);
				await context.SaveChangesAsync();
			}
		}


		public async Task<VehicleSpr> Update(VehicleSpr sprChanges)
		{
			VehicleSpr? spr = await context.VehicleSpr.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
			if (spr != null)
			{
				spr.RegNumber = sprChanges.RegNumber;
				spr.GarNumber = sprChanges.GarNumber;
				spr.DriverSpr = sprChanges.DriverSpr;
				spr.ModelSpr = sprChanges.ModelSpr;
				spr.Norm = sprChanges.Norm;
				spr.EditedByUser = sprChanges.EditedByUser;
				spr.EditDate = sprChanges.EditDate;

				var sprUpdate = context.VehicleSpr.Attach(spr);
				sprUpdate.State = EntityState.Modified;
				context.SaveChanges();
				return spr;
			}
			return sprChanges;
		}

		public async Task<int> GetLastId()
		{
			var id = await context.VehicleSpr.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
			if (id != null)
			{
				try
				{
					int result = Convert.ToInt32(id.Id);
					return result;
				}
				catch (Exception)
				{
					return 0;
				}
			}
			else { return 0; }
		}

		public async Task<int> GetLastCd()
		{
			var cd = await context.VehicleSpr.OrderByDescending(x => x.GarNumber).FirstOrDefaultAsync();
			if (cd != null)
			{
				try
				{
					int result = Convert.ToInt32(cd.GarNumber);
					return result;
				}
				catch (Exception)
				{
					return 0;
				}
			}
			else { return 0; }
		}

	}
}
