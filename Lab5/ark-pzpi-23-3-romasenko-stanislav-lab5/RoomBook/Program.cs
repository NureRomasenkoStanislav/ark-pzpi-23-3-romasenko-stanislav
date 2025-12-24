using Microsoft.EntityFrameworkCore;
using RoomBook.Core.Interfaces;
using RoomBook.Infrastructure.Repositories;
using RoomBook.API.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens; 
using System.Text;
using RoomBook.API.Hubs;
using RoomBook.Core.Services;
using Microsoft.AspNetCore.SignalR;
using RoomBook.Infrastructure.Data;
using RoomBook.Core.Entities;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RoomBookConnection");
builder.Services.AddDbContext<RoomBookDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSignalR();



var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введіть JWT токен у форматі: Bearer {ваш_токен}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIotService, IotService>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>(); 

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IIotService, IotService>();

builder.Services.AddSingleton<ISystemStateService, SystemStateService>();

builder.Services.AddTransient<IHubContext<Hub>>(provider =>
    (IHubContext<Hub>)provider.GetRequiredService<IHubContext<RoomHub>>());

var app = builder.Build();
app.MapHub<RoomHub>("/roomHub");
app.MapHub<IotHub>("/iot-hub");

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoomBook API V1");
    c.RoutePrefix = "swagger"; 
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();  

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var service = scope.ServiceProvider;
        var context = services.GetRequiredService<RoomBookDbContext>();

        context.Database.EnsureCreated(); 

        if (!context.Users.Any()) 
        {
            context.Users.Add(new User
            {
                Email = "admin@roombook.com",
                PasswordHash = "AdminPass123",
                FirstName = "Admin",
                LastName = "System",
                Role = "Administrator",
                IsActive = true 
            });
            context.SaveChanges();
            Console.WriteLine("[SEED] Адміністратор успішно доданий.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при ініціалізації бази даних.");
    }
}

app.Run();