FROM mcr.microsoft.com/dotnet/core/sdk:2.2
COPY ./ /app
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release -o out
WORKDIR /app/out

EXPOSE 80

CMD ["dotnet", "Library.NotificationService2.dll"]
