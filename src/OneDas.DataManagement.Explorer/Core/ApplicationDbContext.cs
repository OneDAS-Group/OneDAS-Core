using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace OneDas.DataManagement.Explorer.Core
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private OneDasExplorerOptions _options;

        public ApplicationDbContext(OneDasExplorerOptions options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = Path.Combine(_options.DataBaseFolderPath, "users.db");
            optionsBuilder.UseSqlite($"Data Source={filePath}");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
