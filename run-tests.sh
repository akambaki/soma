#!/bin/bash

# SOMA Platform Test Runner
# This script runs all tests in the SOMA platform test suite

echo "🧪 SOMA Platform - Comprehensive Test Suite"
echo "============================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test results
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to run test project and capture results
run_test_project() {
    local project_path=$1
    local project_name=$2
    
    echo -e "${YELLOW}Running ${project_name}...${NC}"
    
    # Run tests and capture output
    test_output=$(dotnet test "$project_path" --verbosity normal 2>&1)
    test_exit_code=$?
    
    # Extract test results
    if [[ $test_output =~ Total\ tests:\ ([0-9]+) ]]; then
        local project_total=${BASH_REMATCH[1]}
        TOTAL_TESTS=$((TOTAL_TESTS + project_total))
    fi
    
    if [[ $test_output =~ Passed:\ ([0-9]+) ]]; then
        local project_passed=${BASH_REMATCH[1]}
        PASSED_TESTS=$((PASSED_TESTS + project_passed))
    fi
    
    if [[ $test_output =~ Failed:\ ([0-9]+) ]]; then
        local project_failed=${BASH_REMATCH[1]}
        FAILED_TESTS=$((FAILED_TESTS + project_failed))
    fi
    
    if [ $test_exit_code -eq 0 ]; then
        echo -e "${GREEN}✅ ${project_name}: ALL TESTS PASSED${NC}"
    else
        echo -e "${RED}❌ ${project_name}: SOME TESTS FAILED${NC}"
        echo "Failed test output:"
        echo "$test_output" | grep -A 5 -B 5 "FAIL\|Error Message"
    fi
    echo ""
}

# Build all projects first
echo "🔨 Building all projects..."
dotnet build --verbosity quiet
if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed! Cannot run tests.${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Build successful${NC}"
echo ""

# Run all test projects
echo "🏃 Running all test suites..."
echo ""

# Unit Tests
echo "📋 Unit Tests"
echo "--------------"
run_test_project "tests/Soma.Platform.Core.Tests" "Core Unit Tests"
run_test_project "tests/Soma.Platform.Api.Tests" "API Unit Tests"
run_test_project "tests/Soma.Platform.Web.Tests" "Web Unit Tests"

# Integration Tests
echo "🔗 Integration Tests"
echo "--------------------"
run_test_project "tests/Soma.Platform.Api.IntegrationTests" "API Integration Tests"
run_test_project "tests/Soma.Platform.Web.IntegrationTests" "Web Integration Tests"

# E2E Tests (these might fail in CI without browser setup)
echo "🌐 End-to-End Tests"
echo "-------------------"
echo -e "${YELLOW}Note: E2E tests require Playwright browsers to be installed${NC}"
echo -e "${YELLOW}Run 'dotnet playwright install chromium' to enable E2E tests${NC}"
run_test_project "tests/Soma.Platform.E2E.Tests" "E2E Tests"

# Summary
echo "📊 Test Summary"
echo "==============="
echo "Total Tests: $TOTAL_TESTS"
echo -e "Passed: ${GREEN}$PASSED_TESTS${NC}"
echo -e "Failed: ${RED}$FAILED_TESTS${NC}"

if [ $FAILED_TESTS -eq 0 ]; then
    echo ""
    echo -e "${GREEN}🎉 ALL TESTS PASSED!${NC}"
    echo -e "${GREEN}The SOMA platform is working correctly.${NC}"
    exit 0
else
    echo ""
    echo -e "${RED}❌ SOME TESTS FAILED${NC}"
    echo -e "${RED}Please review the failed tests above.${NC}"
    exit 1
fi