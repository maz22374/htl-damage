using HtlDamage.Application.Infrastructure;
using HtlDamage.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public record NewDamageCmd(
        Guid Guid,
        string Name,
        string Attachment,
        Guid RoomGuid,
        Guid LessonGuid,
        Guid DamageCategoryGuid) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var db = validationContext.GetRequiredService<DamageContext>();

            if (!db.Rooms.Any(r => r.Guid == RoomGuid))
            {
                yield return new ValidationResult("Room does not exist", new[] { nameof(RoomGuid) });
            }
            if (!db.Lessons.Any(l => l.Guid == LessonGuid))
            {
                yield return new ValidationResult("Lesson does not exist", new[] { nameof(LessonGuid) });
            }
            if (!db.DamageCategories.Any(d => d.Guid == DamageCategoryGuid))
            {
                yield return new ValidationResult("DamageCategory does not exist", new[] { nameof(DamageCategoryGuid) });
            }
        }
    }
}
