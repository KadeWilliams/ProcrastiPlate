using Microsoft.OpenApi.Models;
using ProcrastiPlate.Api.Repositories;
using ProcrastiPlate.Api.Repositories.Interfaces;
using ProcrastiPlate.Api.Services;
using ProcrastiPlate.Api.Services.Interface;
using ProcrastiPlate.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Connection string 'Database' not found.");
builder.Services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Procrasti Plate Api", Version = "v0" });
    //c.OperationFilter<CustomHeaderOperationFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5173").AllowAnyHeader().AllowAnyMethod();
        policy.WithOrigins("https://localhost:5002").AllowAnyHeader().AllowAnyMethod();
        policy.WithOrigins("http://localhost:5003").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddTransient<IProcrastiPlateService, ProcrastiPlateService>();
builder.Services.AddTransient<IProcrastiPlateRepository, ProcrastiPlateRepository>();
builder.Services.AddTransient<IRecipeRepository, RecipeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
