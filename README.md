# OpenShift Sample App: Joke Presentation + Joke Provider

![OpenShift](https://img.shields.io/badge/deployed%20on-OpenShift-red)
![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![License: MIT](https://img.shields.io/badge/license-MIT-green)


This repository contains a complete demo of a containerized .NET 8 application deployed to OpenShift using:

- ✅ Horizontal Pod Autoscaler (HPA)
- ✅ Persistent Volume (PVC)
- ✅ BuildConfig with Dockerfile from GitHub
- ✅ Internal service-to-service communication

---

## 🎯 Architecture Overview

This app is split into two .NET 8 components:

- **joke-provider**: A simple API that returns random jokes stored in a JSON file (`jokes.json`) backed by a Persistent Volume.
- **joke-presentation**: A web application that requests a joke from the API and displays it, styled with static assets (CSS, JS).

They are deployed independently and communicate through a Kubernetes service network.

---

## 🚀 How to Deploy on OpenShift

### 1. Clone this repository

```bash
git clone https://github.com/rzavalik/OpenShift.git
cd OpenShift
```

### 2. Log in to your OpenShift cluster and select your project

```bash
oc login --token=YOUR_TOKEN --server=https://api.YOUR_CLUSTER
oc project your-namespace
```

If you are using the developer sandbox, you can get your login command from here:

![Getting the login command from Developer Sandbox](https://github.com/user-attachments/assets/18e26fe0-c593-44d4-94e4-4cb46e967e74)


### 3. Deploy JokeProvider

```bash
# Persistent Volume Claim
oc apply -f openshift/jokeprovider/pvc.yaml

# Build and image stream
oc apply -f openshift/jokeprovider/imagestream.yaml
oc apply -f openshift/jokeprovider/buildconfig.yaml
oc start-build joke-provider --wait

# Service and deployment
oc apply -f openshift/jokeprovider/service.yaml
oc apply -f openshift/jokeprovider/deployment.yaml

# Horizontal Pod Autoscaler
oc apply -f openshift/jokeprovider/hpa.yaml
```

### 4. Deploy JokePresentation

```bash
# Build and image stream
oc apply -f openshift/jokepresentation/imagestream.yaml
oc apply -f openshift/jokepresentation/buildconfig.yaml
oc start-build joke-presentation --wait

# Service, deployment and route
oc apply -f openshift/jokepresentation/service.yaml
oc apply -f openshift/jokepresentation/deployment.yaml
oc apply -f openshift/jokepresentation/route.yaml

# Horizontal Pod Autoscaler
oc apply -f openshift/jokepresentation/hpa.yaml
```

### 5. Access the application

```bash
oc get route joke-presentation -o jsonpath='{.spec.host}'
```

Then open the URL:

```
http://<host-from-command>
```
You should be able to see the app running:
![Joke App Deployed](https://github.com/user-attachments/assets/93a02926-0c2f-474f-ba10-2f9ee7c89c7f)

The app shows you bellow the joke the container that has replied this request (API request to `joke-provider`), and in the footer the container that has replied the request to the page itselft (HTTP request to `joke-presentation`).

---

## 🧪 Troubleshooting

- Check build logs:
```bash
oc logs build/joke-provider-1
oc logs build/joke-presentation-1
```
- Check pod status:
```bash
oc get pods
```
- Check service-to-service communication:
```bash
oc exec -it <joke-presentation-pod> -- curl http://joke-provider:5000/api
```

---

## 📂 Project Structure

```text
openshift/
├── jokeprovider/
│   ├── buildconfig.yaml
│   ├── deployment.yaml
│   ├── hpa.yaml
│   ├── imagestream.yaml
│   ├── pvc.yaml
│   └── service.yaml
├── jokepresentation/
│   ├── buildconfig.yaml
│   ├── deployment.yaml
│   ├── hpa.yaml
│   ├── imagestream.yaml
│   ├── route.yaml
│   └── service.yaml
src/
├── JokeProvider/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Dockerfile
│   └── wwwroot/
├── JokePresentation/
    ├── Program.cs
    ├── Dockerfile
    ├── Controllers/
    ├── Views/
    └── wwwroot/
```

---

## 📜 License

MIT License

---

Created by [@rzavalik](https://github.com/rzavalik) as a sample OpenShift deployment with .NET 8, autoscaling, and persistent storage.
