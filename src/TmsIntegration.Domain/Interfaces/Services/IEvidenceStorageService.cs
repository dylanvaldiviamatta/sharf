namespace TmsIntegration.Domain.Interfaces.Services;

public interface IEvidenceStorageService
{
    Task<string> StoreAsync(
        string orderNumber,
        string fileName,
        string sourceUrl,
        CancellationToken cancellationToken = default);
}
