using Application.DTO;
using Application.Enums;
using Application.Exceptions;
using Application.Features.User.Command;
using Application.Interfaces;
using Application.Wrappers;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Presistance.IdentityModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presistance.SharedServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public AccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMediator mediator)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
        }
        /// <summary>
        /// Registring the user by taking the user object 
        /// </summary>
        /// <param name="registerRequest">user request data</param>
        /// <returns></returns>
        /// <exception cref="ApiException">respone of type integer</exception>
        public async Task<ApiResponse<int>> RegisterUser(RegisterRequest registerRequest)
        {
          
            var user = await _userManager.FindByEmailAsync(registerRequest.UserName);
            if (user != null)
            {
                throw new ApiException($"User Name Not Valid {registerRequest.UserName}");
            }

            var userModel = new ApplicationUser();
            userModel.Email = registerRequest.UserName;
            userModel.UserName = registerRequest.UserName;
            userModel.FirstName = registerRequest.FirstName;
            userModel.LastName = registerRequest.LastName;
            userModel.Device = registerRequest.Device;
            userModel.IpAddress = registerRequest.IpAddress;
            userModel.SecurityStamp = Guid.NewGuid().ToString();
            userModel.IsFirstLogin = true;
            var result = await _userManager.CreateAsync(userModel, registerRequest.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userModel, Roles.Basic.ToString());
                return new ApiResponse<int>(userModel.Id, "User Register successfuly");
            }
            else
            {
                throw new ApiException(result.Errors.ToString());
            }

        }

        /// <summary>
        /// Authenticating the user by taking user name and password and return token if authenticated
        /// </summary>
        /// <param name="request">request of authtcation</param>
        /// <returns></returns>
        /// <exception cref="ApiException">response contain token and other user data</exception>
        public async Task<ApiResponse<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);
            if (user == null)
            {
                // for security reason show generic message
                throw new ApiException($"Incorrect username or password  {request.UserName}");
            }

            var succeeded = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!succeeded)
            {
                throw new ApiException($"Incorrect username or password");
            }

            var jwtSecurity = await GenerateTokenAsync(user);

            var authenticationResponse = new AuthenticationResponse();
            authenticationResponse.FirstName = user.FirstName;
            authenticationResponse.LastName = user.LastName;

            //  var roles = await _userManager.GetRolesAsync(user);

            authenticationResponse.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);


            // Check if the user is login first time then add the balance 5
            if (user.IsFirstLogin)
            {
                user.IsFirstLogin = false;
                await AddUserBalance(user.Id, 5);
                await _userManager.UpdateAsync(user);
            }
            return new ApiResponse<AuthenticationResponse>(authenticationResponse, "User Authenticated Successfully !");
        }

        /// <summary>
        /// Generate the Jwt token on user claims
        /// </summary>
        /// <param name="user">request contain the user object and </param>
        /// <returns> return token in response </returns>
        private async Task<JwtSecurityToken> GenerateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            //string ipAddress = "192.33";
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
              //  new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),

                new Claim("uid", user.Id.ToString())
            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:JwtIssuer"],
                audience: _configuration["JwtSettings:JwtAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        private async Task AddUserBalance(int userId, double balance)
        {
            await _mediator.Send(

                   new CreateUserAccountCommand()
                   {
                       UserId = userId,
                       Balance = balance,
                   }
                    );
        }
    
    }
}
