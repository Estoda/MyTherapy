using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyTherapy.API.Filters;
using MyTherapy.API.Middlewares;
using MyTherapy.Application.Interfaces;
using MyTherapy.Infrastructure.Persistence;
using MyTherapy.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();


builder.Services.AddHttpClient<IPaymobService, PaymobService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<VerifiedTherapistFilter>();
});

builder.Services.AddScoped<VerifiedTherapistFilter>();

builder.Services.AddScoped<IProfileService, ProfileService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.SeedAdmin(db);
}


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();


app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();