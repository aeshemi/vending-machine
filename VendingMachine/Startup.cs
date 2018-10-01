using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using VendingMachine.Repositories;
using VendingMachine.Services;

namespace VendingMachine
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<RepositoryDbContext>(opt => opt.UseInMemoryDatabase("VendingMachine"));

			services.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				.AddMvcOptions(options => options.OutputFormatters.Remove(options.OutputFormatters.OfType<StringOutputFormatter>().First()))
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Converters.Add(new StringEnumConverter());
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					options.SerializerSettings.Formatting = Formatting.Indented;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				});

			services.AddScoped<ICoinRepository, CoinRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();

			services.AddScoped<ICoinsService, CoinsService>();
			services.AddScoped<IProductsService, ProductsService>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Virtual Vending Machine API", Version = "v1" });
				c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "VendingMachine.xml"));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			SeedData.Initialize(serviceProvider.GetRequiredService<RepositoryDbContext>());

			app.UseCors(builder =>
				builder
					.AllowAnyOrigin()
					.AllowAnyHeader()
					.WithMethods("GET", "POST")
			);

			app.UseMvc();

			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.RoutePrefix = "api/docs";
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Virtual Vending Machine API");
			});
		}
	}
}
