namespace RestApia.Extensions.ValuesProvider.AzureKeyVault;

public record KeyVaultSettings
{
    public required string KeyVaultUrl { get; init; }
}
