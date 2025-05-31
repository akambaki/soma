#!/bin/bash

# SOMA Platform Kubernetes Cleanup Script
# This script removes all SOMA platform resources from Kubernetes

set -e

echo "🧹 Cleaning up SOMA Platform Kubernetes deployment"

# Check if kubectl is available
if ! command -v kubectl >/dev/null 2>&1; then
    echo "❌ kubectl is not installed. Please install kubectl first."
    exit 1
fi

# Check if namespace exists
if kubectl get namespace soma-platform >/dev/null 2>&1; then
    echo "🗑️ Removing namespace and all resources..."
    kubectl delete namespace soma-platform
    
    echo "⏳ Waiting for namespace deletion..."
    while kubectl get namespace soma-platform >/dev/null 2>&1; do
        sleep 2
    done
    
    echo "🧹 Cleaning up persistent volume..."
    if kubectl get pv soma-db-pv >/dev/null 2>&1; then
        kubectl delete pv soma-db-pv
    fi
    
    echo "📂 Cleaning up local storage directory..."
    if [ -d "/tmp/soma-db" ]; then
        sudo rm -rf /tmp/soma-db
    fi
    
    echo "✅ Cleanup completed successfully!"
else
    echo "ℹ️ SOMA Platform namespace not found - nothing to clean up."
fi

echo ""
echo "🐳 Docker images are still available. To remove them:"
echo "  docker rmi soma-api:latest soma-web:latest"