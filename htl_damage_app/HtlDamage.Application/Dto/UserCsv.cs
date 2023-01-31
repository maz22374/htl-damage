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
    public class UserCsv: ClassMap<User>
    {
        public UserCsv()
        {
            Map(p => p.FirstName).Name("FirstName");
            Map(p => p.LastName).Name("LastName");
            Map(p => p.UserName).Name("UserName");
            Map(p => p.SchoolClass).Name("SchoolClass");
        }
    }
}
