using PruebaTecnicaF2X.BackendService.AutoMapper;
using PruebaTecnicaF2X.BackendService.Extensiones;
using PruebaTecnicaF2X.ObjectsUtils;
using PruebaTecnicaF2X.BackendService.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<ConfiguratorAppSettings>(builder.Configuration.GetRequiredSection(nameof(ConfiguratorAppSettings)));
ConfiguratorAppSettings configuratorAppSettings =builder.Configuration.GetSection(nameof(ConfiguratorAppSettings)).Get<ConfiguratorAppSettings>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .RegistrarServicio()
    .RegisterAutoMapper();
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
