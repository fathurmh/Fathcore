@echo off
dotnet test

which coverlet || dotnet tool install --global coverlet.console

coverlet ./artifacts/bin/Fathcore.Tests/Debug/netcoreapp2.2/Fathcore.Tests.dll --target "dotnet" --targetargs "test --no-build" --exclude "[xunit*]*" --format lcov --format opencover --output ./artifacts/TestResults/

coverlet ./artifacts/bin/Fathcore.Extensions.Tests/Debug/netcoreapp2.2/Fathcore.Extensions.Tests.dll --target "dotnet" --targetargs "test --no-build" --exclude "[xunit*]*" --format lcov --format opencover --merge-with "coverage.json" --output ./artifacts/TestResults/