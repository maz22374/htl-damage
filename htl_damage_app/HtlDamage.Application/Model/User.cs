using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Model
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        public User(string firstName, string lastName, string userName, string? schoolClass)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            SchoolClass = schoolClass;
        }

#pragma warning disable CS8618 
        protected User() { }
#pragma warning restore CS8618

        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string? SchoolClass { get; set; }
        public List<Damage> Damages { get; } = new();
    }
}
