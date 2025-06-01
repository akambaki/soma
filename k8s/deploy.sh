#!/bin/bash

# SOMA Platform Kubernetes Deployment Script
# This script builds Docker images and deploys the SOMA platform to a local Kubernetes cluster

set -e

echo "🚀 Starting SOMA Platform Kubernetes Deployment"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "🔍 Checking prerequisites..."
if ! command_exists docker; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command_exists kubectl; then
    echo "❌ kubectl is not installed. Please install kubectl first."
    exit 1
fi

# Check if Kubernetes cluster is running
if ! kubectl cluster-info >/dev/null 2>&1; then
    echo "❌ Kubernetes cluster is not accessible. Please ensure your cluster is running."
    exit 1
fi

echo "✅ Prerequisites check passed"

# Build Docker images
echo "🐳 Building Docker images..."

echo "Building API image..."
docker build -t soma-api:latest -f Soma.Platform.Api/Dockerfile .

echo "Building Web image..."
docker build -t soma-web:latest -f Soma.Platform.Web/Dockerfile .

echo "Building Migration image..."
docker build -t soma-migrate:latest -f Dockerfile.migrate .

echo "Building Seeding image..."
docker build -t soma-seed:latest -f Dockerfile.seed .

echo "✅ Docker images built successfully"

# Create local storage directory for Docker Desktop
echo "📂 Creating local storage directories..."
sudo mkdir -p /tmp/soma-postgres
sudo chmod 777 /tmp/soma-postgres

# Deploy to Kubernetes
echo "☸️ Deploying to Kubernetes..."

# Apply all manifests in order
kubectl apply -f k8s/00-namespace-storage.yaml
echo "⏳ Waiting for namespace to be created..."
sleep 5

kubectl apply -f k8s/00-postgres-deployment.yaml
echo "⏳ Waiting for PostgreSQL to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/soma-postgres -n soma-platform

kubectl apply -f k8s/01-api-deployment.yaml
kubectl apply -f k8s/02-web-deployment.yaml

echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/soma-api -n soma-platform
kubectl wait --for=condition=available --timeout=300s deployment/soma-web -n soma-platform

# Get service URLs
echo "🌐 Service Information:"
echo "=================================="
kubectl get services -n soma-platform

# Get pod status
echo ""
echo "📦 Pod Status:"
echo "=================================="
kubectl get pods -n soma-platform

# Show access information
echo ""
echo "🎉 Deployment completed successfully!"
echo "=================================="
echo "Frontend (Blazor Web): http://localhost:30080"
echo "Backend API: Available via ClusterIP (accessible from frontend)"
echo "PostgreSQL Database: Running with persistent storage"
echo ""
echo "Default admin account:"
echo "  Email: admin@soma.com"
echo "  Password: Admin123!"
echo ""
echo "Useful commands:"
echo "  - View API logs: kubectl logs -f deployment/soma-api -n soma-platform"
echo "  - View Web logs: kubectl logs -f deployment/soma-web -n soma-platform"
echo "  - View PostgreSQL logs: kubectl logs -f deployment/soma-postgres -n soma-platform"
echo "  - Delete deployment: kubectl delete namespace soma-platform"
echo "  - Port forward API: kubectl port-forward service/soma-api-service 7073:8080 -n soma-platform"
echo "  - Port forward PostgreSQL: kubectl port-forward service/soma-postgres-service 5432:5432 -n soma-platform"