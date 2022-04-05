$currPath = pwd

cd ../Baltic.Server
docker build -f Baltic.Server.Development.dockerfile -t balticlsc/balticlsc-server:local .
cd $currPath

cd ../Baltic.Node/
docker build -f Baltic.Node.Development.dockerfile -t balticlsc/balticlsc-node:local .
cd $currPath

cd ../Baltic.Node.ClusterProxy.Swarm
docker build -f Baltic.Node.ClusterProxy.Swarm.Development.dockerfile -t balticlsc/balticlsc-cluster-proxy-swarm:local .
cd $currPath