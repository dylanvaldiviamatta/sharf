using TmsIntegration.Domain.Interfaces.Services;

namespace TmsIntegration.Infrastructure.Services;

public sealed class MockEvidenceStorageService : IEvidenceStorageService
{
    private const string MockStorageBaseUrl = "https://storage.mock";

    public Task<string> StoreAsync(
        string orderNumber,
        string fileName,
        string sourceUrl,
        CancellationToken cancellationToken = default)
    {
        var storedUrl = $"{MockStorageBaseUrl}/{orderNumber}/{fileName}";
        return Task.FromResult(storedUrl);
    }
}
