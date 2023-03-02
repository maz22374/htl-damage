using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Dto
{
    public record CredentialsDto(
        string Username,
        string Password)
    {
    }
}
