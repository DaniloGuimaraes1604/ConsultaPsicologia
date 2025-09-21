# Estágio 1: Compilação da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências primeiro para aproveitar o cache do Docker
COPY ["ConsultasPsicologiaMVC/ConsultasPsicologiaMVC.csproj", "ConsultasPsicologiaMVC/"]
RUN dotnet restore "ConsultasPsicologiaMVC/ConsultasPsicologiaMVC.csproj"

# Copia o restante do código-fonte
COPY . .
WORKDIR "/src/ConsultasPsicologiaMVC"

# Compila e publica a aplicação
RUN dotnet publish "ConsultasPsicologiaMVC.csproj" -c Release -o /app/publish

# Estágio 2: Imagem final de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ponto de entrada para o container
ENTRYPOINT ["dotnet", "ConsultasPsicologiaMVC.dll"]
