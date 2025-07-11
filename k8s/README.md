# Jitsi Meet Kubernetes Autoscaling Setup

This directory contains Kubernetes manifests for deploying Jitsi Meet with autoscaling capabilities. The setup includes horizontal pod autoscaling (HPA) for the Jitsi Video Bridge (JVB) and API components, along with comprehensive monitoring using Prometheus and Grafana.

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Load Balancer │    │   Ingress       │    │   Monitoring    │
│   (External)    │    │   Controller    │    │   Stack         │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Jitsi Web     │    │   Jitsi API     │    │   Prometheus    │
│   (HPA: 2-5)    │    │   (HPA: 2-5)    │    │   + Grafana     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   JVB           │    │   PostgreSQL    │    │   OpenTelemetry │
│   (HPA: 2-10)   │    │   Database      │    │   Collector     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │
         ▼
┌─────────────────┐
│   Prosody       │
│   + Jicofo      │
└─────────────────┘
```

## 📋 Prerequisites

### For Windows Development:
1. **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
2. **Minikube** - [Installation guide](https://minikube.sigs.k8s.io/docs/start/)
3. **kubectl** - [Installation guide](https://kubernetes.io/docs/tasks/tools/)
4. **PowerShell** (Windows)

### For Ubuntu Production:
1. **Docker** - `sudo apt-get install docker.io`
2. **kubectl** - [Installation guide](https://kubernetes.io/docs/tasks/tools/)
3. **Kubernetes cluster** (EKS, GKE, AKS, or self-hosted)

## 🚀 Quick Start (Windows)

### 1. Install Prerequisites
```powershell
# Install Chocolatey (if not installed)
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install required tools
choco install minikube kubernetes-cli docker-desktop
```

### 2. Start Minikube
```powershell
# Start minikube with sufficient resources
minikube start --cpus 4 --memory 8192 --disk-size 20g

# Enable required addons
minikube addons enable ingress
minikube addons enable metrics-server
minikube addons enable storage-provisioner
```

### 3. Deploy Jitsi Meet
```powershell
# Navigate to your project directory
cd /path/to/your/jitsi/project

# Run the deployment script
.\k8s\deploy.ps1

# Or deploy with custom options
.\k8s\deploy.ps1 -Environment local -SkipBuild:$false -SkipStorage:$false
```

### 4. Access Services
After deployment, you'll get URLs for:
- **API**: `http://<minikube-ip>:<port>`
- **Web Interface**: `http://<minikube-ip>:<port>`
- **Grafana**: `http://<minikube-ip>:<port>` (admin/admin)
- **Prometheus**: `http://<minikube-ip>:<port>`

## 🛑 Stopping and Cleanup

### Stop the Deployment
```powershell
# Remove the deployment
.\k8s\cleanup.ps1

# Remove everything including storage
.\k8s\cleanup.ps1 -All

# Keep storage data
.\k8s\cleanup.ps1 -KeepStorage
```

### Stop Minikube
```powershell
minikube stop
```

## 📊 Monitoring and Autoscaling

### Check Autoscaling Status
```powershell
# View HPA status
kubectl get hpa -n jitsi-meet

# Describe specific HPA
kubectl describe hpa jvb-hpa -n jitsi-meet
kubectl describe hpa jitsi-api-hpa -n jitsi-meet
```

### Monitor Resources
```powershell
# View pod status
kubectl get pods -n jitsi-meet

# View resource usage
kubectl top pods -n jitsi-meet

# View logs
kubectl logs -f deployment/jvb -n jitsi-meet
kubectl logs -f deployment/jitsi-api -n jitsi-meet
```

### Access Monitoring Dashboards
1. **Grafana**: Access via the provided URL
   - Username: `admin`
   - Password: `admin`
   - Dashboards: Jitsi Meet Overview, Resource Usage

2. **Prometheus**: Access via the provided URL
   - Query metrics: `jitsi_meetings_total`, `jitsi_participants_total`
   - View targets and alerts

## 🔧 Configuration

### Autoscaling Parameters
The HPA configurations are set to:
- **JVB**: 2-10 replicas, CPU 70%, Memory 80%
- **API**: 2-5 replicas, CPU 70%, Memory 80%
- **Web**: 2-5 replicas, CPU 70%, Memory 80%

### Customize Configuration
Edit the following files to customize your setup:
- `k8s/configmaps.yaml` - Application configuration
- `k8s/secrets.yaml` - Secrets (update with your values)
- `k8s/jvb-deployment.yaml` - JVB autoscaling settings
- `k8s/api-deployment.yaml` - API autoscaling settings

## 🌐 Production Deployment (Ubuntu)

### 1. Prepare Your Cluster
```bash
# Ensure you have a Kubernetes cluster running
kubectl cluster-info

# Install metrics-server (if not present)
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml
```

### 2. Build and Push Images
```bash
# Build your API image
docker build -t your-registry/jitsiappointmentapi:latest ./JitsiAppointmentApi

# Push to your registry
docker push your-registry/jitsiappointmentapi:latest
```

### 3. Update Configuration
```bash
# Update image references in k8s files
sed -i 's|jitsiappointmentapi:latest|your-registry/jitsiappointmentapi:latest|g' k8s/*.yaml

# Update domain names in configmaps
sed -i 's|yourdomain.com|your-actual-domain.com|g' k8s/configmaps.yaml
```

### 4. Deploy
```bash
# Apply all resources
kubectl apply -f k8s/

# Or use kustomize
kubectl apply -k k8s/
```

## 🔍 Troubleshooting

### Common Issues

1. **Pods not starting**
   ```powershell
   kubectl describe pod <pod-name> -n jitsi-meet
   kubectl logs <pod-name> -n jitsi-meet
   ```

2. **HPA not working**
   ```powershell
   kubectl describe hpa <hpa-name> -n jitsi-meet
   kubectl top pods -n jitsi-meet
   ```

3. **Storage issues**
   ```powershell
   kubectl get pv,pvc -n jitsi-meet
   kubectl describe pvc <pvc-name> -n jitsi-meet
   ```

4. **Network connectivity**
   ```powershell
   kubectl get svc -n jitsi-meet
   kubectl describe svc <service-name> -n jitsi-meet
   ```

### Performance Tuning

1. **Increase resources**:
   - Edit resource limits in deployment files
   - Adjust HPA thresholds

2. **Optimize autoscaling**:
   - Modify `behavior` settings in HPA
   - Adjust `stabilizationWindowSeconds`

3. **Monitor bottlenecks**:
   - Use Grafana dashboards
   - Check Prometheus metrics

## 📈 Scaling Behavior

The autoscaling is configured with the following behavior:

### Scale Up
- **Stabilization Window**: 60 seconds
- **Policy**: 100% increase every 15 seconds
- **Triggers**: CPU > 70% OR Memory > 80%

### Scale Down
- **Stabilization Window**: 300 seconds (5 minutes)
- **Policy**: 10% decrease every 60 seconds
- **Cooldown**: Prevents rapid scaling down

## 🔐 Security Considerations

1. **Update secrets** in `k8s/secrets.yaml` with strong passwords
2. **Use HTTPS** in production (configure TLS certificates)
3. **Network policies** can be added for additional security
4. **RBAC** can be configured for fine-grained access control

## 📚 Additional Resources

- [Jitsi Meet Documentation](https://jitsi.github.io/handbook/)
- [Kubernetes HPA Documentation](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)

## 🤝 Support

For issues and questions:
1. Check the troubleshooting section above
2. Review Kubernetes and Jitsi logs
3. Consult the official documentation
4. Open an issue in the project repository 