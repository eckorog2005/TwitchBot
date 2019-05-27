FROM mcr.microsoft.com/dotnet/core/aspnet:latest
COPY bin/Release/netcoreapp2.2/publish app/

WORKDIR /app

ENTRYPOINT ["dotnet", "Twitchbot.App.dll"]