param (
    [string]$mode="localhost"
)

invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-server"

docker pull consul:latest
docker pull postgres:latest
docker pull dpage/pgadmin4:latest

docker stack deploy --compose-file="./baltic-yamls-$mode/balticlsc-server-infrastructure.yml" balticlsc-server