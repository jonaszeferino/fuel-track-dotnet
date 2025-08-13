# Use a imagem oficial do .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto
COPY ["MeuProjetoBlazorServer.csproj", "./"]
RUN dotnet restore "MeuProjetoBlazorServer.csproj"

# Copia o resto do código
COPY . .
WORKDIR "/src/"

# Build da aplicação
RUN dotnet build "MeuProjetoBlazorServer.csproj" -c Release -o /app/build

# Publish da aplicação
FROM build AS publish
RUN dotnet publish "MeuProjetoBlazorServer.csproj" -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define a variável de ambiente para a porta
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "MeuProjetoBlazorServer.dll"]
