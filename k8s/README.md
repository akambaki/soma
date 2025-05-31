# SOMA Platform - Kubernetes Deployment

This directory contains Kubernetes manifests and deployment scripts to run the SOMA Platform locally with frontend, backend, and database.

## Architecture

The SOMA Platform consists of three main components:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Blazor Web    │    │   Web API       │    │   SQLite DB     │
│   (Frontend)    │◄──►│   (Backend)     │◄──►│   (Storage)     │
│   Port: 30080   │    │   Port: 8080    │    │   Volume Mount  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Prerequisites

- **Docker**: For building container images
- **Kubernetes**: Local cluster (Docker Desktop, minikube, or kind)
- **kubectl**: Kubernetes command-line tool

## Quick Start

1. **Ensure Kubernetes is running**:
   ```bash
   kubectl cluster-info
   ```

2. **Deploy the application**:
   ```bash
   cd k8s
   ./deploy.sh
   ```

3. **Access the application**:
   - Frontend: http://localhost:30080
   - The backend API is accessible internally via ClusterIP

## Manual Deployment

If you prefer to deploy manually:

```bash
# Build Docker images
docker build -t soma-api:latest -f Soma.Platform.Api/Dockerfile .
docker build -t soma-web:latest -f Soma.Platform.Web/Dockerfile .

# Deploy to Kubernetes
kubectl apply -f k8s/00-namespace-storage.yaml
kubectl apply -f k8s/01-api-deployment.yaml
kubectl apply -f k8s/02-web-deployment.yaml

# Wait for deployments
kubectl wait --for=condition=available --timeout=300s deployment/soma-api -n soma-platform
kubectl wait --for=condition=available --timeout=300s deployment/soma-web -n soma-platform
```

## Configuration

### Environment Variables

The application uses ConfigMaps for configuration:

**API Configuration (`soma-api-config`)**:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Key`: JWT signing key
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience

**Web Configuration (`soma-web-config`)**:
- `ApiService__BaseUrl`: Backend API URL (internal service)

### Storage

- **SQLite Database**: Persisted using a PersistentVolume mounted at `/tmp/soma-db`
- **Storage Class**: `local-storage` for development
- **Volume Size**: 1Gi

## Services

### API Service (`soma-api-service`)
- **Type**: ClusterIP (internal only)
- **Port**: 8080
- **Endpoints**:
  - `/health` - Health check
  - `/api/auth/*` - Authentication endpoints
  - `/swagger` - API documentation (development)

### Web Service (`soma-web-service`)
- **Type**: NodePort (external access)
- **Port**: 8080
- **NodePort**: 30080
- **Access**: http://localhost:30080

## Monitoring and Debugging

### View Application Status
```bash
# Check pod status
kubectl get pods -n soma-platform

# Check service status
kubectl get services -n soma-platform

# Check persistent volumes
kubectl get pv,pvc -n soma-platform
```

### View Logs
```bash
# API logs
kubectl logs -f deployment/soma-api -n soma-platform

# Web logs
kubectl logs -f deployment/soma-web -n soma-platform
```

### Port Forwarding (Optional)
```bash
# Access API directly
kubectl port-forward service/soma-api-service 7073:8080 -n soma-platform

# Access Web directly (alternative to NodePort)
kubectl port-forward service/soma-web-service 5001:8080 -n soma-platform
```

## Health Checks

The deployments include liveness and readiness probes:

- **API Health**: `GET /health`
- **Web Health**: `GET /` (root page)

## Cleanup

To remove the entire deployment:

```bash
kubectl delete namespace soma-platform
```

This will remove all resources including:
- Deployments
- Services
- ConfigMaps
- PersistentVolumeClaims
- The namespace itself

Note: The PersistentVolume may need to be manually deleted:
```bash
kubectl delete pv soma-db-pv
```

## Troubleshooting

### Common Issues

1. **Images not found**: Ensure Docker images are built locally
   ```bash
   docker images | grep soma
   ```

2. **Persistent Volume issues**: Check if the local path exists
   ```bash
   ls -la /tmp/soma-db/
   ```

3. **Database connection issues**: Check API logs for connection errors
   ```bash
   kubectl logs deployment/soma-api -n soma-platform
   ```

4. **Service communication**: Verify services are running
   ```bash
   kubectl get endpoints -n soma-platform
   ```

### Resource Requirements

**Minimum Requirements**:
- CPU: 500m (0.5 cores)
- Memory: 512Mi
- Storage: 1Gi

**Recommended for Development**:
- CPU: 1 core
- Memory: 1Gi
- Storage: 2Gi

## Security Notes

- JWT keys are stored in ConfigMaps (use Secrets in production)
- SQLite is used for simplicity (use external database in production)
- No TLS/HTTPS configured (enable for production)
- Default passwords and keys (change for production)