# Azure Key Vault Secrets Value Provider

**What is this extension?**

This extension is designed to securely and conveniently use secrets (such as passwords, API keys, or connection strings) stored in your Azure Key Vault directly within your RestApia API requests.

**Why is this useful?**

* **Enhanced Security:** Avoid hardcoding sensitive information directly into your API requests. Instead, retrieve them securely from Azure Key Vault.
* **Simplified Management:** Centrally manage your secrets in Azure Key Vault, eliminating the need to update them across multiple locations.

**How does it work?**

Imagine you have an API request that requires sensitive values that cannot be safely stored within your RestApia collection. You can utilize an Azure Key Vault secret as a templated variable, which can be used in headers, query arguments, or for configuring authorization.

* The extension retrieves the secret from your Azure Key Vault.
* All retrieved values are cached securely as encrypted data on your local machine, improving performance for subsequent requests.
* **Authentication:** The extension uses the [DefaultAzureCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet) for seamless authentication with Azure. This supports various authentication methods, including environment variables, managed identities, Azure CLI, Visual Studio, and more, automatically selecting the appropriate method based on your environment.

**What do you need?**

* An active Azure subscription with an Azure Key Vault where your secrets are stored.
* Appropriate permissions for your Azure account to access those secrets.
* The RestApia application.