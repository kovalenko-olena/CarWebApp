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
    public class DriverRepository : ISprRepository<DriverSpr>
    {
        private readonly ApplicationDbContext context;
        public DriverRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DriverSpr>> GetAllItems()
        {
            return await context.DriverSpr.ToListAsync();
        }

        public async Task<DriverSpr> GetItem(int id)
        {
            var sprQuery = await (from a in context.DriverSpr
                                  where a.Id == id
                                  select new
                                  {
                                      Id = a.Id,
                                      Tn = a.Tn,
                                      FirstName = a.FirstName,
                                      LastName = a.LastName,
                                      CreatedByUser = a.CreatedByUser,
                                      CreateDate = a.CreateDate,
                                      EditedByUser = a.EditedByUser,
                                      EditDate = a.EditDate,
                                      EncryptedId = a.EncryptedId
                                  }).FirstOrDefaultAsync();
            if (sprQuery != null)
            {
				DriverSpr spr = new DriverSpr()
				{
					Id = sprQuery.Id,
					Tn = sprQuery.Tn,
					FirstName = sprQuery.FirstName,
					LastName = sprQuery.LastName,
					CreatedByUser = sprQuery.CreatedByUser,
					CreateDate = sprQuery.CreateDate,
					EditedByUser = sprQuery.EditedByUser,
					EditDate = sprQuery.EditDate,
					EncryptedId = sprQuery.EncryptedId
				};

				return spr;
			}
            return new DriverSpr();
            
        }

        public async Task<bool> FindItem(int id)
        {
            return await context.DriverSpr.AnyAsync(e => e.Id == id);
        }

        public async Task<DriverSpr> Add(DriverSpr spr)
        {
            context.DriverSpr.Add(spr);
            await context.SaveChangesAsync();
            return spr;
        }

        public async Task Delete(int id)
        {
            DriverSpr? spr = await context.DriverSpr.FirstOrDefaultAsync(m => m.Id == id);
            if (spr != null)
            {
                context.DriverSpr.Remove(spr);
                await context.SaveChangesAsync();
            }
        }


        public async Task<DriverSpr> Update(DriverSpr sprChanges)
        {
            DriverSpr? spr = await context.DriverSpr.FirstOrDefaultAsync(m => m.Id == sprChanges.Id);
            if (spr != null)
            {
                spr.Tn = sprChanges.Tn;
                spr.FirstName = sprChanges.FirstName;
                spr.LastName = sprChanges.LastName;
                spr.EditedByUser = sprChanges.EditedByUser;
                spr.EditDate = sprChanges.EditDate;

                var sprUpdate = context.DriverSpr.Attach(spr);
                sprUpdate.State = EntityState.Modified;
                context.SaveChanges();
                return spr;
            }
            return sprChanges;
        }

        public async Task<int> GetLastId()
        {
			var id = await context.DriverSpr.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
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
			var cd = await context.DriverSpr.OrderByDescending(x => x.Tn).FirstOrDefaultAsync();
			if (cd != null)
            {
                try
                {
                    int result = Convert.ToInt32(cd.Tn);
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
