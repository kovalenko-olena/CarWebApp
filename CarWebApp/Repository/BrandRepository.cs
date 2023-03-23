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
	public class BrandRepository : ISprRepository<BrandSpr>
	{
		private readonly ApplicationDbContext context;
		public BrandRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<BrandSpr>> GetAllItems()
		{
			return await context.BrandSpr.ToListAsync();
		}

		public async Task<BrandSpr> GetItem(int id)
		{
			var sprQuery = await (from a in context.BrandSpr
								  where a.Id == id
								  select new
								  {
									  Id = a.Id,
									  Cd = a.Cd,
									  Name = a.Name,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).FirstOrDefaultAsync();
			if (sprQuery != null)
			{
				BrandSpr spr = new BrandSpr()
				{
					Id = sprQuery.Id,
					Cd = sprQuery.Cd,
					Name = sprQuery.Name,
					CreatedByUser = sprQuery.CreatedByUser,
					CreateDate = sprQuery.CreateDate,
					EditedByUser = sprQuery.EditedByUser,
					EditDate = sprQuery.EditDate,
					EncryptedId = sprQuery.EncryptedId
				};

				return spr;
			}
			return new BrandSpr();

		}

		public async Task<bool> FindItem(int id)
		{
			return await context.BrandSpr.AnyAsync(e => e.Id == id);
		}

		public async Task<BrandSpr> Add(BrandSpr spr)
		{
			context.BrandSpr.Add(spr);
			await context.SaveChangesAsync();
			return spr;
		}

		public async Task Delete(int id)
		{
			BrandSpr? spr = await context.BrandSpr.FirstOrDefaultAsync(m => m.Id == id);
			if (spr != null)
			{
				context.BrandSpr.Remove(spr);
				await context.SaveChangesAsync();
			}
		}


		public async Task<BrandSpr> Update(BrandSpr sprChanges)
		{
			BrandSpr? spr = await context.BrandSpr.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
			if (spr != null)
			{
				spr.Cd = sprChanges.Cd;
				spr.Name = sprChanges.Name;
				spr.EditedByUser = sprChanges.EditedByUser;
				spr.EditDate = sprChanges.EditDate;

				var sprUpdate = context.BrandSpr.Attach(spr);
				sprUpdate.State = EntityState.Modified;
				context.SaveChanges();
				return spr;
			}
			return sprChanges;
		}

		public async Task<int> GetLastId()
		{
			var id = await context.BrandSpr.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
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
			var cd = await context.BrandSpr.OrderByDescending(x => x.Cd).FirstOrDefaultAsync();
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

	}
}
