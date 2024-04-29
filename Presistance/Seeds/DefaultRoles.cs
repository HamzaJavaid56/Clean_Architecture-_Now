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
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>(); // create the instance of ApplicationRole at runtime

            var superAdmin = new ApplicationRole();
            superAdmin.Name = Roles.SuperAdmin.ToString();
            superAdmin.NormalizedName = Roles.SuperAdmin.ToString().ToUpper();
            await roleManager.CreateAsync(superAdmin);

            var admin = new ApplicationRole();
            admin.Name = Roles.Admin.ToString();
            admin.NormalizedName = Roles.Admin.ToString().ToUpper();
            await roleManager.CreateAsync(admin);

            var basic = new ApplicationRole();
            basic.Name = Roles.Basic.ToString();
            basic.NormalizedName = Roles.Basic.ToString().ToUpper();
            await roleManager.CreateAsync(basic);
        }
    }
}
