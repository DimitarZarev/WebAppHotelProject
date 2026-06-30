using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebAppHotelFinal.Data.Domain
{
    public class AppRole:IdentityRole
    {
        public DateTime OnCreated { get; set; } = DateTime.UtcNow;
    }
}
