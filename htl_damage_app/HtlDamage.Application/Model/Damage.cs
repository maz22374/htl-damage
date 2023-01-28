using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    public class Damage
    {
        public Damage(string name)
        {
            Name = name;
        }

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
