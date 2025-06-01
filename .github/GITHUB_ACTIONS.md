# GitHub Actions CI/CD Documentation

This document describes the GitHub Actions workflows implemented for the SOMA platform to ensure code quality and catch regressions early.

## 📋 Overview

The SOMA platform uses multiple GitHub Actions workflows to maintain code quality:

1. **`ci.yml`** - Main CI/CD pipeline for comprehensive testing
2. **`pr-validation.yml`** - Fast validation for pull requests
3. **`nightly-tests.yml`** - Comprehensive regression testing

## 🚀 Workflows

### 1. Main CI/CD Pipeline (`ci.yml`)

**Triggers**: Push to `main`/`develop`, Pull Requests
**Duration**: ~15-20 minutes

**Jobs**:
- **Build**: Validates solution builds successfully
- **Unit Tests**: Fast unit tests (Core, API, Web)
- **Integration Tests**: Database integration tests with PostgreSQL
- **E2E Tests**: Browser automation tests with Playwright
- **Code Quality**: Code analysis and coverage reporting
- **Test Summary**: Consolidated test results

**Features**:
- ✅ Parallel test execution for faster feedback
- ✅ PostgreSQL service containers for realistic testing
- ✅ Playwright browser automation
- ✅ Code coverage collection
- ✅ Test result artifacts and reporting
- ✅ Caching for faster builds

### 2. PR Validation (`pr-validation.yml`)

**Triggers**: Pull Request events (opened, synchronized, reopened)
**Duration**: ~5-8 minutes

**Jobs**:
- **Validate PR**: Fast unit tests and code formatting
- **Changes Analysis**: Analyzes changed files and impact

**Features**:
- ✅ Fast feedback for PRs (unit tests only)
- ✅ Code formatting validation
- ✅ Automated PR comments with results
- ✅ Breaking change detection
- ✅ Impact assessment of changes

### 3. Nightly Regression Tests (`nightly-tests.yml`)

**Triggers**: Scheduled (2 AM UTC daily), Manual dispatch
**Duration**: ~25-30 minutes

**Jobs**:
- **Comprehensive Test Suite**: Full test execution using `run-tests.sh`
- **Security Scan**: Vulnerability analysis
- **Performance Tests**: Basic load testing (optional)

**Features**:
- ✅ Complete test suite execution
- ✅ Security vulnerability scanning
- ✅ Performance regression detection
- ✅ Detailed reporting and artifacts

## 🧪 Test Execution Strategy

### Unit Tests (Fastest - ~2-3 minutes)
```
- Soma.Platform.Core.Tests
- Soma.Platform.Api.Tests  
- Soma.Platform.Web.Tests
```

### Integration Tests (Medium - ~5-7 minutes)
```
- Soma.Platform.Api.IntegrationTests
- Soma.Platform.Web.IntegrationTests
```

### E2E Tests (Slowest - ~8-10 minutes)
```
- Soma.Platform.E2E.Tests (Playwright)
```

## 🔧 Configuration

### Environment Variables
```yaml
DOTNET_VERSION: '8.0.x'
ConnectionStrings__DefaultConnection: PostgreSQL connection
ASPNETCORE_ENVIRONMENT: Testing
```

### Services
- **PostgreSQL 16**: Database for integration/E2E tests
- **Playwright**: Browser automation for E2E tests

### Caching Strategy
- **NuGet packages**: `~/.nuget/packages`
- **Playwright browsers**: `~/.cache/ms-playwright`

## 📊 Test Results & Artifacts

### Artifacts Generated
- `unit-test-results` - Unit test TRX files
- `integration-test-results` - Integration test TRX files  
- `e2e-test-results` - E2E test TRX files
- `code-coverage` - Coverage reports
- `e2e-screenshots` - Failure screenshots (if E2E fails)

### Test Reporting
- **GitHub Checks**: Pass/fail status on commits
- **PR Comments**: Automated validation feedback
- **Test Summary**: Consolidated results in GitHub UI
- **Coverage Reports**: Code coverage analysis

## 🛠️ Local Development

### Running Tests Locally
```bash
# Run all tests (same as CI)
./run-tests.sh

# Run specific test types
dotnet test tests/Soma.Platform.Core.Tests/
dotnet test tests/Soma.Platform.Api.IntegrationTests/
dotnet test tests/Soma.Platform.E2E.Tests/
```

### Setup E2E Tests
```bash
# Install Playwright
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium
```

### Code Formatting
```bash
# Check formatting
dotnet format --verify-no-changes

# Fix formatting
dotnet format
```

## 🔍 Troubleshooting

### Common Issues

#### 1. E2E Tests Failing
- **Cause**: Browser not installed or pages not loading
- **Solution**: Check Playwright setup and application startup

#### 2. Integration Tests Failing  
- **Cause**: Database connection issues
- **Solution**: Verify PostgreSQL service is healthy

#### 3. Code Formatting Errors
- **Cause**: Code style violations
- **Solution**: Run `dotnet format` locally before pushing

#### 4. Build Failures
- **Cause**: Compilation errors or missing dependencies
- **Solution**: Ensure `dotnet build` works locally

### Debugging Workflows

1. **Check Workflow Logs**: GitHub Actions > Failed workflow > Job logs
2. **Download Artifacts**: Test results and coverage reports
3. **Local Reproduction**: Run same commands locally
4. **Compare with Previous Runs**: Identify recent changes

## ⚡ Performance Optimization

### Workflow Speed Optimizations
- **Parallel Jobs**: Unit/Integration/E2E tests run in parallel
- **Caching**: NuGet packages and Playwright browsers cached
- **Conditional Steps**: E2E screenshots only on failure
- **Fast Feedback**: PR validation runs only unit tests

### Resource Usage
- **Build Job**: ~2-4 minutes
- **Unit Tests**: ~2-3 minutes  
- **Integration Tests**: ~5-7 minutes
- **E2E Tests**: ~8-10 minutes

## 📈 Metrics & Monitoring

### Success Metrics
- **Test Pass Rate**: Target >95%
- **Build Success Rate**: Target >98%
- **Average Pipeline Duration**: <20 minutes
- **Code Coverage**: Target >80%

### Alerts & Notifications
- **Failed Tests**: Workflow fails on any test failure
- **Security Issues**: Nightly scan results
- **Performance Regression**: Manual performance test runs

## 🔄 Maintenance

### Regular Tasks
- **Weekly**: Review failed nightly tests
- **Monthly**: Update dependencies and tools
- **Quarterly**: Optimize workflow performance

### Updating Workflows
1. Test changes locally using `act` (GitHub Actions runner)
2. Update workflow files in `.github/workflows/`
3. Test with draft PR before merging

---

## 🎯 Best Practices

### For Developers
1. **Run tests locally** before pushing
2. **Fix formatting issues** with `dotnet format`
3. **Write tests** for new features
4. **Check PR validation** results before requesting review

### For Maintainers  
1. **Monitor nightly test failures** for regressions
2. **Keep dependencies updated** for security
3. **Review and optimize** workflow performance monthly
4. **Maintain test data** and fixtures for consistency

This comprehensive testing strategy ensures the SOMA platform maintains high quality and catches regressions early in the development process.