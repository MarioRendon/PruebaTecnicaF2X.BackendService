using PruebaTecnicaF2X.BackendService.AutoMapper;
using PruebaTecnicaF2X.UseCase.Consultas;
using AutoMapper.Data;
using PruebaTecnicaF2X.UseCase.ProcesarInformacion;
using PruebaTecnicaF2X.Http.Api;

namespace PruebaTecnicaF2X.BackendService.Extensiones
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(cfg => { cfg.AddDataReaderMapping(); }, typeof(EntityProfile));

        public static IServiceCollection RegistrarServicio(this IServiceCollection services)
        {
            #region UseCase

            services.AddScoped<IConsultaUseCase, ConsultaUseCase>();
            services.AddScoped<IProcesarUseCase, ProcesarUseCase>();
            services.AddScoped<IConexionApiAdapter, ConexionApiAdapter>();
            #endregion
            return services;
        }
    }
}
