using Application.DTO;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presistance.SharedServices;
using Microsoft.AspNetCore.HttpOverrides;
using Azure.Core;
using System.Net;
using Microsoft.Extensions.Hosting;
using Application.Features.Account.Query;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IAccountService _accountService;

        public UserController(IMediator mediator, IAccountService accountService)
        {
            _mediator = mediator;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterRequest user)
        {
            user.IpAddress = IpAddress;
            var result = await _accountService.RegisterUser(user);
            return Ok(result);
        }

        [Route("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticationRequest user)
        {
            var result = await _accountService.Authenticate(user);
            return Ok(result);
        }

        [Authorize]
        [Route("auth/balance")]
        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var result = await _mediator.Send(new GetUserBalanceByIdQuery() { UserId = CurrentUserId });
            return Ok(result);

        }

        [Route("exception")]
        [HttpGet]
        public  Task<IActionResult> ThrowException()
        {
            throw new Exception("some exception occure");

        }
    }
}
