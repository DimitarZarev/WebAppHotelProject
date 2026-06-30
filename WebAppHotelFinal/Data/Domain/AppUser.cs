using Microsoft.AspNetCore.Identity;
using WebAppHotelFinal.Models;

namespace WebAppHotelFinal.Data.Domain
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // 🔗 Navigation property to Client
        public Client Client { get; set; }
    }
}
