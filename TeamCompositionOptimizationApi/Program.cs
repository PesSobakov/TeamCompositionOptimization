
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamCompositionOptimizationApi.Models.Database;

namespace TeamCompositionOptimizationApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //CORS
            string CORSOpenPolicy = "OpenCORSPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                  name: CORSOpenPolicy,
                  builder =>
                  {
                      builder.WithOrigins("http://localhost:4200", "https://victorious-beach-01a30820f.4.azurestaticapps.net", "*").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                  });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Events.OnRedirectToAccessDenied =
                    options.Events.OnRedirectToLogin = c =>
                    {
                        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.FromResult<object>(null!);
                    };
                });
            builder.Services.AddDbContext<DatabaseContext>(
                options =>
                {
                    //options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"), options2 =>
                    options.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"), options2 =>
                    {
                        options2.CommandTimeout(300);
                    });
                });
        

        var app = builder.Build();

        app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors(CORSOpenPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }



    app.UseHttpsRedirection();




            app.MapControllers();



            app.Run();
        }
    }
}
