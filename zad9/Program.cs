using Microsoft.EntityFrameworkCore;
using zad8.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//konfiguracja bazy danych
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async ( context, next ) =>
{
    try
    {
       await next();
    }
    catch (Exception e)
    {
        // zapisywanie do pliku logs.txt
        File.AppendAllText("logs.txt", e.ToString() + "\n");
        throw;
    }
});

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();