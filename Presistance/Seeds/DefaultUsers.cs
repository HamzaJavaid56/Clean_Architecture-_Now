using Application.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Presistance.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistance.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser();

            user.Email= "Admin@gmail.com";
            user.UserName = user.Email;
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.IsFirstLogin = true;

            if (userManager.Users.All(x => x.Id != user.Id))
            {
                var result = await userManager.FindByEmailAsync(user.Email);

                if (result == null)
                {
                    await userManager.CreateAsync(user, "Admin@gmail.com123");
                    await userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                
                    }
            }
        }
    }
}
