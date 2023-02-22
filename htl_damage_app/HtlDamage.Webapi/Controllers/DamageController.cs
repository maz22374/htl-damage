using HtlDamage.Application.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAllDamages()
        {
            var damages = await _db.Damages.OrderBy(c => c.Name).ToListAsync();
            return Ok(damages);
        }
    }
}
