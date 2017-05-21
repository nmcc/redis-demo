This repo contains some demo code for Redis using .NET Core 1.1

# Set-up the stage

1. Start the Redis container: ` docker-compose up -d`
1. Open a shell on the container: `docker exec -it redisdemo_redis-demo_1 bash`
1. Open redis CLI on the container shell: `redis-cli`

# Running the demo programs
To run the demo programs, it is required to have .NET Core 1.1 installed on your computer.

1. `cd <demo-project>`
1. `dotnet restore`
1. `dotnet run`
