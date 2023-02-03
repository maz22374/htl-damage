using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    [Index(nameof(Name), IsUnique = true)]
    public class DamageCategory
    {
        public DamageCategory(string name)
        {
            Name = name;
        }

#pragma warning disable CS8618
        protected DamageCategory() { }
#pragma warning restore CS8618 

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public List<DamageRecipient> DamageRecipients { get; } = new();
    }
}
