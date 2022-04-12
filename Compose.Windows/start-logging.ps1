param (
    [string]$mode="localhost"
)

invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-server"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-node"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-logging"

invoke-expression -Command "./build-fluentd.ps1"

docker stack deploy --compose-file="./baltic-yamls-$mode/balticlsc-logging.yml" balticlsc-logging