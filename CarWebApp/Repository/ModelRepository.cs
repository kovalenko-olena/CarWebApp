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
	public class ModelRepository : ISprRepository<ModelSpr>
	{
		private readonly ApplicationDbContext context;
		public ModelRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<ModelSpr>> GetAllItems()
		{
			//return await context.ModelSpr.ToListAsync();
			var sprQuery = await (from a in context.ModelSpr
								  select new
								  {
									  Id = a.Id,
									  Cd = a.Cd,
									  Name = a.Name,
									  FuelSpr = a.FuelSpr,
									  BrandSpr = a.BrandSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).ToListAsync();

			List<ModelSpr> spr = new List<ModelSpr>();
			foreach (var sprItem in sprQuery)
			{
				spr.Add(new ModelSpr
				{
					Id = sprItem.Id,
					Cd = sprItem.Cd,
					Name = sprItem.Name,
					FuelSpr = sprItem.FuelSpr,
					BrandSpr = sprItem.BrandSpr,
					CreatedByUser = sprItem.CreatedByUser,
					CreateDate = sprItem.CreateDate,
					EditedByUser = sprItem.EditedByUser,
					EditDate = sprItem.EditDate,
					EncryptedId = sprItem.EncryptedId
				});
			}
			return spr;
		}

		public async Task<ModelSpr> GetItem(int id)
		{
			var sprQuery = await (from a in context.ModelSpr
								  where a.Id == id
								  select new
								  {
									  Id = a.Id,
									  Cd = a.Cd,
									  Name = a.Name,
									  FuelSpr = a.FuelSpr,
									  BrandSpr = a.BrandSpr,
									  CreatedByUser = a.CreatedByUser,
									  CreateDate = a.CreateDate,
									  EditedByUser = a.EditedByUser,
									  EditDate = a.EditDate,
									  EncryptedId = a.EncryptedId
								  }).FirstOrDefaultAsync();
			if (sprQuery != null)
			{
				ModelSpr spr = new ModelSpr()
				{
					Id = sprQuery.Id,
					Cd = sprQuery.Cd,
					Name = sprQuery.Name,
					FuelSpr = sprQuery.FuelSpr,
					BrandSpr = sprQuery.BrandSpr,
					CreatedByUser = sprQuery.CreatedByUser,
					CreateDate = sprQuery.CreateDate,
					EditedByUser = sprQuery.EditedByUser,
					EditDate = sprQuery.EditDate,
					EncryptedId = sprQuery.EncryptedId
				};

				return spr;
			}
			return new ModelSpr();
		}

		public async Task<bool> FindItem(int id)
		{
			return await context.ModelSpr.AnyAsync(e => e.Id == id);
		}

		public async Task<ModelSpr> Add(ModelSpr spr)
		{
			context.ModelSpr.Add(spr);
			await context.SaveChangesAsync();
			return spr;
		}

		public async Task Delete(int id)
		{
			ModelSpr? spr = await context.ModelSpr.FirstOrDefaultAsync(m => m.Id == id);
			if (spr != null)
			{
				context.ModelSpr.Remove(spr);
				await context.SaveChangesAsync();
			}
		}


		public async Task<ModelSpr> Update(ModelSpr sprChanges)
		{
			ModelSpr? spr = await context.ModelSpr.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
			if (spr != null)
			{
				spr.Cd = sprChanges.Cd;
				spr.Name = sprChanges.Name;
				spr.FuelSpr = sprChanges.FuelSpr;
				spr.BrandSpr = sprChanges.BrandSpr;
				spr.EditedByUser = sprChanges.EditedByUser;
				spr.EditDate = sprChanges.EditDate;

				var sprUpdate = context.ModelSpr.Attach(spr);
				sprUpdate.State = EntityState.Modified;
				context.SaveChanges();
				return spr;
			}
			return sprChanges;
		}

		public async Task<int> GetLastId()
		{
			var id = await context.ModelSpr.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
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
			var cd = await context.ModelSpr.OrderByDescending(x => x.Cd).FirstOrDefaultAsync();
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
