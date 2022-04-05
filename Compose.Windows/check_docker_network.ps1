param (
    [Parameter(Mandatory=$true)][string]$networkName
)

if (docker network ls | select-string -Pattern $networkName)
{
    Write-Output "docker network $networkName exists -> moving on"
} else
{
    Write-Output "docker network $networkName does not exists -> creating network"
    docker network create -d overlay $networkName --label com.docker.stack.namespace=$networkName --attachable
}
