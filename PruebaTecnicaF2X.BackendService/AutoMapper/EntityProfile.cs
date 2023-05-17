using AutoMapper;
using PruebaTecnicaF2X.Model.RecaudosAcumulado;
using PruebaTecnicaF2X.SqlServer.Recaudo;

namespace PruebaTecnicaF2X.BackendService.AutoMapper;

public class EntityProfile:Profile
{
    public EntityProfile() {
        CreateMap<Recaudos, RecaudosEntity>().ReverseMap();
    }
}
