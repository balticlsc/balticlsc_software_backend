param (
    [string]$mode="localhost"
)

invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-server"

docker pull balticlsc/balticfrontend:latest

docker stack deploy --compose-file="./baltic-yamls-$mode/balticlsc-front.yml" balticlsc-front