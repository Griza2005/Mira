FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY MIRA.Api/MIRA.Api.csproj MIRA.Api/
RUN dotnet restore MIRA.Api/MIRA.Api.csproj

COPY . .
RUN dotnet build MIRA.Api/MIRA.Api.csproj -c Release -o /app/build
RUN dotnet publish MIRA.Api/MIRA.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MIRA.Api.dll"]
