using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Application.Dtos.User;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace RestaurantFoodPlanningSystem.Services;

public class TokenService
{
    private readonly IConfiguration config;

    private JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

    private ILogger<TokenService> _logger;

    public TokenService(IConfiguration        config,
                        ILogger<TokenService> logger)
    {
        this.config = config;
        _logger     = logger;
    }

    public String CreateToken(UserResultDto user)
    {
        List<Claim> claims = new List<Claim>() {
                                                   new Claim(
                                                             ClaimTypes.Name,
                                                             user.UserName),
                                                   new Claim(
                                                             ClaimTypes.NameIdentifier,
                                                             user.Id.ToString()),
                                                   new Claim(
                                                             JwtRegisteredClaimNames.Sub,
                                                             user.UserName),
                                                   new Claim(
                                                             JwtRegisteredClaimNames.Jti,
                                                             Guid
                                                                 .NewGuid()
                                                                 .ToString()),
                                                   new Claim(
                                                             JwtRegisteredClaimNames.Iss,
                                                             config["Issuer"])
                                               };


        foreach (string role in user.Role)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (string aud in config["Audience"]
                     .Split(","))
        {
            claims.Add(
                       new Claim(
                                 JwtRegisteredClaimNames.Aud,
                                 aud));
        }

        //Console.WriteLine(config["Token"]);
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TOKEN")));

        SigningCredentials sc = new SigningCredentials(
                                                       key,
                                                       SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor() {
                                                                               Issuer             = config["Issuer"],
                                                                               Subject            = new ClaimsIdentity(claims),
                                                                               Expires            = DateTime.UtcNow.AddDays(7),
                                                                               SigningCredentials = sc
                                                                           };

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(String token)
    {
        try
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters() {
                                                                                                 ValidateIssuerSigningKey = true,
                                                                                                 ValidateLifetime         = true,
                                                                                                 ValidateIssuer           = true,
                                                                                                 ValidIssuer              = config["Issuer"],
                                                                                                 ValidateAudience         = true,
                                                                                                 ValidAudiences = config["Audience"]
                                                                                                     ?.Split(","),
                                                                                                 IssuerSigningKey =
                                                                                                     new SymmetricSecurityKey(
                                                                                                                              Encoding.UTF8
                                                                                                                                      .GetBytes(
                                                                                                                                                Environment.GetEnvironmentVariable("TOKEN")
                                                                                                                                               )),
                                                                                                 RequireExpirationTime = true,
                                                                                                 ClockSkew             = TimeSpan.FromHours(3)
                                                                                             };


            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(
                                                                         token,
                                                                         validationParameters,
                                                                         out var validatedToken);

            if (claimsPrincipal != null)
            {
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            _logger.LogDebug(
                             e,
                             e.Message);

            return false;
        }
    }
}