using Microsoft.OpenApi.Models;
using ProcrastiPlate.API.Repositories;
using ProcrastiPlate.API.Repositories.Interfaces;
using ProcrastiPlate.API.Services;
using ProcrastiPlate.API.Services.Interface;
using ProcrastiPlate.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new InvalidOperationException("Connection string 'Database' not found.");
builder.Services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Recipe Tracker API", Version = "v0" });
    //c.OperationFilter<CustomHeaderOperationFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:5173");
            policy.WithOrigins("https://localhost:5002");
            policy.WithOrigins("http://localhost:5003");
        }
    );
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddTransient<IProcrastiPlateService, ProcrastiPlateService>();
builder.Services.AddTransient<IProcrastiPlateRepository, ProcrastiPlateRepository>();

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