using PruebaTecnicaF2X.UseCase.Consultas;

namespace PruebaTecnicaF2X.BackendService.Extensiones
{
    public static class ServiceExtension
    {

        public static IServiceCollection RegistrarServicio(this IServiceCollection services)
        {
            #region UseCase

            services.AddScoped<IConsultaUseCase, ConsultaUseCase>();
            #endregion

            return services;
        }
    }
}
