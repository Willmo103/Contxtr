using System.Text.RegularExpressions;
using Contxtr.Core.Configuration;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Processing
{
    public class CodebaseProcessor : ICodebaseProcessor
    {
        private readonly IDocumentProcessor _documentProcessor;
        private readonly ILogger<CodebaseProcessor> _logger;
        private readonly ContxtrConfiguration _configuration;
        private IgnorePatterns _ignorePatterns;

        public CodebaseProcessor(
            IDocumentProcessor documentProcessor,
            ILogger<CodebaseProcessor> logger,
            IContxtrConfigurationProvider configProvider)
        {
            _documentProcessor = documentProcessor;
            _logger = logger;
            _configuration = configProvider.LoadConfigurationAsync().GetAwaiter().GetResult();
        }

        public async Task<CodebaseStructure> ProcessCodebaseAsync(
            string rootPath,
            IgnorePatterns ignorePatterns,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing codebase at: {Path}", rootPath);
            _ignorePatterns = ignorePatterns;

            var codebase = new CodebaseStructure
            {
                Name = Path.GetFileName(rootPath),
                RootPath = rootPath,
                RootDirectory = await ProcessDirectoryAsync(
                    rootPath,
                    rootPath,
                    cancellationToken)
            };

            // Flatten all documents into the main collection for easy access
            FlattenDocuments(codebase.RootDirectory, codebase.Documents);

            // Add git metadata if available
            if (_configuration.Ignore.EnableGitIgnore && Directory.Exists(Path.Combine(rootPath, ".git")))
            {
                await AddGitMetadataAsync(codebase, cancellationToken);
            }

            return codebase;
        }

        private async Task<DirectoryNode> ProcessDirectoryAsync(
            string path,
            string rootPath,
            CancellationToken cancellationToken)
        {
            var directoryInfo = new DirectoryInfo(path);
            var node = new DirectoryNode
            {
                Name = directoryInfo.Name,
                FullPath = path,
                RelativePath = Path.GetRelativePath(rootPath, path)
            };

            // Process subdirectories
            foreach (var subDir in directoryInfo.GetDirectories())
            {
                if (ShouldIgnore(subDir.FullName))
                {
                    _logger.LogDebug("Ignoring directory: {Path}", subDir.FullName);
                    continue;
                }

                var subNode = await ProcessDirectoryAsync(
                    subDir.FullName,
                    rootPath,
                    cancellationToken);
                node.Subdirectories.Add(subNode);
            }

            // Process files
            foreach (var file in directoryInfo.GetFiles())
            {
                if (ShouldIgnore(file.FullName))
                {
                    _logger.LogDebug("Ignoring file: {Path}", file.FullName);
                    continue;
                }

                try
                {
                    var document = await _documentProcessor.ProcessAsync(file.FullName, cancellationToken);
                    node.Documents.Add(document);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {Path}", file.FullName);
                }
            }

            return node;
        }

        private bool ShouldIgnore(string path)
        {
            var normalizedPath = path.Replace('\\', '/');

            // Check custom ignore patterns first
            foreach (var pattern in _ignorePatterns.Patterns)
            {
                if (pattern.IsRegex)
                {
                    if (Regex.IsMatch(normalizedPath, pattern.Pattern))
                    {
                        _logger.LogTrace("Path {Path} matched custom regex pattern {Pattern}", path, pattern.Pattern);
                        return true;
                    }
                }
                else
                {
                    if (normalizedPath.Contains(pattern.Pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogTrace("Path {Path} matched custom simple pattern {Pattern}", path, pattern.Pattern);
                        return true;
                    }
                }
            }

            // Check configuration patterns
            foreach (var pattern in _configuration.Ignore.Patterns)
            {
                if (pattern.Pattern != null && normalizedPath.Contains(pattern.Pattern, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogTrace("Path {Path} matched config pattern {Pattern}", path, pattern.Pattern);
                    return true;
                }
            }

            // Check .gitignore if enabled
            if (_configuration.Ignore.EnableGitIgnore)
            {
                var gitIgnorePath = Path.Combine(_configuration.Storage.BaseDirectory, ".gitignore");
                if (File.Exists(gitIgnorePath))
                {
                    var gitIgnorePatterns = File.ReadAllLines(gitIgnorePath)
                        .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'));

                    foreach (var pattern in gitIgnorePatterns)
                    {
                        if (GitWildcardMatches(normalizedPath, pattern))
                        {
                            _logger.LogTrace("Path {Path} matched gitignore pattern {Pattern}", path, pattern);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool GitWildcardMatches(string path, string pattern)
        {
            // Convert git-style pattern to regex
            string regex = "^" + Regex.Escape(pattern)
                .Replace("\\*\\*/", ".*?")  // **/ means any directory depth
                .Replace("\\*", "[^/]*")    // * means any characters except /
                .Replace("\\?", "[^/]")     // ? means any single character except /
                + "$";

            return Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);
        }

        private void FlattenDocuments(DirectoryNode node, ICollection<Document> allDocuments)
        {
            foreach (var doc in node.Documents)
            {
                allDocuments.Add(doc);
            }

            foreach (var subDir in node.Subdirectories)
            {
                FlattenDocuments(subDir, allDocuments);
            }
        }

        private async Task AddGitMetadataAsync(CodebaseStructure codebase, CancellationToken cancellationToken)
        {
            try
            {
                using var repo = new Repository(codebase.RootPath);
                codebase.Metadata["GitBranch"] = repo.Head.FriendlyName;
                codebase.Metadata["GitCommit"] = repo.Head.Tip.Sha;
                codebase.Metadata["GitRemoteUrl"] = repo.Network.Remotes.FirstOrDefault()?.Url ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add git metadata for {Path}", codebase.RootPath);
            }
        }
    }
}