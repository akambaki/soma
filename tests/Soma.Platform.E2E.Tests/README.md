# End-to-End Tests

## Current Status: Temporarily Disabled in CI

The E2E tests in this project are currently **placeholder tests** and are temporarily disabled in the CI/CD pipeline due to Playwright browser installation timeouts in the GitHub Actions environment.

## What's in the E2E Tests

The current E2E tests (`UnitTest1.cs`) contain:
- Basic Playwright setup and browser initialization
- Placeholder test methods for authentication flows
- All actual test logic is commented out (as it requires running applications)

## Why Disabled in CI

The CI/CD pipeline was experiencing consistent failures during the Playwright browser download step, specifically:
- Chromium browser download hanging at 70-80% completion
- Timeout issues in GitHub Actions environment
- Blocking other important tests from running

## Re-enabling E2E Tests

To re-enable E2E tests in CI:

1. **Uncomment the e2e-tests job** in `.github/workflows/ci.yml`
2. **Update the test summary job** to include e2e-tests dependency
3. **Implement proper test scenarios** that actually test the application

## Running E2E Tests Locally

To run E2E tests locally:

```bash
# Install Playwright browsers
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium

# Run the tests
dotnet test tests/Soma.Platform.E2E.Tests
```

**Note**: The tests will currently fail because they're placeholder tests that don't actually test anything meaningful.

## Future Implementation

When implementing real E2E tests:

1. Start the Web application (Blazor Server)
2. Start the API application  
3. Use Playwright to navigate to pages and test user workflows
4. Verify actual functionality like login, registration, profile management

## Alternative Testing Strategy

Until E2E tests are properly implemented, the authentication functionality is thoroughly tested through:
- **Unit Tests**: Testing individual components and services
- **Integration Tests**: Testing API endpoints with real database
- **Manual Testing**: Using the actual web application

These provide comprehensive coverage of the authentication system without requiring browser automation.