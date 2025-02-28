#!/bin/bash
set -e # Exit on error

# Config
PROJECT_ROOT="/Users/metehangultekin/Developer/QuickAPI"
VERSION="1.0.0"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
API_KEY="YOUR_API_KEY_HERE" # Replace with your NuGet API key

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Starting NuGet package publishing process...${NC}"

# Step 1: Build and pack QuickAPI.Extensions
echo -e "${YELLOW}Step 1: Building and packing QuickAPI.Extensions...${NC}"
cd "$PROJECT_ROOT/QuickAPI.Extensions"
dotnet build -c Release
dotnet pack -c Release
echo -e "${GREEN}QuickAPI.Extensions package created successfully${NC}"

# Step 2: Build and pack QuickAPI.Database
echo -e "${YELLOW}Step 2: Building and packing QuickAPI.Database...${NC}"
cd "$PROJECT_ROOT/QuickAPI.Database"
dotnet build -c Release
dotnet pack -c Release
echo -e "${GREEN}QuickAPI.Database package created successfully${NC}"

# Step 3: Update QuickAPI.Shared to use NuGet package for Extensions
echo -e "${YELLOW}Step 3: Updating QuickAPI.Shared to use QuickAPI.Extensions NuGet package...${NC}"
cd "$PROJECT_ROOT/QuickAPI.Shared"
dotnet remove reference ../QuickAPI.Extensions/QuickAPI.Extensions.csproj
dotnet add package QuickAPI.Extensions -v $VERSION --source "$PROJECT_ROOT/QuickAPI.Extensions/bin/Release"

# Step 4: Build and pack QuickAPI.Shared
echo -e "${YELLOW}Step 4: Building and packing QuickAPI.Shared...${NC}"
dotnet build -c Release
dotnet pack -c Release
echo -e "${GREEN}QuickAPI.Shared package created successfully${NC}"

# Step 5: Update QuickAPI to use NuGet packages instead of project references
echo -e "${YELLOW}Step 5: Updating QuickAPI to use QuickAPI.Database and QuickAPI.Shared NuGet packages...${NC}"
cd "$PROJECT_ROOT/QuickAPI"
dotnet remove reference ../QuickAPI.Database/QuickAPI.Database.csproj
dotnet remove reference ../QuickAPI.Shared/QuickAPI.Shared.csproj
dotnet add package QuickAPI.Database -v $VERSION --source "$PROJECT_ROOT/QuickAPI.Database/bin/Release"
dotnet add package QuickAPI.Shared -v $VERSION --source "$PROJECT_ROOT/QuickAPI.Shared/bin/Release"

# Step 6: Build and pack QuickAPI
echo -e "${YELLOW}Step 6: Building and packing QuickAPI...${NC}"
dotnet build -c Release
dotnet pack -c Release
echo -e "${GREEN}QuickAPI package created successfully${NC}"

echo -e "${YELLOW}All packages have been built. You can now publish them to NuGet with:${NC}"
echo "dotnet nuget push $PROJECT_ROOT/QuickAPI.Extensions/bin/Release/QuickAPI.Extensions.$VERSION.nupkg --api-key YOUR_API_KEY --source $NUGET_SOURCE"
echo "dotnet nuget push $PROJECT_ROOT/QuickAPI.Database/bin/Release/QuickAPI.Database.$VERSION.nupkg --api-key YOUR_API_KEY --source $NUGET_SOURCE"
echo "dotnet nuget push $PROJECT_ROOT/QuickAPI.Shared/bin/Release/QuickAPI.Shared.$VERSION.nupkg --api-key YOUR_API_KEY --source $NUGET_SOURCE"
echo "dotnet nuget push $PROJECT_ROOT/QuickAPI/bin/Release/QuickAPI.$VERSION.nupkg --api-key YOUR_API_KEY --source $NUGET_SOURCE"

echo -e "${GREEN}Package creation completed successfully!${NC}"