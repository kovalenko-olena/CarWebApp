using Microsoft.EntityFrameworkCore;

namespace CarWebApp.Models
{
	
	public static class ModelBuilderExtensions
	{
		
		public static void Seed(this ModelBuilder modelBuilder)
		{
			/*modelBuilder.Entity<FuelSpr>().HasData(
				new FuelSpr
				{
					Id= 10,
					Cd= 1,
					Name = "DT"
				},
				new FuelSpr
				{
					Id = 11,
					Cd = 2,
					Name = "DT"
				}
				);*/
		}
	}
}
