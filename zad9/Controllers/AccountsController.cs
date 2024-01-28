using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
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
    
    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {
        var account = _context.Account
            .Where(a => a.Login == loginRequest.Login)
            .FirstOrDefault();
        
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
            issuer: "http://localhost:5001",
            audience: "http://localhost:5001",
            claims: accountClaims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );
        
        var refreshToken = Guid.NewGuid().ToString();
        account.refreshToken = refreshToken;
        account.refreshTokenExp = DateTime.Now.AddDays(1);
        _context.SaveChanges();
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken
        });
    }
    
    
}

