## Common targets

- `.\build.ps1 Clean` - Clean up output directories

## Extensions build

- `.\build.ps1 Extension_Build --ExtensionName RestApia.Extensions.Auth.OAuth2` - build NuGet package for the OAuth2 extension
  - optional `--ExtensionLibVersion 1.0.0` to specify the version
- `.\build.ps1 Extension_Push --ExtensionName RestApia.Extensions.Auth.OAuth2` - build the OAuth2 extension and push it to the NuGet server

## Shared library build

- `.\build.ps1 Shared_Build` - build NuGet package for the shared library
  - optional `--SharedLibVersion 1.0.0` to specify the version
- `.\build.ps1 Shared_Push` - build the shared library and push it to the NuGet server
