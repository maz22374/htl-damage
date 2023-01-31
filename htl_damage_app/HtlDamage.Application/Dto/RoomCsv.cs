using CsvHelper.Configuration;
using HtlDamage.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public class RoomCsv: ClassMap<Room>
    {
        public RoomCsv()
        {
            Map(p => p.RoomNumber).Name("RoomNumber");
            Map(p => p.RoomCategory).Name("RoomCategory");
            Map(p => p.Floor).Name("Floor");
            Map(p => p.Building).Name("Building");
        }
    }
}
