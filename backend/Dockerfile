# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the .csproj files first and restore - this layer only gets
# invalidated (re-run) when a project reference actually changes, not on
# every source edit, so most rebuilds skip re-downloading NuGet packages.
COPY src/RailBooking.Domain/RailBooking.Domain.csproj src/RailBooking.Domain/
COPY src/RailBooking.Api/RailBooking.Api.csproj src/RailBooking.Api/
RUN dotnet restore src/RailBooking.Api/RailBooking.Api.csproj

# Now bring in the rest of the source and publish. The Tests project is
# deliberately not copied in at all - it's irrelevant to running the app
# and would only slow the image down.
COPY src/RailBooking.Domain/ src/RailBooking.Domain/
COPY src/RailBooking.Api/ src/RailBooking.Api/
RUN dotnet publish src/RailBooking.Api/RailBooking.Api.csproj -c Release -o /app --no-restore

# ---- Runtime stage ----
# aspnet (not sdk) - no compiler/build tools in the final image, just what's
# needed to run an already-published app. Much smaller, smaller attack surface.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RailBooking.Api.dll"]
