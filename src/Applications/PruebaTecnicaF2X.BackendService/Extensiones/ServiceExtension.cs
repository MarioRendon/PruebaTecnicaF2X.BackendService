using PruebaTecnicaF2X.BackendService.AutoMapper;
using PruebaTecnicaF2X.UseCase.Consultas;
using AutoMapper.Data;
using PruebaTecnicaF2X.UseCase.ProcesarInformacion;
using PruebaTecnicaF2X.Http.Api;
using PruebaTecnicaF2X.Model.RecaudosAcumulado.Gateway;
using PruebaTecnicaF2X.SqlServer;

namespace PruebaTecnicaF2X.BackendService.Extensiones
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(cfg => { cfg.AddDataReaderMapping(); }, typeof(EntityProfile));

        public static IServiceCollection RegisterSQL(this IServiceCollection services, string dataConexion) =>
            services.AddSingleton<IDapperContext>(provider => new DapperContext(dataConexion));
        public static IServiceCollection RegistrarServicio(this IServiceCollection services)
        {


            
            services.AddScoped<IConexionApiAdapter, ConexionApiAdapter>();
            services.AddScoped<IRecaudosRepository, RecaudoAdapter>();
            #region UseCase

            services.AddScoped<IConsultaUseCase, ConsultaUseCase>();
            services.AddScoped<IProcesarUseCase, ProcesarUseCase>();
            #endregion
            return services;
        }
    }
}
