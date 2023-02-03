using System;
using System.ComponentModel.DataAnnotations;

namespace HtlDamage.Application.Model
{
    public class DamageReport
    {
        public DamageReport(Damage damage, User user, DateTime created)
        {
            Damage = damage;
            DamageId = damage.Id;
            User = user;
            UserId = user.Id;
            Created = created;
        }

#pragma warning disable CS8618 
        protected DamageReport() { }
#pragma warning restore CS8618 

        public int DamageId { get; private set; }
        public Damage Damage { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }
        public DateTime Created { get; set; }
    }
}
