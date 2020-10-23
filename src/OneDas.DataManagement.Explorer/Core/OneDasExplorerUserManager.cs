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
                var defaultRootUsername = "root@root.org";
                var defaultRootPassword = "#root0/User1";

                string rootUsername = Environment.GetEnvironmentVariable("ONEDAS_ROOT_USERNAME");
                string rootPassword = Environment.GetEnvironmentVariable("ONEDAS_ROOT_PASSWORD");

                // remove any references to defaultUserName
                if (!string.IsNullOrWhiteSpace(rootUsername))
                {
                    var userToDelete = userManager.FindByNameAsync(defaultRootUsername).Result;

                    if (userToDelete != null)
                        userManager.DeleteAsync(userToDelete);
                }
                // fallback to default user
                else
                {
                    rootUsername = defaultRootUsername;
                }

                // fallback to default password
                if (string.IsNullOrWhiteSpace(rootPassword))
                    rootPassword = defaultRootPassword;

                // ensure there is a root user
                if (userManager.FindByNameAsync(rootUsername).Result == null)
                {
                    var user = new IdentityUser()
                    {
                        UserName = rootUsername,
                    };

                    var result = userManager.CreateAsync(user, rootPassword).Result;

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
