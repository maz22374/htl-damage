using AutoMapper;
using HtlDamage.Application.Model;

namespace HtlDamage.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Dto
            CreateMap<Damage, DamageDto>();

            // Cmd
            CreateMap<DamageCmd, Damage>();
        }
    }
}
