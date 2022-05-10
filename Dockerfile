FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /
COPY . .
RUN dotnet restore "Presentation/Presentation.csproj"
RUN dotnet publish "Presentation/Presentation.csproj" -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT ["dotnet", "Presentation.dll"]
