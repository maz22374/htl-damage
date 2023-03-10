using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    public class Damage
    {
        public Damage(string name, string imageUrl, Room room, DateTime created, 
            DateTime lastSeen, Lesson lesson, DamageCategory damageCategory)
        {
            Name = name;
            ImageUrl = imageUrl;
            Room = room;
            Created = created;
            LastSeen = lastSeen;
            Lesson = lesson;
            DamageCategory = damageCategory;
        }

#pragma warning disable CS8618
        protected Damage() { }
#pragma warning restore CS8618 

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public Room Room { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastSeen { get; set; }
        public Lesson Lesson { get; set; }
        public DamageCategory DamageCategory { get; set; }
        public List<DamageReport> DamageReports { get; } = new();
    }
}
