using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Helper.JWT;

public static class JwtHelper
{

    private static byte[] key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("key"));
    public static string CreateToken(string username)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,username));
        var tokenDescription = new SecurityTokenDescriptor()
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddHours(0.5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
        };
        var tokeHandler = new JwtSecurityTokenHandler();
        var createdToken = tokeHandler.CreateToken(tokenDescription);
        return tokeHandler.WriteToken(createdToken);
    }
    
    
    
}