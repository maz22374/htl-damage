using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    [Index(nameof(RoomNumber), IsUnique = true)]
    public class Room
    {
        public Room(RoomCategory roomCategory, string floor, string building, string? roomNumber = null)
        {
            RoomNumber = roomNumber;
            RoomCategory = roomCategory;
            Floor = floor;
            Building = building;
        }

#pragma warning disable CS8618 
        protected Room() { }
#pragma warning restore CS8618 

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string? RoomNumber { get; set; }
        public RoomCategory RoomCategory { get; set; }
        public string Floor { get; set; }
        public string Building { get; set; }
        public List<Damage> Damages { get; } = new();
    }
}
