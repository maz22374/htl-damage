using AutoMapper;
using HtlDamage.Application.Dto;
using HtlDamage.Application.Infrastructure;
using HtlDamage.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HtlDamage.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamageController : ControllerBase
    {
        private readonly DamageContext _db;
        private readonly IMapper _mapper;

        public DamageController(DamageContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDamages()
        {
            var damages = await _db.Damages
                .OrderBy(d => d.Room.RoomNumber)
                .Select(d => new
                {
                    d.Guid,
                    d.Name,
                    Room = d.Room.RoomNumber,
                    d.Created,
                    d.LastSeen,
                    LessonNumber = d.Lesson.LessonNumber,
                    DamageCategory = d.DamageCategory.Name,
                    DamageReports = d.DamageReports.Select(dr => new
                    {
                        FirstName = dr.User.FirstName,
                        LastName = dr.User.LastName,
                        UserName = dr.User.UserName
                    })
                })
                .ToListAsync();

            return Ok(damages);
        }
    }
}
