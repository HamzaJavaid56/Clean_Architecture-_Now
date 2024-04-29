using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Presistance.Helper;
using Presistance.SharedServices;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        public int? CurrentUserId { get; set; }
        public string? IpAddress { get; set; }


        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            IpAddress = IpAddressHelper.GetIpAddresss(ctx.HttpContext);

            if (ctx.HttpContext.Request.Headers["Authorization"].FirstOrDefault() != null)
            {

                string token = ctx.HttpContext.Request.Headers["Authorization"].FirstOrDefault().Split("Bearer")[1].Trim();
                if (String.IsNullOrEmpty(token) || token == "undefined")
                {
                    return;
                }
                CurrentUserId = Authorization.GetUserIdFromJWT(token);

            }
        }
    }
}
