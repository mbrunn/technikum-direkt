#!/bin/bash

set -e
run_cmd="dotnet run --server.urls http://*:5000"

#export PATH="$PATH:$HOME/.dotnet/tools/"

until dotnet ef database update -p ./DataAccess/TechnikumDirekt.DataAccess.Sql/TechnikumDirekt.DataAccess.Sql.csproj -s ./Services/TechnikumDirekt.Services/TechnikumDirekt.Services.csproj; do
>&2 echo "SQL Server is starting up. Applying EF Core migrations."
sleep 1
done

>&2 echo "SQL Server is up - Starting application"
exec $run_cmd