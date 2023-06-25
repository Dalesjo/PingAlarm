#!/bin/bash

if output=$(git status --porcelain) && [ -n "$output" ]; then
  echo "You have uncommited changes"
  #exit 3
fi

# New Version number
git pull

# dotnet tool install -g dotnet-version-cli
# https://github.com/skarpdev/dotnet-version-cli
dotnet version --skip-vcs -f ./PingAlarm/PingAlarm.csproj patch

VERSION=$(cat ./PingAlarm/PingAlarm.csproj | grep -oPm1 "(?<=<Version>)[^<]+" | tr -d '\n')
echo "New verison is ${VERSION}"

dotnet publish -c Release --self-contained --runtime linux-arm64 PingAlarm/PingAlarm.csproj

echo "Commiting changes"
git add ./PingAlarm/PingAlarm.csproj
cd PingAlarm/bin/Release/net6.0/linux-arm64/publish/
rm appsettings.Development.json
tar -czvf ./../../../../../../images/PingAlarm-${VERSION}.tar.gz *

cd ./../../../../../../

#git commit -m "Built new Version ${VERSION}"
#git push
