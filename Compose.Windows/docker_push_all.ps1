param (
    [string]$imageTag="latest"
)

invoke-expression -Command ./docker_build_all.ps1

docker tag balticlsc/balticlsc-server:local balticlsc/balticlsc-server:$imageTag
docker tag balticlsc/balticlsc-node:local balticlsc/balticlsc-node:$imageTag
docker tag balticlsc/balticlsc-cluster-proxy-swarm:local balticlsc/balticlsc-cluster-proxy-swarm:$imageTag

docker push balticlsc/balticlsc-server:$imageTag
docker push balticlsc/balticlsc-node:$imageTag
docker push balticlsc/balticlsc-cluster-proxy-swarm:$imageTag