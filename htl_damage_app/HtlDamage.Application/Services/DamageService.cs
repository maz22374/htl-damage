using AutoMapper;
using Bogus.DataSets;
using HtlDamage.Application.Infrastructure;
using HtlDamage.Application.Model;
using System.Linq;

namespace HtlDamage.Webapi.Services
{
    public class DamageService
    {
        private readonly DamageContext _db;
        private readonly IMapper _mapper;

        public DamageService(DamageContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IQueryable<Damage> Damages => _db.Damages.AsQueryable();
    }
}
