using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace CarWebApp.Repository
{
	public class WayBillRepository : IWayBillRepository
	{
		private readonly ApplicationDbContext context;
		public WayBillRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<WayBill>> GetAllItems()
		{
			//return await context.WayBill.ToListAsync();
			var sprQuery = await (from a in context.WayBill
								  select new
								  {
									  Id = a.Id,
									  Cd = a.Cd,
									  DtGive = a.DtGive,
									  DtReturn = a.DtReturn,
									  DtOut = a.DtOut,
									  DtIn = a.DtIn,
									  SpdOut = a.SpdOut,
									  SpdIn = a.SpdIn,
									  FuelBalOut = a.FuelBalOut,
									  FuelBalIn = a.FuelBalIn,
									  FuelFillUp = a.FuelFillUp,
									  FuelConsumNorm = a.FuelConsumNorm,
									  FuelConsumFact = a.FuelConsumFact,
									  DriverSpr = a.DriverSpr,
									  FuelSpr = a.FuelSpr,
									  VehicleSpr = a.VehicleSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).ToListAsync();

			List<WayBill> spr = new List<WayBill>();

			foreach (var sprItem in sprQuery)
			{
				spr.Add(new WayBill
				{
					Id = sprItem.Id,
					Cd = sprItem.Cd,
					DtGive = sprItem.DtGive,
					DtReturn = sprItem.DtReturn,
					DtOut = sprItem.DtOut,
					DtIn = sprItem.DtIn,
					SpdOut = sprItem.SpdOut,
					SpdIn = sprItem.SpdIn,
					FuelBalOut = sprItem.FuelBalOut,
					FuelBalIn = sprItem.FuelBalIn,
					FuelFillUp = sprItem.FuelFillUp,
					FuelConsumNorm = sprItem.FuelConsumNorm,
					FuelConsumFact = sprItem.FuelConsumFact,
					DriverSpr = sprItem.DriverSpr,
					FuelSpr = sprItem.FuelSpr,
					VehicleSpr = sprItem.VehicleSpr,
					CreatedByUser = sprItem.CreatedByUser,
					CreateDate = sprItem.CreateDate,
					EditedByUser = sprItem.EditedByUser,
					EditDate = sprItem.EditDate,
					EncryptedId = sprItem.EncryptedId
				});
			}

			return spr;
		}

		public async Task<WayBill> GetItem(int id)
		{
			var sprQuery = await (from a in context.WayBill
								  where a.Id == id
								  select new
								  {
									  Id = a.Id,
									  Cd = a.Cd,
									  DtGive = a.DtGive,
									  DtReturn = a.DtReturn,
									  DtOut = a.DtOut,
									  DtIn = a.DtIn,
									  SpdOut = a.SpdOut,
									  SpdIn = a.SpdIn,
									  FuelBalOut = a.FuelBalOut,
									  FuelBalIn = a.FuelBalIn,
									  FuelFillUp = a.FuelFillUp,
									  FuelConsumNorm = a.FuelConsumNorm,
									  FuelConsumFact = a.FuelConsumFact,
									  DriverSpr = a.DriverSpr,
									  FuelSpr = a.FuelSpr,
									  VehicleSpr = a.VehicleSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).FirstOrDefaultAsync();
			if (sprQuery!= null)
			{
				WayBill spr = new WayBill()
				{
					Id = sprQuery.Id,
					Cd = sprQuery.Cd,
					DtGive = sprQuery.DtGive,
					DtReturn = sprQuery.DtReturn,
					DtOut = sprQuery.DtOut,
					DtIn = sprQuery.DtIn,
					SpdOut = sprQuery.SpdOut,
					SpdIn = sprQuery.SpdIn,
					FuelBalOut = sprQuery.FuelBalOut,
					FuelBalIn = sprQuery.FuelBalIn,
					FuelFillUp = sprQuery.FuelFillUp,
					FuelConsumNorm = sprQuery.FuelConsumNorm,
					FuelConsumFact = sprQuery.FuelConsumFact,
					DriverSpr = sprQuery.DriverSpr,
					FuelSpr = sprQuery.FuelSpr,
					VehicleSpr = sprQuery.VehicleSpr,
					CreatedByUser = sprQuery.CreatedByUser,
					CreateDate = sprQuery.CreateDate,
					EditedByUser = sprQuery.EditedByUser,
					EditDate = sprQuery.EditDate,
					EncryptedId = sprQuery.EncryptedId
				};

				return spr;
			}
			return new WayBill();
		}

		public async Task<bool> FindItem(int id)
		{
			return await context.WayBill.AnyAsync(e => e.Id == id);
		}

		public async Task<WayBill> Add(WayBill spr)
		{
			context.WayBill.Add(spr);
			await context.SaveChangesAsync();
			return spr;
		}

		public async Task Delete(int id)
		{
			WayBill? spr = await context.WayBill.FirstOrDefaultAsync(m => m.Id == id);
			if (spr != null)
			{
				context.WayBill.Remove(spr);
				await context.SaveChangesAsync();
			}
		}


		public async Task<WayBill> Update(WayBill sprChanges)
		{
			WayBill? spr = await context.WayBill.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
			if (spr != null)
			{
				spr.Cd = sprChanges.Cd;
				spr.DtGive = sprChanges.DtGive;
				spr.DtReturn = sprChanges.DtReturn;
				spr.DtOut = sprChanges.DtOut;
				spr.DtIn = sprChanges.DtIn;
				spr.SpdOut = sprChanges.SpdOut;
				spr.SpdIn = sprChanges.SpdIn;
				spr.FuelBalOut = sprChanges.FuelBalOut;
				spr.FuelBalIn = sprChanges.FuelBalIn;
				spr.FuelFillUp = sprChanges.FuelFillUp;
				spr.FuelConsumNorm = sprChanges.FuelConsumNorm;
				spr.FuelConsumFact = sprChanges.FuelConsumFact;
				spr.DriverSpr = sprChanges.DriverSpr;
				spr.FuelSpr = sprChanges.FuelSpr;
				spr.VehicleSpr = sprChanges.VehicleSpr;
				spr.EditedByUser = sprChanges.EditedByUser;
				spr.EditDate = sprChanges.EditDate;

				var sprUpdate = context.WayBill.Attach(spr);
				sprUpdate.State = EntityState.Modified;
				context.SaveChanges();
				return spr;
			}
			return sprChanges;
		}

		public async Task<int> GetLastId()
		{
			var id = await context.WayBill.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
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
			var cd = await context.WayBill.OrderByDescending(x => x.Cd).FirstOrDefaultAsync();
			if (cd != null)
			{
				try
				{
					int result = Convert.ToInt32(cd.Cd);
					return result;
				}
				catch (Exception)
				{
					return 0;
				}
			}
			else { return 0; }
		}

		public async Task<decimal> GetLastFuel(VehicleSpr vehicle)
		{
			var id = await context.WayBill.OrderByDescending(x => x.Id).Where(y=>y.VehicleSpr==vehicle).FirstOrDefaultAsync();
			if (id != null)
			{
				var sprQuery = await (from a in context.WayBill
									  where a.Id == id.Id
									  select new
									  {
										  FuelBalIn = a.FuelBalIn,
									  }).FirstOrDefaultAsync();
				return sprQuery?.FuelBalIn??0;
			}
			return 0;
			
		}
		public async Task<int> GetLastSpd(VehicleSpr vehicle)
		{
			var id = await context.WayBill.OrderByDescending(x => x.Id).Where(y => y.VehicleSpr == vehicle).FirstOrDefaultAsync();
			if (id != null)
			{
				var sprQuery = await (from a in context.WayBill
									  where a.Id == id.Id
									  select new
									  {
										  SpdIn = a.SpdIn,
									  }).FirstOrDefaultAsync();
				return sprQuery?.SpdIn ?? 0;
			}
			return 0;
		}
	}
}
