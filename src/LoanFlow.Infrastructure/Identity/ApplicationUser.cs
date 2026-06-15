using LoanFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LoanFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public CustomerProfile? CustomerProfile { get; set; }
}

