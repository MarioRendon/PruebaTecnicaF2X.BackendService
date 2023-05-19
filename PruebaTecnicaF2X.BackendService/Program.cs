using PruebaTecnicaF2X.BackendService.AutoMapper;
using PruebaTecnicaF2X.BackendService.Extensiones;
using PruebaTecnicaF2X.ObjectsUtils;
using PruebaTecnicaF2X.BackendService.AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<ConfiguratorAppSettings>(builder.Configuration.GetRequiredSection(nameof(ConfiguratorAppSettings)));
ConfiguratorAppSettings configuratorAppSettings =builder.Configuration.GetSection(nameof(ConfiguratorAppSettings)).Get<ConfiguratorAppSettings>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200");
                      });
});
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .RegisterAutoMapper()
    .RegistrarServicio()
    
    .RegisterSQL(configuratorAppSettings.ConexionSql);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var urlAceptadas = builder.Configuration
                   .GetSection("AllowedHosts").Value.Split(",");
app.UseCors(builder => builder.WithOrigins(urlAceptadas)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      );
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
