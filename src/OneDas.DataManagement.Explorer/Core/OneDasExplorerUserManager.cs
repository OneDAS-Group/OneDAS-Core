using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace OneDas.DataManagement.Explorer.Core
{
    public class OneDasExplorerUserManager
    {
        private ILogger _logger;
        private IServiceProvider _serviceProvider;

        // Both, userDB and userManager, cannot be pulled in here because they are scoped
        public OneDasExplorerUserManager(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger("OneDAS Explorer");
        }

        public void Initialize()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userDB = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // database
                if (userDB.Database.EnsureCreated())
                    _logger.LogInformation($"SQLite database initialized.");

                // ensure there is a root user
                if (userManager.FindByNameAsync("root@root.org").Result == null)
                {
                    var user = new IdentityUser()
                    {
                        UserName = "root@root.org",
                    };

                    var defaultPassword = "#root0/User1";
                    var result = userManager.CreateAsync(user, defaultPassword).Result;

                    if (result.Succeeded)
                    {
                        var claim = new Claim("IsAdmin", "true");
                        userManager.AddClaimAsync(user, claim).Wait();
                    }
                }

                // ensure there is a test user
                if (userManager.FindByNameAsync("test@root.org").Result == null)
                {
                    var user = new IdentityUser()
                    {
                        UserName = "test@root.org",
                    };

                    var defaultPassword = "#test0/User1";
                    var result = userManager.CreateAsync(user, defaultPassword).Result;
                }
            }
        }
    }
}
