# Jitsi Meet Kubernetes Deployment Script for Windows
param(
    [string]$Environment = "local",
    [switch]$SkipBuild = $false,
    [switch]$SkipStorage = $false
)

Write-Host "Starting Jitsi Meet Kubernetes Deployment..." -ForegroundColor Green

# Check if kubectl is installed
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) 
{
    Write-Host "kubectl is not installed. Please install kubectl first." -ForegroundColor Red
    exit 1
}

# Check if minikube is installed
if (-not (Get-Command minikube -ErrorAction SilentlyContinue)) 
{
    Write-Host "minikube is not installed. Please install it from https://minikube.sigs.k8s.io/docs/start/" -ForegroundColor Red
    exit 1
}

# Check if minikube is running (for local development)
if ($Environment -eq "local") 
{
    Write-Host "Checking minikube status..." -ForegroundColor Yellow

    $isRunning = $false
    try {
        $minikubeJson = minikube status --output=json 2>$null
        $minikubeStatus = $minikubeJson | ConvertFrom-Json
        $isRunning = $minikubeStatus.Host -eq "Running"
    } catch {
        $isRunning = $false
    }

    if (-not $isRunning) {
        Write-Host "Minikube is not running or not initialized. Starting minikube..." -ForegroundColor Yellow
        minikube start --cpus 4 --memory 8192 --disk-size 20g
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to start minikube" -ForegroundColor Red
            exit 1
        }
    }

    # Enable essential minikube addons
    Write-Host "Enabling minikube addons..." -ForegroundColor Yellow
    minikube addons enable ingress
    minikube addons enable metrics-server
    minikube addons enable storage-provisioner
}

# Build and push Docker images (skip if --SkipBuild is specified)
if (-not $SkipBuild) 
{
    Write-Host "Building Docker images..." -ForegroundColor Yellow

    # Build the API image
    Write-Host "Building JitsiAppointmentApi image..." -ForegroundColor Cyan
    docker build -t jitsiappointmentapi:latest ./JitsiAppointmentApi
    if ($LASTEXITCODE -ne 0) 
    {
        Write-Host "Failed to build API image" -ForegroundColor Red
        exit 1
    }

    # Load image into minikube (for local development)
    if ($Environment -eq "local") 
    {
        Write-Host "Loading image into minikube..." -ForegroundColor Cyan
        minikube image load jitsiappointmentapi:latest
    }
}

# Create storage directories (skip if --SkipStorage is specified)
if (-not $SkipStorage -and $Environment -eq "local") 
{
    Write-Host "Setting up storage directories..." -ForegroundColor Yellow

    $directories = @("/mnt/data/prometheus", "/mnt/data/grafana", "/mnt/data/postgres")
    foreach ($dir in $directories) {
        Write-Host "Creating directory: $dir" -ForegroundColor Cyan
        minikube ssh "sudo mkdir -p $dir; sudo chmod 777 $dir"
        if ($LASTEXITCODE -ne 0) 
        {
            Write-Host "Failed to create directory $dir" -ForegroundColor Red
            exit 1
        }
    }
}

# Apply Kubernetes resources
Write-Host "Applying Kubernetes resources..." -ForegroundColor Yellow

kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/storage.yaml
kubectl apply -f k8s/secrets.yaml
kubectl apply -f k8s/configmaps.yaml

# Deploy Postgres and wait until ready
kubectl apply -f k8s/postgres-deployment.yaml
Write-Host "Waiting for database to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=postgres -n jitsi-meet --timeout=300s

# Deploy Jitsi components
kubectl apply -f k8s/prosody-deployment.yaml
kubectl apply -f k8s/jicofo-deployment.yaml
kubectl apply -f k8s/jvb-deployment.yaml
kubectl apply -f k8s/web-deployment.yaml
kubectl apply -f k8s/api-deployment.yaml

# Deploy monitoring stack
kubectl apply -f k8s/monitoring.yaml

# Deploy ingress controller config
kubectl apply -f k8s/ingress.yaml

# Wait for all pods to be ready
Write-Host "Waiting for all pods to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=jitsi-meet -n jitsi-meet --timeout=600s

# Output service URLs (only for local)
if ($Environment -eq "local") 
{
    $apiUrl = minikube service jitsi-api-external -n jitsi-meet --url
    $webUrl = minikube service jitsi-web-external -n jitsi-meet --url
    $grafanaUrl = minikube service grafana-service -n jitsi-meet --url
    $prometheusUrl = minikube service prometheus-service -n jitsi-meet --url

    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Service URLs:" -ForegroundColor Cyan
    Write-Host "   API: $apiUrl" -ForegroundColor White
    Write-Host "   Web: $webUrl" -ForegroundColor White
    Write-Host "   Grafana: $grafanaUrl" -ForegroundColor White
    Write-Host "   Prometheus: $prometheusUrl" -ForegroundColor White
    Write-Host ""
    Write-Host "Grafana credentials:" -ForegroundColor Cyan
    Write-Host "   Username: admin" -ForegroundColor White
    Write-Host "   Password: admin" -ForegroundColor White
}
else 
{
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    Write-Host "Services are available via LoadBalancer IPs" -ForegroundColor Cyan
}

# Show status
Write-Host ""
kubectl get pods -n jitsi-meet

Write-Host ""
kubectl get services -n jitsi-meet

Write-Host ""
kubectl get hpa -n jitsi-meet

Write-Host ""
Write-Host "Jitsi Meet with autoscaling is now deployed!" -ForegroundColor Green
