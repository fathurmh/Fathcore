dotnet test `
  ./${workspaceFolder}/test/Fathcore.Tests/Fathcore.Tests.csproj `
  --no-build `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutput=$PSScriptRoot/artifacts/TestResults/ `
  /p:Exclude="[xunit*]*%2c[*.Tests?]*"

dotnet test `
  ./${workspaceFolder}/test/Fathcore.Extensions.Tests/Fathcore.Extensions.Tests.csproj `
  --no-build `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutput=$PSScriptRoot/artifacts/TestResults/ `
  /p:Exclude="[xunit*]*%2c[*.Tests?]*" `
  /p:MergeWith=${PSScriptRoot}/artifacts/TestResults/coverage.json
