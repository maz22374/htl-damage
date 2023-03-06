using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public record DamageDto(
        Guid Guid,
        string Name,
        string ImageUrl,
        Guid RoomGuid,
        Guid LessonGuid,
        Guid DamageCategoryGuid)
    {
    }
}
