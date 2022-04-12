param (
    [string]$mode="localhost"
)

invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-server"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-node"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-batch-manager"

docker pull balticlsc/balticlsc-node:latest
docker pull balticlsc/balticlsc-cluster-proxy-swarm:latest

docker stack deploy --compose-file="./baltic-yamls-$mode/balticlsc-node-main.yml" balticlsc-node