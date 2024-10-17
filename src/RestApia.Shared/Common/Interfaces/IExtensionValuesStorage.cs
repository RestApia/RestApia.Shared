using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Common.Interfaces;

/// <summary>
/// Extension values storage.
/// </summary>
public interface IExtensionValuesStorage
{
    /// <summary>
    /// Read all extension values.
    /// </summary>
    IReadOnlyCollection<ExtensionValueModel> GetValues(Guid dataId);

    /// <summary>
    /// Update extension values.
    /// </summary>
    void SetValues(IReadOnlyCollection<ExtensionValueModel> values, Guid dataId, DateTime? expiresAt = null);
}
