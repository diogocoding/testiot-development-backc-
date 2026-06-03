# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AccessControlAPI.csproj", "./"]
RUN dotnet restore "AccessControlAPI.csproj"
COPY . .
RUN dotnet publish "AccessControlAPI.csproj" -c Release -o /app/publish

# Estágio de Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AccessControlAPI.dll"]