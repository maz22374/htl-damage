using CsvHelper.Configuration;
using HtlDamage.Application.Model;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public record RoomDto(
        string RoomNumber,
        string RoomCategory,
        string Floor,
        string Building)
    {
    }
}
