FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Soma.Platform.Web/Soma.Platform.Web.csproj", "Soma.Platform.Web/"]
COPY ["Soma.Platform.Core/Soma.Platform.Core.csproj", "Soma.Platform.Core/"]
RUN dotnet restore "Soma.Platform.Web/Soma.Platform.Web.csproj"
COPY . .
WORKDIR "/src/Soma.Platform.Web"
RUN dotnet build "Soma.Platform.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Soma.Platform.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "Soma.Platform.Web.dll"]