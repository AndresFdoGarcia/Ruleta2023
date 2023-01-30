#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

ENV RULETA2023_ENVIROMENT=local

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ruleta2023/Ruleta2023.csproj", "Ruleta2023/"]
RUN dotnet restore "Ruleta2023/Ruleta2023.csproj"
COPY . .
WORKDIR "/src/Ruleta2023"
RUN dotnet build "Ruleta2023.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ruleta2023.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Ruleta2023.dll"]