using Database.Database.Redis;
using Microsoft.EntityFrameworkCore;
using Database.Database.MySQL.Contexto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MySQL DBContexto registration
builder.Services.AddDbContext<DBContexto>(options =>
    options
    .UseLazyLoadingProxies()
    .UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    )
);

// Redis connection registration
builder.Services.AddSingleton(x => new conexionRedis(
    //Modificar la cadena de conexion en appsettings.json
    builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379"
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
