# SOMA Platform - Container Deployment Guide

This guide provides containerized deployment options for the SOMA Platform authentication system built with .NET 8 and Blazor.

## 🚀 Quick Start Options

### Option 1: Docker Compose (Recommended for Development)

```bash
# Clone and start all services
git clone https://github.com/akambaki/soma.git
cd soma
docker-compose up --build
```

**Access URLs:**
- Frontend (Blazor Web): http://localhost:5001
- Backend API: http://localhost:7073
- API Documentation: http://localhost:7073/swagger
- PostgreSQL Database: localhost:5432

**Default Admin Login:**
- Email: admin@soma.com
- Password: Admin123!

### Option 2: Kubernetes (Production-like Local Environment)

```bash
# Deploy to local Kubernetes cluster
cd k8s
./deploy.sh
```

**Access URLs:**
- Frontend: http://localhost:30080
- Health Checks: Automatic via probes
- Admin Login: admin@soma.com / Admin123!

**Clean up:**
```bash
cd k8s
./cleanup.sh
```

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Blazor Web    │    │   Web API       │    │  PostgreSQL DB  │
│   (Frontend)    │◄──►│   (Backend)     │◄──►│   (Storage)     │
│   Port: 30080   │    │   Port: 8080    │    │   Port: 5432    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │                        ▲
                                ▼                        │
                       ┌─────────────────┐      ┌─────────────────┐
                       │  Init Container │      │  Init Container │
                       │   (Migration)   │─────►│    (Seeding)    │
                       └─────────────────┘      └─────────────────┘
```

**Key Features:**
- **PostgreSQL Database**: Production-ready RDBMS with ACID compliance
- **Automatic Migrations**: EF Core migrations applied on startup
- **Data Seeding**: Default admin user and roles created automatically
- **Health Monitoring**: Comprehensive health checks for all services
- **Persistent Storage**: Database data survives container restarts

## 🔧 Configuration

### Container Environment Variables

**API Container:**
- `ConnectionStrings__DefaultConnection`: Database path
- `Jwt__Key`: JWT signing key  
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience
- `DOTNET_RUNNING_IN_CONTAINER=true`

**Web Container:**
- `ApiService__BaseUrl`: Backend API URL
- `DOTNET_RUNNING_IN_CONTAINER=true`

### Ports

- **Frontend**: 8080 (internal) → 30080 (NodePort) or 5001 (Docker Compose)
- **Backend**: 8080 (internal) → ClusterIP or 7073 (Docker Compose)

## 📁 Container Files

```
├── Soma.Platform.Api/Dockerfile    # Backend container
├── Soma.Platform.Web/Dockerfile    # Frontend container
├── docker-compose.yml              # Docker Compose config
└── k8s/                            # Kubernetes manifests
    ├── 00-namespace-storage.yaml   # Namespace and storage
    ├── 01-api-deployment.yaml      # API deployment & service
    ├── 02-web-deployment.yaml      # Web deployment & service
    ├── deploy.sh                   # Deployment script
    ├── cleanup.sh                  # Cleanup script
    └── README.md                   # Detailed K8s guide
```

## 🔍 Monitoring

### Health Checks

- **API Health**: `GET /health` (returns JSON status)
- **Web Health**: Root page availability
- **Database**: Automatic SQLite file creation

### Debugging

```bash
# Docker Compose logs
docker-compose logs -f soma-api
docker-compose logs -f soma-web

# Kubernetes logs
kubectl logs -f deployment/soma-api -n soma-platform
kubectl logs -f deployment/soma-web -n soma-platform
```

## 🔒 Security Notes

⚠️ **Development Configuration Only**

- JWT keys are in plaintext ConfigMaps
- HTTP traffic (no TLS)
- SQLite database (single file)
- Default secrets and passwords

For production deployment:
- Use Kubernetes Secrets for sensitive data
- Enable TLS/HTTPS with proper certificates
- Use external database (PostgreSQL/SQL Server)
- Configure proper authentication providers

## 📋 Prerequisites

### Docker Compose
- Docker Desktop or Docker Engine
- Docker Compose v3.8+

### Kubernetes
- Local Kubernetes cluster (Docker Desktop, minikube, kind)
- kubectl CLI tool
- Sufficient resources: 1 CPU, 1Gi RAM, 2Gi storage

## 🆘 Troubleshooting

### Common Issues

1. **Port conflicts**: Change ports in docker-compose.yml
2. **Permission denied**: Ensure Docker has proper permissions
3. **Images not found**: Run `docker-compose build` first
4. **Database issues**: Check volume mounts and permissions

### Clean Start

```bash
# Docker Compose - complete reset
docker-compose down -v
docker system prune -f
docker-compose up --build

# Kubernetes - complete reset
kubectl delete namespace soma-platform
kubectl delete pv soma-db-pv
./deploy.sh
```

For detailed Kubernetes deployment information, see [k8s/README.md](k8s/README.md).

## 🎯 Authentication Features

This deployment includes:

- ✅ User registration with email verification
- ✅ JWT-based authentication 
- ✅ Password strength validation
- ✅ Account lockout protection
- ✅ Two-factor authentication API
- ✅ User profile management
- ✅ Responsive Blazor UI