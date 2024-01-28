using Microsoft.AspNetCore.Mvc;
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

