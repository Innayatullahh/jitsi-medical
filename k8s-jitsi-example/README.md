# Jitsi Meet + API: Local Development and Production Deployment

This project demonstrates how to run Jitsi Meet and your API both locally (for development) and on a live server (for production) using Docker Compose and Kubernetes.

---

## Local Development (Docker Compose)

1. **Edit your `.env` files** for local configuration.

2. **Run all services locally:**
   ```sh
   docker-compose up
   ```
   This will start Jitsi Meet, your API, and any other services defined in your `docker-compose.yml` and `docker-compose.override.yml`.

3. **Access your services:**
   - Jitsi Meet: [https://localhost:8443](https://localhost:8443) (or as configured)
   - API: [http://localhost:YOUR_API_PORT](http://localhost:YOUR_API_PORT)

---

## Production Deployment (Kubernetes)

### 1. Build and Push Images

- Build your API and Jitsi images:
  ```sh
  docker build -t <your-registry>/jitsi-appointment-api:latest JitsiAppointmentApi/
  # (Repeat for other services as needed)
  ```
- Push to your container registry.

### 2. Deploy Jitsi Meet

- Follow the instructions in this README for Helm-based Jitsi Meet deployment.

### 3. Deploy Your API

- Apply the manifests:
  ```sh
  kubectl apply -f k8s-jitsi-example/api-deployment.yaml
  kubectl apply -f k8s-jitsi-example/api-service.yaml
  ```
- (Optional) Create an Ingress for your API if you want to expose it via a domain.

### 4. Configuration

- Use Kubernetes ConfigMaps and Secrets for environment variables and sensitive data.
- Ensure your API and Jitsi Meet use the correct URLs and credentials for production.

---

## Keeping Environments in Sync

- Use the same Dockerfiles for both environments.
- Parameterize all environment-specific values (URLs, secrets, ports).
- Document the mapping between Compose services and Kubernetes resources.
- Use a CI/CD pipeline to build, test, and deploy images.

---

## Summary Table

| Environment | Orchestration   | Config         | Access                | Deployment         |
|-------------|----------------|----------------|-----------------------|--------------------|
| Localhost   | Docker Compose | .env files     | localhost/127.0.0.1   | `docker-compose up`|
| Production  | Kubernetes     | ConfigMap/Secret| Domain/Ingress        | `kubectl apply`/Helm|

---

## Files

- `docker-compose.yml`, `docker-compose.override.yml` - Local dev
- `k8s-jitsi-example/api-deployment.yaml` - API Deployment (K8s)
- `k8s-jitsi-example/api-service.yaml` - API Service (K8s)
- `k8s-jitsi-example/jitsi-values.yaml` - Jitsi Meet Helm values
- Other manifests for Jitsi Meet, autoscaling, and TLS

---

## References

- [Jitsi Helm Chart](https://github.com/jitsi-contrib/jitsi-helm)
- [Jitsi Kubernetes Guide](https://jitsi.github.io/handbook/docs/devops-guide/devops-guide-kubernetes)
- [Kubernetes HPA](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)
