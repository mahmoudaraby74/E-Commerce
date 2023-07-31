using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Dtos;
using StoreAPI.ResponseModule;
using System.Security.Claims;

namespace StoreAPI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var userDto = new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = tokenService.CreateToken(user)
            };

            return userDto;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            var userDto = new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = tokenService.CreateToken(user)
            };

            return userDto;
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult<UserDto>> SignUp(SignUpDto signUpDto)
        {
            var user = new AppUser
            {
                Email = signUpDto.Email,
                DisplayName = signUpDto.DisplayName,
                UserName = signUpDto.Email
            };

            var result = await userManager.CreateAsync(user, signUpDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            var userDto = new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = tokenService.CreateToken(user)
            };

            return userDto;
        }
        [Authorize]
        [HttpGet("GetUserAddress")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await userManager.Users
        .Include(u => u.Address)
        .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
            if (user == null)
            {
                return NotFound(new ApiResponse(404));
            }
            var mappedAddress = mapper.Map<AddressDto>(user.Address);
            return Ok(mappedAddress);
        }
        [Authorize]
        [HttpPost("UpdateUserAddress")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user == null)
            {
                return NotFound(new ApiResponse(404));
            }

            mapper.Map(addressDto, user.Address);
            var result =await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Problem Updating User Address"));
            }

            // Map the updated Address entity to an AddressDto object
            var updatedAddressDto = mapper.Map<AddressDto>(user.Address);

            return Ok(updatedAddressDto);
        }
    }
}
