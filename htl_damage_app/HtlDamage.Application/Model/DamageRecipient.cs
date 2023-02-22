using System;

namespace HtlDamage.Application.Model
{
    public class DamageRecipient
    {
        public DamageRecipient(string email, DamageCategory damageCategory)
        {
            Email = email;
            DamageCategory = damageCategory;
            DamageCategoryId = damageCategory.Id;
        }

#pragma warning disable CS8618
        protected DamageRecipient() { }
#pragma warning restore CS8618

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public int DamageCategoryId { get; set; }
        public DamageCategory DamageCategory { get; set; }
    }
}