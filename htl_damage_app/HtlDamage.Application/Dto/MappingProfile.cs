using AutoMapper;
using HtlDamage.Application.Model;

namespace HtlDamage.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Damage, DamageDto>();
            CreateMap<DamageDto, Damage>();
        }
    }
}
