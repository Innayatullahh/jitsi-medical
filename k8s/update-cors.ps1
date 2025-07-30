# Update CORS Configuration on Server
param(
    [string]$Environment = "production",
    [switch]$SkipBuild = $false
)

Write-Host "Updating CORS configuration on server..." -ForegroundColor Green

# Check if kubectl is installed
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) 
{
    Write-Host "kubectl is not installed. Please install kubectl first." -ForegroundColor Red
    exit 1
}

# Build and push Docker images (skip if --SkipBuild is specified)
if (-not $SkipBuild) 
{
    Write-Host "Building and pushing Docker images..." -ForegroundColor Yellow

    # Build the API image
    Write-Host "Building JitsiAppointmentApi image..." -ForegroundColor Cyan
    docker build -t jitsiappointmentapi:latest ./JitsiAppointmentApi
    if ($LASTEXITCODE -ne 0) 
    {
        Write-Host "Failed to build API image" -ForegroundColor Red
        exit 1
    }

    # Tag and push to your registry (update with your registry)
    Write-Host "Tagging and pushing image..." -ForegroundColor Cyan
    docker tag jitsiappointmentapi:latest your-registry/jitsiappointmentapi:latest
    docker push your-registry/jitsiappointmentapi:latest
    if ($LASTEXITCODE -ne 0) 
    {
        Write-Host "Failed to push image" -ForegroundColor Red
        exit 1
    }
}

# Update Kubernetes resources
Write-Host "Updating Kubernetes resources..." -ForegroundColor Yellow

# Apply updated configmaps
kubectl apply -f k8s/configmaps.yaml
if ($LASTEXITCODE -ne 0) 
{
    Write-Host "Failed to apply configmaps" -ForegroundColor Red
    exit 1
}

# Apply updated API deployment
kubectl apply -f k8s/api-deployment.yaml
if ($LASTEXITCODE -ne 0) 
{
    Write-Host "Failed to apply API deployment" -ForegroundColor Red
    exit 1
}

# Restart the API deployment to pick up new configuration
Write-Host "Restarting API deployment..." -ForegroundColor Yellow
kubectl rollout restart deployment/jitsi-api -n jitsi-meet
if ($LASTEXITCODE -ne 0) 
{
    Write-Host "Failed to restart deployment" -ForegroundColor Red
    exit 1
}

# Wait for rollout to complete
Write-Host "Waiting for rollout to complete..." -ForegroundColor Yellow
kubectl rollout status deployment/jitsi-api -n jitsi-meet --timeout=300s
if ($LASTEXITCODE -ne 0) 
{
    Write-Host "Rollout failed or timed out" -ForegroundColor Red
    exit 1
}

Write-Host "CORS configuration updated successfully!" -ForegroundColor Green
Write-Host "API should now accept requests from configured origins." -ForegroundColor Cyan

# Show current status
Write-Host "`nCurrent API status:" -ForegroundColor Yellow
kubectl get pods -n jitsi-meet -l app=jitsi-api
kubectl get svc -n jitsi-meet -l app=jitsi-api 