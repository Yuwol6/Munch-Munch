## Kubernetes Demonstration Commands
- scalability: adjust the number of containers to handle different volumes of clients without affecting
the running application/horizontal scaling
    - check existing pods by `kubectl get pods`
    - increase replicas of Recommendation API by `kubectl scale deployment/recommendation-api --replicas=4`
    - check by `kubectl get pods - w`
        - can stop by Ctrl+C

- reliability: the application consistently functions without failure even if some nodes or containers are
down
    - check existing pods by `kubectl get pods`
    - delete an existing pod by `kubectl delete pod <POD-NAME>
    - check another pod automatically respawn by `kubectl get pods -w` (or from another split screen terminal)

- rollout: the application can be updated without going down
    - check existing pods by `kubectl get pods`
    - do a rolling update with a demo by `kubectl set image deployment/frontend frontend=yuwol6/munchmunch-frontend:demo`
    - check the pods by `kubectl get pods -w` (or from another split screen terminal)

- rollback: the application can be rolled back without going down
    - check existing pods by `kubectl get pods`
    - do a rollback by `kubectl rollout undo deployment/frontend`
    - check the pods by `kubectl get pods -w` (or from another split screen terminal)

- load balancing
    - external load balancer by GCP from browser to frontend application, across the 2 frontend pods (replicas)
    - internal ClusterIP load balancers from frontend application to the backend microservices, and from
    backend microservices (History API & Voting API) to PostgreSQL DB deployed on the cluster.

- microservice architecture and containerisation
    - each microservice has their own container image and YAML file that was deployed on the K8s
    cluster, to be isolated in their own pods