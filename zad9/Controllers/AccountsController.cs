using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using zad8.Models;
using zad8.Repo;

namespace zad8.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    
    private readonly ILogger<DoctorsController> _logger;
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;
    
    public AccountsController(ILogger<DoctorsController> logger, DatabaseContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(AddUserDTO addUserDto)
    {
        var salt = Guid.NewGuid().ToString();
        var password = addUserDto.Password;
        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        
        var account = new Account
        {
            Login = addUserDto.Login,
            Password = passwordHash, //hasło posolone
            Salt = salt
        };
        await _context.Account.AddAsync(account);
        await _context.SaveChangesAsync();
        return Ok("Dodano konto");
    }
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var account = await _context.Account
            .Where(a => a.Login == loginRequest.Login)
            .FirstOrDefaultAsync();
        
        string passwordHash = account.Password;
        
        string crunchPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: loginRequest.Password,
            salt: Encoding.UTF8.GetBytes(account.Salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        
        //walidacja hasła
        if (passwordHash != crunchPassword)
        {
        return Unauthorized("Niepoprawne hasło");
        }
        
        Claim[] accountClaims = new[]
        {
            new Claim(ClaimTypes.Name, account.Login),
        };
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "http://localhost:5148",
            audience: "http://localhost:5148",
            claims: accountClaims,
            expires: DateTime.Now.AddMinutes(20),
            signingCredentials: credentials
        );
        
        var refreshToken = Guid.NewGuid().ToString();
        account.refreshToken = refreshToken;
        account.refreshTokenExp = DateTime.Now.AddDays(1);
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenDTO tokenDto)
    {
        var principal = getPrincipal(tokenDto.Token);
        if (principal == null || principal.Identity == null)
        {
            return Unauthorized();
        }
        var account = await _context.Account.FirstOrDefaultAsync(u => u.Login == principal.Identity.Name);
        
        if (account == null || account.refreshToken != tokenDto.RefreshToken || account.refreshTokenExp < DateTime.Now)
        {
            return Unauthorized();
        }
        
        Claim[] userClaims = new[]
        {
            new Claim(ClaimTypes.Name, account.Login),
        };
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "http://localhost:5148",
            audience: "http://localhost:5148",
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(20),
            signingCredentials: credentials
        );
        
        var refreshTokenNew = Guid.NewGuid().ToString();
        account.refreshToken = refreshTokenNew;
        account.refreshTokenExp = DateTime.Now.AddDays(7);
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = refreshTokenNew
        });
    }
    
    private ClaimsPrincipal? getPrincipal(string token)
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"])),
            ValidateLifetime = true
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
    
}

