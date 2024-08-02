
using Microsoft.AspNetCore.Identity;
namespace MSAuth.Models;

public class ApplicationUser : IdentityUser
{
    public bool IsFirstLogin { get; set; } = true;
}
