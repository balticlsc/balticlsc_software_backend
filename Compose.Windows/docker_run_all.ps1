#invoke-expression -Command "./docker_build_all.ps1"

invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-server"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-node"
invoke-expression -Command "./check_docker_network.ps1 -networkName balticlsc-batch-manager"

docker stack deploy --compose-file=./balticlsc-server-infrastructure.yml baltic-server
docker stack deploy --compose-file=./balticlsc-server-main.yml baltic-server
docker stack deploy --compose-file=./balticlsc-node-infrastructure.yml baltic-node
docker stack deploy --compose-file=./balticlsc-node-main.yml baltic-node
docker stack deploy --compose-file=./balticlsc-front-main.yml baltic-front