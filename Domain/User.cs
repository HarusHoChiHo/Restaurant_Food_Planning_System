using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser<int>
{
    public override int     Id           { get; set; }
    public override string? UserName     { get; set; }
    public override string? PasswordHash { get; set; }


    public User()
        : base()
    {
        
    }
    
    public User(string userName)
        : base(userName)
    {
        
    }
    
    public User(string userName, string passwordHash)
        : base(userName)
    {
        PasswordHash = passwordHash;
    }
}