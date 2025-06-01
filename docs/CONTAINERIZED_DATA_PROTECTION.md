# Data Protection Configuration for Containerized Environments

## Problem

When running the SOMA platform in containerized environments (Docker/Kubernetes), users may encounter antiforgery token decryption errors like:

```
Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException: The antiforgery token could not be decrypted.
System.Security.Cryptography.CryptographicException: The key was not found in the key ring.
```

This happens because:
1. Data protection keys are generated when the application starts
2. Keys are stored in temporary directories that don't persist across container restarts
3. When the container restarts, new keys are generated
4. Previously encrypted antiforgery tokens can't be decrypted with the new keys

## Solution

The SOMA Web application now includes persistent data protection configuration:

### Configuration Details

**Program.cs** - Added data protection configuration:
```csharp
// Configure Data Protection for containerized environment
var dataProtectionKeyPath = Environment.GetEnvironmentVariable("DATA_PROTECTION_KEY_PATH") ?? "/tmp/dataprotection-keys";

// Ensure the key directory exists
Directory.CreateDirectory(dataProtectionKeyPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath))
    .SetApplicationName("Soma.Platform.Web");
```

**docker-compose.yml** - Added persistent volume for keys:
```yaml
soma-web:
  environment:
    - DATA_PROTECTION_KEY_PATH=/app/dataprotection-keys
  volumes:
    - soma-web-keys:/app/dataprotection-keys

volumes:
  soma-web-keys:
    driver: local
```

### Key Benefits

1. **Persistent Keys**: Data protection keys are stored in a persistent volume
2. **Container Restart Resilience**: Keys survive container restarts and updates
3. **Configurable Path**: Key storage location can be customized via environment variable
4. **Cross-Platform Support**: Works on Linux and Windows containers

### Environment Variables

- `DATA_PROTECTION_KEY_PATH`: Directory to store data protection keys (default: `/tmp/dataprotection-keys`)

### Testing

The fix includes comprehensive integration tests that verify:
- Data protection service is configured correctly
- Keys can encrypt and decrypt data
- Key directory is created properly
- Application name is set for key isolation

## Usage

When deploying with Docker Compose:
```bash
docker-compose up -d
```

The volume `soma-web-keys` will automatically persist data protection keys across container restarts.

For Kubernetes deployments, ensure you have a persistent volume mounted at the configured key path.

## Production Considerations

For production environments, consider:
1. Using a shared storage solution (Azure Blob, AWS S3, Redis) for multi-instance deployments
2. Implementing proper key encryption at rest
3. Regular key rotation policies
4. Backup strategies for the key storage

This solution resolves the immediate container restart issue while maintaining security best practices.