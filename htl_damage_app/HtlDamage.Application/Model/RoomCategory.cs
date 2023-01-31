using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    [Index(nameof(Name), IsUnique = true)]
    public class RoomCategory
    {
        public RoomCategory(string name)
        {
            Name = name;
        }

#pragma warning disable CS8618 
        protected RoomCategory() { }
#pragma warning restore CS8618 

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
