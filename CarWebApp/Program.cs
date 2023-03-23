using CarWebApp.Data;
using CarWebApp.Models;
using CarWebApp.Repository;
using CarWebApp.Security;
using CarWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System.Drawing.Text;
using System.Security.Claims;
using FastReport;
using FastReport.Utils;
using FastReport.Data;



namespace CarWebApp
{
	public class Program
	{


		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);
			builder.WebHost.ConfigureLogging((context, logging) =>
			{
				logging.AddConfiguration(context.Configuration.GetSection("Loggin"));
				logging.AddConsole();
				logging.AddDebug();
				logging.AddEventSourceLogger();
				logging.AddNLog();
			});


			builder.Configuration.AddUserSecrets("aspnet-CarWebApp-0d6560fa-96bc-45eb-b010-f34c40898ce1");

			// Add services to the container.
			var connectionString = builder.Configuration
				.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			builder.Services.AddIdentity<Models.ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 5;
				options.Password.RequiredUniqueChars = 3;
				options.SignIn.RequireConfirmedEmail = true;
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
			})
							.AddEntityFrameworkStores<ApplicationDbContext>()
							.AddDefaultTokenProviders();

			// changes token lifespan of all token types to 5 hours
			builder.Services.Configure<DataProtectionTokenProviderOptions>(
				options => options.TokenLifespan = TimeSpan.FromHours(5));


			builder.Services.AddControllersWithViews(config =>
			{
				var polisy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();
				config.Filters.Add(new AuthorizeFilter(polisy));
			});

			builder.Services.AddAuthentication().AddGoogle(options =>
				{
					options.ClientId = builder.Configuration.GetValue<string>("GoogleClientId");
					options.ClientSecret = builder.Configuration.GetValue<string>("GoogleClientSecret");
					//options.CallbackPath = "/Fuel";
				});
			builder.Services.AddAuthentication().AddFacebook(options =>
			{
				options.ClientId = builder.Configuration.GetValue<string>("FacebookClientId");
				options.ClientSecret = builder.Configuration.GetValue<string>("FacebookClientSecret");
				//options.CallbackPath = "/Fuel";
			});


			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
			});

			// who have Delete Role can delete roles 
			builder.Services.AddAuthorization(options =>
			{

				// Claim policy
				options.AddPolicy("DeleteRolePolicy",
					policy => policy.RequireClaim("Delete Role", "true"));
				//options.AddPolicy("EditRolePolicy",
				//policy => policy.RequireClaim("Edit Role", "true"));

				/*options.AddPolicy("EditRolePolicy",
					policy => policy.RequireAssertion(context =>
					context.User.IsInRole("Admin") &&
					context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
					context.User.IsInRole("Super Admin")
					));
				*/
				options.AddPolicy("EditRolePolicy",
					policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

				options.AddPolicy("CreateRolePolicy",
					policy => policy.RequireClaim("Create Role", "true"));
				// Role policy
				options.AddPolicy("AdminRolePolicy",
					policy => policy.RequireRole("Admin"));

				// data access policy
				options.AddPolicy("Administrator",
					policy => policy.RequireClaim("Administrator", "true"));
				options.AddPolicy("Master",
				   policy => policy.RequireClaim("Master", "true"));
				options.AddPolicy("Operator",
				   policy => policy.RequireClaim("Operator", "true"));

				options.AddPolicy("AdministratorOrMaster",
					policy =>
						policy.RequireAssertion(context =>
						context.User.HasClaim(c =>
							(c.Type == "Administrator" && c.Value == "true" ||
							c.Type == "Master" && c.Value == "true"))));

				options.AddPolicy("AdministratorOrMasterOrOperator",
					policy =>
						policy.RequireAssertion(context =>
						context.User.HasClaim(c =>
							(c.Type == "Administrator" && c.Value == "true" ||
							c.Type == "Master" && c.Value == "true" ||
							c.Type == "Operator" && c.Value == "true"))));

			});
			//
			builder.Services.AddRazorPages();

			//builder.Services.AddSingleton<IFuelRepository,MockFuelRepository>();
			builder.Services.AddScoped<ISprRepository<FuelSpr>, FuelRepository>();
			builder.Services.AddScoped<ISprRepository<ModelSpr>, ModelRepository>();
			builder.Services.AddScoped<ISprRepository<BrandSpr>, BrandRepository>();
			builder.Services.AddScoped<ISprRepository<DriverSpr>, DriverRepository>();
			builder.Services.AddScoped<ISprRepository<VehicleSpr>, VehicleRepository>();
			builder.Services.AddScoped<IWayBillRepository, WayBillRepository>();
			builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
			builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
			builder.Services.AddSingleton<DataProtectionPurposeStrings>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				//app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
				//app.UseStatusCodePagesWithRedirects("/Error/{0}");
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
				app.UseExceptionHandler("/ErrorHandler");
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();
			FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
			app.UseFastReport();
			app.Run();
		}
		public class MsSqlDataConnection : FastReport.Data.DataConnectionBase
		{
			public override string QuoteIdentifier(string value, System.Data.Common.DbConnection connection)
			{
				return "\"" + value + "\"";
			}

			public override System.Type GetConnectionType()
			{
				return typeof(System.Data.SqlClient.SqlConnection);
			}

			public override System.Type GetParameterType()
			{
				return typeof(System.Data.SqlDbType);
			}

			public override System.Data.Common.DbDataAdapter GetAdapter(string selectCommand, System.Data.Common.DbConnection connection, FastReport.Data.CommandParameterCollection parameters)
			{
				System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(selectCommand, connection as System.Data.SqlClient.SqlConnection);
				foreach (FastReport.Data.CommandParameter p in parameters)
				{
					System.Data.SqlClient.SqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (System.Data.SqlDbType)p.DataType, p.Size);
					parameter.Value = p.Value;
				}
				return adapter;
			}
		}
	}
}