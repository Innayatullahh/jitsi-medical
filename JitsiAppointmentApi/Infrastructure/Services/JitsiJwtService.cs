using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JitsiAppointmentApi.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JitsiAppointmentApi.Infrastructure.Services
{
    public class JitsiJwtService
    {
        private readonly JitsiOptions _options;

        public JitsiJwtService(IOptions<JitsiOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(string room, string userName, string userEmail)
        {
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddMinutes(_options.JwtExpiryMinutes).ToUnixTimeSeconds();
            var nbf = now.AddMinutes(-1).ToUnixTimeSeconds(); // 1 minute in the past for clock skew

            var claims = new List<Claim>
            {
                new Claim("aud", _options.JwtAudience),
                new Claim("iss", _options.JwtIssuer),
                new Claim("sub", _options.ServerUrl.Replace("https://", "").Replace("http://", "")),
                new Claim("room", room),
                new Claim(JwtRegisteredClaimNames.Exp, exp.ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Nbf, nbf.ToString(), ClaimValueTypes.Integer64),
                new Claim("context", $@"{{""user"":{{""name"":""{userName}"",""email"":""{userEmail}""}}}}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 