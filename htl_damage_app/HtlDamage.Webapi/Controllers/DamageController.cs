using HtlDamage.Application.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HtlDamage.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamageController : ControllerBase
    {
        private readonly DamageContext _db;

        public DamageController(DamageContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllCompanies()
        {
            var companies = _db.Companies.OrderBy(c => c.Name).ToList();
            return Ok(companies);
        }
    }
}
