dotnet test `
  ./${workspaceFolder}/test/Fathcore.Tests/Fathcore.Tests.csproj `
  --no-build `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat="json%2copencover%2ccobertura" `
  /p:CoverletOutput=$PSScriptRoot/artifacts/TestResults/ `
  /p:Exclude="[xunit*]*%2c[*.Tests?]*"

dotnet test `
  ./${workspaceFolder}/test/Fathcore.Extensions.Tests/Fathcore.Extensions.Tests.csproj `
  --no-build `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat="json%2copencover%2ccobertura" `
  /p:CoverletOutput=$PSScriptRoot/artifacts/TestResults/ `
  /p:Exclude="[xunit*]*%2c[*.Tests?]*" `
  /p:MergeWith=${PSScriptRoot}/artifacts/TestResults/coverage.json

Remove-Item ${PSScriptRoot}/artifacts/TestResults/coverage.json
