using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eShopSolution.Data.EF
{
    public class EShopSolutionContextFactory : IDesignTimeDbContextFactory<EShopSolutionDbContext>
    {
        public EShopSolutionDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            var connectionstring = configuration.GetConnectionString("eShopSolutionDb");

            var optionsBuilder = new DbContextOptionsBuilder<EShopSolutionDbContext>();
            optionsBuilder.UseSqlServer(connectionstring);
            return new EShopSolutionDbContext(optionsBuilder.Options);
        }
    }
}
