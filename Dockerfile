### STAGE 1: Build ###
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY *.sln ./
COPY *.sh ./

ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install -g dotnet-ef

COPY Services/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p Services/${file%.*}/ && mv $file Services/${file%.*}/; done

COPY Services/TechnikumDirekt.Services/appsettings.json ./

COPY BusinessLogic/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p BusinessLogic/${file%.*}/ && mv $file BusinessLogic/${file%.*}/; done

COPY DataAccess/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p DataAccess/${file%.*}/ && mv $file DataAccess/${file%.*}/; done

COPY ServiceAgents/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ServiceAgents/${file%.*}/ && mv $file ServiceAgents/${file%.*}/; done

COPY IntegrationTests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p IntegrationTests/${file%.*}/ && mv $file IntegrationTests/${file%.*}/; done

RUN dotnet restore 

COPY . .
WORKDIR /src
RUN dotnet build "./Services/TechnikumDirekt.Services/TechnikumDirekt.Services.csproj" -c Release -o /app/build

### STAGE 2: Publish ###
FROM build AS publish
RUN dotnet publish "./Services/TechnikumDirekt.Services/TechnikumDirekt.Services.csproj" -c Release -o /app/publish

### STAGE 3: Apply EF Migrations ###
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

### Development ###
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechnikumDirekt.Services.dll"]