using Demo.Abstractions;
using Demo.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using PipelineOriented.Continuations;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(BaseRequestDto<LoginRequest> request)
        {
            return Ok(await _accountService.LoginAsync(request));
        }

     

    }
}
