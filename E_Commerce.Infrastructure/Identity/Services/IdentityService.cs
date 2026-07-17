using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail(Error.NotFound("User Not Found", $"User with Email {email} Is Not Found"));
            else
                return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e=>new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id,user.DisplayName,user.Email,user.UserName));
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User with Email {email} Is Not Found"));
            else
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, ct);
            if (user?.Address == null)
                return Result<AddressDto>.Fail(Error.NotFound("Address Not Found", $"Address of User With Email {email} Is Not Found"));
            var address = user.Address;
            return new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Country = address.Country,
                Street = address.Street
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("User Not Found", $"User With Email {email} Is Not Found");
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, ct);
            if(user?.Address == null)
            {
                // Insert
                user.Address = new Address()
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    City = addressDto.City,
                    Street = addressDto.Street,
                    Country = addressDto.Country,
                };
            }
            else
            {
                // Update
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.City = addressDto.City;
                user.Address.Street = addressDto.Street;
                user.Address.Country = addressDto.Country;
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return addressDto;
            else
                return Error.Failure("Failure", string.Join(';', result.Errors.Select(e => e.Description)));
        }
    }
}
