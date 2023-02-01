using Bogus;
using CsvHelper.Configuration;
using HtlDamage.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public record UserDto(
        string FirstName,
        string LastName, 
        string UserName,
        string? SchoolClass)
    { 
    }
}
