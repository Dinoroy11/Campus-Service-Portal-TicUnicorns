using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;



using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Database
// =========================================================

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// =========================================================
// Controllers
// =========================================================
builder.Services.AddScoped<ICanteenRepository, CanteenRepository>();
builder.Services.AddScoped<ICanteenService, CanteenService>();


builder.Services.AddControllers();


// =========================================================
// Swagger / OpenAPI
// =========================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// =========================================================
// Build Application
// =========================================================

var app = builder.Build();


// =========================================================
// HTTP Request Pipeline
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();