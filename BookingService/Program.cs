using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BookingService.Data;
using BookingService.Services;

var builder = WebApplication.CreateBuilder(args);

// SQL Server DB-Kontext
builder.Services.AddDbContext<BookingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Business-Logik-Service
builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService", Version = "v1" });
});

var app = builder.Build();

// Automatische Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();