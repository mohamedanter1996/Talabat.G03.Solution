using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities;
using Talabat.Core.Service.Contract;

namespace Talabat.APIs.Controllers
{

	public class AccountController : BaseApiController
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthService authService,IMapper mapper)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_authService = authService;
			_mapper = mapper;
		}
		[HttpPost("login")] // POST : /api/Account/login

		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var User = await _userManager.FindByEmailAsync(model.Email); //انا كده بدور جوه ال users عشان اتأكد إنه كان عامل registeration عندي قبل كده لأن هو ده لوقتي بيعمل عندي login طب بدور ازاي من خلال ال object الي من class user manager ده 
			if (User is null)
			{
				return Unauthorized(new ApiResponse(401, "Invalid Login"));
			}
			var result = await _signInManager.CheckPasswordSignInAsync(User, model.Password, false);

			if (!result.Succeeded)
			{
				return Unauthorized(new ApiResponse(401, "Invalid Login"));
			}

			return Ok(new UserDto()
			{
				DisplayName = User.DisplayName,
				Email = User.Email,
				Token = await _authService.CreateTokenAsync(User, _userManager)
			});


		}


		[HttpPost("register")] // POST : /api/Account/register

		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			var user = new ApplicationUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split("@")[0],
				PhoneNumber = model.Phone
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded) { return BadRequest(new ApivalidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) }); }

			return Ok(new UserDto() { DisplayName = user.DisplayName, Email = user.Email, Token = await _authService.CreateTokenAsync(user, _userManager) });

		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet]  // Get: /api/Account
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

			var user = await _userManager.FindByEmailAsync(email);



			return Ok(new UserDto()
			{
				DisplayName = user?.DisplayName ?? string.Empty,
				Email = user?.Email ?? string.Empty,
				Token = await _authService.CreateTokenAsync(user, _userManager)
			});
		}


		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("address")] //GEt:/api/Account/address
		public async Task<ActionResult<AddressDto>> GetUserAddress()
		{
			
			var user = await _userManager.FindWithAddressAsync(User);

			return Ok(_mapper.Map<AddressDto>(user.Address));
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPut("address")] //Put:/api/Account/address

		public async Task<ActionResult<Talabat.Core.Entities.Address>> UpdateUserAddress(AddressDto address)
		{
			var updatedAddress=_mapper.Map<Talabat.Core.Entities.Address>(address);

			var user = await _userManager.FindWithAddressAsync(User);

			updatedAddress.Id = user.Address.Id;

			user.Address = updatedAddress;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded) { return BadRequest(new ApivalidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) }); }
			return Ok(address);
		}
	}
}
