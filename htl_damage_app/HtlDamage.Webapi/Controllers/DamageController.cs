using AutoMapper;
using Docker.DotNet.Models;
using HtlDamage.Application.Cmd;
using HtlDamage.Application.Infrastructure;
using HtlDamage.Application.Model;
using HtlDamage.Webapi.Services;
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
        private readonly DamageService _damageService;

        public DamageController(DamageService damageService)
        {
            _damageService = damageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDamages()
        {
            var damages = await _damageService.Damages
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

        [HttpPost]
        public async Task<IActionResult> AddDamage(DamageCmd damageCmd)
        {
            var (success, message, damage) = await _damageService.AddDamage(damageCmd);
            if(!success) return BadRequest(message);
            return Ok(damage);
        }
    }
}
