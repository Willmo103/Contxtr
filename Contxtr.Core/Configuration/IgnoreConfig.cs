using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contxtr.Core.Models;

namespace Contxtr.Core.Configuration
{
    public class IgnoreConfig
    {
        public bool EnableGitIgnore { get; set; } = true;
        public bool EnableSystemIgnores { get; set; } = true;
        public List<IgnorePattern> Patterns { get; set; } = new();

        public static IgnoreConfig FromJson(JsonElement json)
        {
            var config = new IgnoreConfig();

            if (json.TryGetProperty("EnableGitIgnore", out var gitIgnore))
            {
                config.EnableGitIgnore = gitIgnore.GetBoolean();
            }

            if (json.TryGetProperty("EnableSystemIgnores", out var sysIgnore))
            {
                config.EnableSystemIgnores = sysIgnore.GetBoolean();
            }

            if (json.TryGetProperty("Patterns", out var patterns))
            {
                foreach (var pattern in patterns.EnumerateObject())
                {
                    config.Patterns.Add(new IgnorePattern
                    {
                        Pattern = pattern.Name,
                        Description = pattern.Value.GetString() ?? string.Empty,
                        IsRegex = pattern.Name.Contains('*') || pattern.Name.Contains('?'),
                        IsActive = true,
                        Category = "User"
                    });
                }
            }

            return config;
        }
    }
}
