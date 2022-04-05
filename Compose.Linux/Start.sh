#!/bin/bash
sudo docker stack deploy -c portainer-agent-stack.yml portainer
#firewall-cmd --zone=public --add-port=5001/tcp --permanent
sudo systemctl start docker
#sudo docker network create --scope=swarm baltic
sudo docker stack deploy --compose-file=infrastructure.yml baltic 
sudo docker stack services baltic
sudo docker volume ls

