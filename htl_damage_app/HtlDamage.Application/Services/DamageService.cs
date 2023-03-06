using AutoMapper;
using Bogus.DataSets;
using HtlDamage.Application.Cmd;
using HtlDamage.Application.Dto;
using HtlDamage.Application.Infrastructure;
using HtlDamage.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HtlDamage.Webapi.Services
{
    public class DamageService
    {
        private readonly DamageContext _db;
        private readonly StorageClient _storageClient;
        private readonly IMapper _mapper;

        public DamageService(DamageContext db, IMapper mapper, StorageClient storageClient)
        {
            _db = db;
            _mapper = mapper;
            _storageClient = storageClient;
        }

        public IQueryable<Damage> Damages => _db.Damages.AsQueryable();

        public async Task<(bool success, string? message, DamageDto? damage)> AddDamage(DamageCmd damageCmd)
        {
            var guid = Guid.NewGuid().ToString();
            var filename = $"{DateTime.Now:yyyyMMdd}-{guid}.jpg";
            var result = await _storageClient.UploadFileToAzure("damagephotos", filename, Convert.FromBase64String(damageCmd.Attachment));

            var damage = new Damage(
                name: damageCmd.Name,
                imageUrl: result.Link,
                room: await _db.Rooms.FirstAsync(r => r.Guid == damageCmd.RoomGuid),
                created: DateTime.UtcNow,
                lastSeen: DateTime.UtcNow,
                lesson: await _db.Lessons.FirstAsync(l => l.Guid == damageCmd.LessonGuid),
                damageCategory: await _db.DamageCategories.FirstAsync(d => d.Guid == damageCmd.DamageCategoryGuid));

            await _db.Damages.AddAsync(damage);
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException e) { return (false, e.InnerException?.Message ?? e.Message, null); }

            return (true, string.Empty, _mapper.Map<Damage, DamageDto>(damage));
        }
    }
}
