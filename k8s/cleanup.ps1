# Jitsi Meet Kubernetes Cleanup Script for Windows
# This script removes the Jitsi Meet deployment from Kubernetes

param(
    [switch]$All = $false,
    [switch]$KeepStorage = $false
)

Write-Host "🧹 Starting Jitsi Meet Kubernetes Cleanup..." -ForegroundColor Yellow

# Check if kubectl is installed
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "❌ kubectl is not installed." -ForegroundColor Red
    exit 1
}

# Delete all resources in the jitsi-meet namespace
Write-Host "🗑️  Deleting Jitsi Meet resources..." -ForegroundColor Cyan

# Delete deployments
Write-Host "📦 Deleting deployments..." -ForegroundColor Yellow
kubectl delete deployment --all -n jitsi-meet --ignore-not-found=true

# Delete services
Write-Host "🔗 Deleting services..." -ForegroundColor Yellow
kubectl delete service --all -n jitsi-meet --ignore-not-found=true

# Delete ingress
Write-Host "🌐 Deleting ingress..." -ForegroundColor Yellow
kubectl delete ingress --all -n jitsi-meet --ignore-not-found=true

# Delete HPA
Write-Host "📈 Deleting Horizontal Pod Autoscalers..." -ForegroundColor Yellow
kubectl delete hpa --all -n jitsi-meet --ignore-not-found=true

# Delete configmaps
Write-Host "⚙️  Deleting configmaps..." -ForegroundColor Yellow
kubectl delete configmap --all -n jitsi-meet --ignore-not-found=true

# Delete secrets
Write-Host "🔐 Deleting secrets..." -ForegroundColor Yellow
kubectl delete secret --all -n jitsi-meet --ignore-not-found=true

# Delete PVCs (unless KeepStorage is specified)
if (-not $KeepStorage) {
    Write-Host "💾 Deleting persistent volume claims..." -ForegroundColor Yellow
    kubectl delete pvc --all -n jitsi-meet --ignore-not-found=true
    
    # Delete persistent volumes
    Write-Host "💾 Deleting persistent volumes..." -ForegroundColor Yellow
    kubectl delete pv prometheus-pv grafana-pv postgres-pv --ignore-not-found=true
    
    # Delete storage class
    Write-Host "💾 Deleting storage class..." -ForegroundColor Yellow
    kubectl delete storageclass local-storage --ignore-not-found=true
}

# Delete namespace
Write-Host "🏷️  Deleting namespace..." -ForegroundColor Yellow
kubectl delete namespace jitsi-meet --ignore-not-found=true

# Clean up Docker images (if All is specified)
if ($All) {
    Write-Host "🐳 Cleaning up Docker images..." -ForegroundColor Yellow
    docker rmi jitsiappointmentapi:latest --force 2>$null
    docker system prune -f
}

# Stop minikube (if All is specified)
if ($All) {
    Write-Host "🛑 Stopping minikube..." -ForegroundColor Yellow
    minikube stop
}

Write-Host "✅ Cleanup completed successfully!" -ForegroundColor Green

if ($KeepStorage) {
    Write-Host "💡 Storage data has been preserved. Use -KeepStorage:$false to remove it." -ForegroundColor Yellow
}

Write-Host "🎉 Jitsi Meet deployment has been removed!" -ForegroundColor Green 