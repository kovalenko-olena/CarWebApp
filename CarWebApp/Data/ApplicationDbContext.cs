using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarWebApp.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CarWebApp.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<FuelSpr> FuelSpr { get; set; }
		public DbSet<DriverSpr> DriverSpr { get; set; }
		public DbSet<BrandSpr> BrandSpr { get; set; }
		public DbSet<ModelSpr> ModelSpr { get; set; }
		public DbSet<VehicleSpr> VehicleSpr { get; set; }
		public DbSet<WayBill> WayBill { get; set; }

		/*
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Seed();
		}*/
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<VehicleSpr>().Property(p => p.Norm).HasPrecision(5, 2);
			modelBuilder.Entity<WayBill>().Property(p => p.FuelBalOut).HasPrecision(6, 2);
			modelBuilder.Entity<WayBill>().Property(p => p.FuelBalIn).HasPrecision(6, 2);
			modelBuilder.Entity<WayBill>().Property(p => p.FuelFillUp).HasPrecision(6, 2);
			modelBuilder.Entity<WayBill>().Property(p => p.FuelConsumNorm).HasPrecision(6, 2);
			modelBuilder.Entity<WayBill>().Property(p => p.FuelConsumFact).HasPrecision(6, 2);

			modelBuilder.Seed();
			foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
				.SelectMany(e => e.GetForeignKeys()))
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}