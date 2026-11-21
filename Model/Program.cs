using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Model.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); 

builder.Services.AddDbContext<MapDbContext>(o =>
    o.UseSqlServer(
        builder.Configuration.GetConnectionString("MapDb")));

var app = builder.Build(); app.MapGet("/ping", (MapDbContext db) => db.Users.Count());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

var root = Path.Combine(app.Environment.ContentRootPath, "upload");
Directory.CreateDirectory(root);
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(root),
    RequestPath = "/upload" });

app.MapControllers();

app.Run();
