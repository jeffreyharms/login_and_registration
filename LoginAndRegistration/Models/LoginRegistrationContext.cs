#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace LoginAndRegistration.Models;

public class LoginRegistrationContext : DbContext
{
    public LoginRegistrationContext(DbContextOptions options) : base(options) {  }
    public DbSet<User> users { get; set; }
}