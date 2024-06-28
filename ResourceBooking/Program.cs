using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResourceBooking;
using ResourceBooking.Data;
using ResourceBooking.Helpers;
using ResourceBooking.Interfaces;
using ResourceBooking.Repositories;
using ResourceBooking.Services; // Make sure to include this using directive
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddTransient<Seed>();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });

        // Register repositories
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();

        // Register TokenService
        builder.Services.AddSingleton<TokenService>(); // Register TokenService

        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfiles));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Creating the DbContext to connect with the database
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        //Configure the JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

        var app = builder.Build();

        // Seed the database if the argument is provided
        if (args.Length == 1 && args[0].ToLower() == "seeddata")
        {
            SeedData(app);
        }

        // Seed the database.
        void SeedData(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var seed = scope.ServiceProvider.GetRequiredService<Seed>();
                seed.SeedDataContext();
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
