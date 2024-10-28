using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.IgnorePatterns
{
    public class JsonIgnorePatternProvider : IIgnorePatternProvider
    {
        private readonly string _configPath;
        private readonly ILogger<JsonIgnorePatternProvider> _logger;

        public JsonIgnorePatternProvider(
            IConfiguration configuration,
            ILogger<JsonIgnorePatternProvider> logger)
        {
            _configPath = configuration.GetValue<string>("IgnorePatterns:JsonPath")
                ?? "ignore-patterns.json";
            _logger = logger;
        }

        public async Task<IgnorePatterns> LoadPatternsAsync(
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_configPath))
            {
                _logger.LogWarning("No ignore patterns file found at: {Path}", _configPath);
                return new IgnorePatterns { Source = "json" };
            }
             
            var json = await File.ReadAllTextAsync(_configPath, cancellationToken);
            var patterns = JsonSerializer.Deserialize<IgnorePatterns>(json)
                ?? new IgnorePatterns { Source = "json" };
            return patterns;
        }
    }

    public class SqlIgnorePatternProvider : IIgnorePatternProvider
    {
        private readonly string _connectionString;
        private readonly ILogger<SqlIgnorePatternProvider> _logger;

        public SqlIgnorePatternProvider(
            IConfiguration configuration,
            ILogger<SqlIgnorePatternProvider> logger)
        {
            _connectionString = configuration.GetConnectionString("IgnorePatternsDb")
                ?? throw new ArgumentNullException("Connection string not found");
            _logger = logger;
        }

        public async Task<IgnorePatterns> LoadPatternsAsync(
            CancellationToken cancellationToken = default)
        {
            // Implementation for loading from SQL
            // This is just a skeleton - implement based on your DB schema
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var patterns = new IgnorePatterns { Source = "sql" };

            using var command = new SqlCommand(
                "SELECT Pattern, Description, IsRegex, Category FROM IgnorePatterns",
                connection);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                patterns.Patterns.Add(new IgnorePattern
                {
                    Pattern = reader.GetString(0),
                    Description = reader.GetString(1),
                    IsRegex = reader.GetBoolean(2),
                    Category = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }

            return patterns;
        }
    }
}
