using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CarWebApp.Repository
{
	public class FuelRepository : ISprRepository<FuelSpr>
	{
		private readonly ApplicationDbContext context;
		public FuelRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public async Task<IEnumerable<FuelSpr>> GetAllItems()
		{
			return await context.FuelSpr.ToListAsync();
		}

		public async Task<FuelSpr> GetItem(int id)
		{
			// don't have properties CreatedByUser and EditedByUser
			// FuelSpr spr = await context.FuelSpr.FirstOrDefaultAsync(m => m.Id == Id);

			var sprQuery = await (from a in context.FuelSpr
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
				FuelSpr spr = new FuelSpr()
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
			return new FuelSpr();
		}

		public async Task<bool> FindItem(int id)
		{
			return await context.FuelSpr.AnyAsync(e => e.Id == id);
		}

		public async Task<FuelSpr> Add(FuelSpr spr)
		{
			context.FuelSpr.Add(spr);
			await context.SaveChangesAsync();
			return spr;
		}

		public async Task Delete(int id)
		{
			FuelSpr? spr = await context.FuelSpr.FirstOrDefaultAsync(m => m.Id == id);
			if (spr != null)
			{
				context.FuelSpr.Remove(spr);
				await context.SaveChangesAsync();
			}
		}


		public async Task<FuelSpr> Update(FuelSpr sprChanges)
		{
			FuelSpr? spr = await context.FuelSpr.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
			if (spr != null)
			{
				spr.Cd = sprChanges.Cd;
				spr.Name = sprChanges.Name;
				spr.EditedByUser = sprChanges.EditedByUser;
				spr.EditDate = sprChanges.EditDate;

				var sprUpdate = context.FuelSpr.Attach(spr);
				sprUpdate.State = EntityState.Modified;
				context.SaveChanges();

				return spr;
			}
			return sprChanges;
		}

		public async Task<int> GetLastId()
		{
			var id = await context.FuelSpr.OrderByDescending(x => x.Id).FirstOrDefaultAsync();

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
			return 0;
		}

		public async Task<int> GetLastCd()
		{
			var cd = await context.FuelSpr.OrderByDescending(x => x.Cd).FirstOrDefaultAsync();

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
			return 0;
		}

		public ApplicationUser GetCreatedUser(int id)
		{
			var user = (from a in context.FuelSpr
						where a.Id.Equals(id)
						select a.CreatedByUser).ToList().First();

			return user;
		}

		public ApplicationUser? GetEditedUser(int id)
		{
			var user = (from a in context.FuelSpr
						where a.Id.Equals(id)
						select a.EditedByUser).ToList().First();

			return user;
		}
	}
}
