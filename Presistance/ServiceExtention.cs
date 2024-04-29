using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Presistance.Context;
using Presistance.IdentityModels;
using Presistance.Seeds;
using Presistance.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Presistance
{
    public static class ServiceExtention
    {
        public static async void AddPresistance(this IServiceCollection services, IConfiguration configuration)
        {

            // Registration in DI container

            #region DbConnectivity

            // For SqlServer

            /*
            var connectionString = configuration.GetConnectionString("DefaultConnectionSqlServer");
            services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));
            */

            // For MySql

            var connectionString = configuration.GetConnectionString("DefaultConnectionMySql");
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<ApplicationDbContext>(option => option.UseMySql(connectionString, serverVersion));

            #endregion
           
            // DI Identity
            services.AddIdentityCore<ApplicationUser>()
                 .AddRoles<ApplicationRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>();

            
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddTransient<IAccountService, AccountService>();

            #region Seeding
           
            DefaultRoles.SeedRolesAsync(services.BuildServiceProvider()).Wait();
            DefaultUsers.SeedUsersAsync(services.BuildServiceProvider()).Wait();
            
            #endregion
        }
    }
}
