using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        // Login [Email + Password] => Token, Email, DisplayName
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
    }
}
