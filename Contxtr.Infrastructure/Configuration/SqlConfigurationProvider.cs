using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Configuration;
using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Configuration
{
    public class SqlConfigurationProvider : BaseConfigurationProvider
    {
        private readonly string _connectionString;

        public SqlConfigurationProvider(
            ILogger<SqlConfigurationProvider> logger,
            IConfiguration configuration)
            : base(logger, configuration)
        {
            _connectionString = configuration.GetConnectionString("ContxtrConfig")
                ?? throw new ArgumentNullException("ContxtrConfig connection string not found");
        }

        public override async Task<ContxtrConfiguration> LoadConfigurationAsync(
            CancellationToken cancellationToken = default)
        {
            // Use EntityFramework or Dapper to load configuration from database
            var config = new ContxtrConfiguration();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            //// Example using Dapper
            //config.LanguageMap.Extensions = (await connection.QueryAsync<KeyValuePair<string, string>>(
            //    "SELECT Extension, Language FROM LanguageMap"))
            //    .ToDictionary(x => x.Key, x => x.Value);

            //config.Ignore.Patterns = (await connection.QueryAsync<IgnorePattern>(
            //"SELECT Pattern, Description FROM IgnorePatterns")).ToList();

            // Load other configuration sections...

            return ApplyDefaults(config);
        }
    }
}
