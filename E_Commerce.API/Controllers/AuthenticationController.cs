using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        // Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
            => ToActionResult(await _authenticationService.LoginAsync(loginDto));
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct)
            => ToActionResult(await _authenticationService.RegisterAsync(registerDto, ct));
        [HttpGet("email-exists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery]string email, CancellationToken ct)
            => ToActionResult(await _authenticationService.CheckEmailExistsAsync(email, ct));
        [Authorize]
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
            => ToActionResult(await _authenticationService.GetCurrentUserAsync(GetEmailFromToken(), ct));
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken ct)
            => ToActionResult(await _authenticationService.GetUserAddressAsync(GetEmailFromToken(), ct));
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto, CancellationToken ct)
            => ToActionResult(await _authenticationService.UpsertUserAddressAsync(GetEmailFromToken(), addressDto, ct));
    }
}
