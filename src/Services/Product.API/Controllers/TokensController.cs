using Contracts.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Identity;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TokensController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [HttpPost]
    [AllowAnonymous]
    public IActionResult GetToken()
    {
        var token = _tokenService.GetToken(new TokenRequest());
        return Ok(token);
    }
}