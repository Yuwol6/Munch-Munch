## Deployment Instructions
- Create GKE Cluster with:
    - zonal
    - australia-southeast2-b
    - access using DNS (tick)
    - IPv4 addresses (don't tick)
    - COS image type
    - e2-standard-2 (2 vCPU, 1 core, 8 GB memory)
    - 30 GB boot disk size
    - 2 nodes
- connect to cluster via Cloud Shell Terminal
- press enter to execute premade command
    - this connects Google Cloud Shell to the GKE cluster, allowing management via kubectl

- create munchmunch directory and move into it by
    - `mkdir -p ~/munchmunch-k8s`
    - `cd ~/munchmunch-k8s`

- create K8s Secret named postgres-secret ==note: for actual deployment for public service, change
the DB password from compose.yaml and the Program.cs files of the APIs==
```
kubectl create secret generic postgres-secret \
  --from-literal=POSTGRES_USER=munchmunch \
  --from-literal=POSTGRES_PASSWORD=devpassword \
  --from-literal=POSTGRES_DB=munchmunch \
  --from-literal='CONNECTION_STRING=Host=postgres;Port=5432;Database=munchmunch;Username=munchmunch;Password=devpassword'
```

- upload yaml files using Google Cloud Shell GUI
- run the YAML files by
```
kubectl apply -f postgres.yaml
kubectl apply -f recommendation.yaml
kubectl apply -f history.yaml
kubectl apply -f voting.yaml
kubectl apply -f frontend.yaml
```

- can check PostgreSQL deployment/creation by `kubectl rollout status deployment/postgres`
- create DB tables
- `kubectl exec -it deployment/postgres -- psql -U munchmunch -d munchmunch`
```
CREATE TABLE IF NOT EXISTS recommendation_history (
    id BIGSERIAL PRIMARY KEY,
    user_id TEXT NOT NULL,
    food TEXT NOT NULL,
    recommended_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE IF NOT EXISTS votes (
    id BIGSERIAL PRIMARY KEY,
    user_id TEXT NOT NULL,
    food TEXT NOT NULL,
    voted_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```
- can check tables creation by `\dt`
- exit DB environment by `\q`

- verify all pods by `kubectl get pods`
    - should see 9 pods total
- verify services by `kubectl get services`
- use the EXTERNAL-IP of the frontend LoadBalancer to access MunchMunch web app at http://<EXTERNAL-IP>
