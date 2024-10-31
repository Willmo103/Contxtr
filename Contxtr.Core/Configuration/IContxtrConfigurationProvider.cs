namespace Contxtr.Core.Configuration
{
    public interface IContxtrConfigurationProvider
    {
        Task<ContxtrConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken = default);
    }
}
