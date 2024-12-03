using Bookstore.Data;
using Bookstore.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Bookstore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddScoped<GenreService>();
			//builder.Services.AddScoped<BookService>();
			builder.Services.AddScoped<SeedingService>();
			//builder.Services.AddScoped<SaleService>();
			//builder.Services.AddScoped<SellerService>();

			builder.Services.AddDbContext<BookstoreContext>(options =>
			{
				options.UseMySql(
					builder
						.Configuration
						.GetConnectionString("BookstoreContext"),
					ServerVersion
						.AutoDetect(
							builder
								.Configuration
								.GetConnectionString("BookstoreContext")
						)
				);
			});

			var app = builder.Build();

			var ptBR = new CultureInfo("pt-BR");

			var localizationOption = new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture(ptBR),
				SupportedCultures = new List<CultureInfo> { ptBR },
				SupportedUICultures = new List<CultureInfo> { ptBR }
			};

			app.UseRequestLocalization(localizationOption);


			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}
			else
			{
				app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedingService>().Seed();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}