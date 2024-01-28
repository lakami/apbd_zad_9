using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using zad8.Models;
using zad8.Repo;

namespace zad8.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    
    private readonly ILogger<DoctorsController> _logger;
    private readonly DatabaseContext _context;
    
    public AccountsController(ILogger<DoctorsController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
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
    
    
    // [AllowAnonymous]
    // [HttpPost("login")]
    // public IActionResult Login(LoginRequest loginRequest)
    // {
    //     var account = _context.Account
    //         .Where(a => a.Login == loginRequest.Login)
    //         .FirstOrDefault();
    //     
    //     string passwordHash = account.Password;
    //     string crunchPassword = "";
    //     
    //     //walidacja hasła
    //     if (passwordHash != crunchPassword)
    //     {
    //     return Unauthorized("Niepoprawne hasło");
    //     }
    //     
    //     Claim[] accountClaims = new[]
    //     {
    //         new Claim(ClaimTypes.Name, "Roman"),
    //         new Claim(ClaimTypes.Role, "admin"),
    //         new Claim(ClaimTypes.Role, "user")
    //     };
    //     
    //     SymerticSecurityKey key = new SymerticSecurityKey(Encoding.UTF8.GetBytes(_));
    // }
    //
    
}

