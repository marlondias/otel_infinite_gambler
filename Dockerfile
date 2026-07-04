FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

# Copy csproj and restore dependencies to cache them in the image
COPY *.csproj ./
RUN dotnet restore

# The source code is mounted at runtime via compose.yaml.
# We keep the container running so the user can execute commands inside it.
CMD ["sleep", "infinity"]
