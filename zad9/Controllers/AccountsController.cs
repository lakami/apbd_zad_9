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
    
    
    
}

