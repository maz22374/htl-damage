using AutoMapper;
using Bogus.DataSets;
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

        public DamageService(DamageContext db, StorageClient storageClient)
        {
            _db = db;
            _storageClient = storageClient;
        }

        public IQueryable<Damage> Damages => _db.Damages.AsQueryable();

        public async Task<(bool success, string? message, Damage? damage)> AddDamage(NewDamageCmd damageCmd)
        {
            var guid = Guid.NewGuid().ToString();
            var filename = $"{DateTime.Now:yyyyMMdd}-{guid}.jpg";
            var result = await _storageClient.UploadFileToAzure("damagephotos", filename, Convert.FromBase64String(damageCmd.Attachment));

            var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Guid == damageCmd.RoomGuid);
            var lesson = await _db.Lessons.FirstOrDefaultAsync(l => l.Guid == damageCmd.LessonGuid);
            var damageCategory = await _db.DamageCategories.FirstOrDefaultAsync(d => d.Guid == damageCmd.DamageCategoryGuid);

            if (room is null) return (false, $"Room {damageCmd.RoomGuid} not found.", null);
            if (lesson is null) return (false, $"Lesson {damageCmd.LessonGuid} not found.", null);
            if (damageCategory is null) return (false, $"DamageCategory {damageCmd.DamageCategoryGuid} not found.", null);

            var damage = new Damage(
                name: damageCmd.Name,
                imageUrl: result.Link,
                room: room,
                created: DateTime.UtcNow,
                lastSeen: DateTime.UtcNow,
                lesson: lesson,
                damageCategory: damageCategory);

            await _db.Damages.AddAsync(damage);
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException e) { return (false, e.InnerException?.Message ?? e.Message, null); }

            return (true, string.Empty, damage);
        }
    }
}
