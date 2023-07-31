using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using StoreAPI.Extensions;
using StoreAPI.MiddleWares;
using infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Core.Entities.Identity;
using System.Data.Common;

namespace StoreAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerDocumetations();
            builder.Services.AddDbContext<StoreDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
                     options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis"), true);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService(typeof(StoreDbContext)) as StoreDbContext;
                    await context.Database.MigrateAsync();
                    await StoreDbContextSeed.SeedAsync(context, loggerFactory);
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var identity = services.GetRequiredService<ApplicationIdentityDbContext>();
                    await identity.Database.MigrateAsync();
                    await AddIdentityDbContextSeed.SeedUserAsync(userManager);

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex.Message);
                }
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleWare>();
            app.UseStaticFiles();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}