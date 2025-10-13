# some commands used during development
docker build -t mock-selenium .
docker build -t mock-selenium src/test/resources/vcmock

docker stop test-container
docker rm test-container
docker run --name test-container -d  mock-selenium

docker logs test-container
docker stop test-container
docker ps -a --filter "name=test-container"
docker ps -a --filter "name=test-container" --filter "status=exited"
docker inspect -f '{{.State.Status}}' test-container