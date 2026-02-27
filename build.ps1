# 配置环境变量
$env:PATH = "C:\Users\Administrator\.dotnet;$env:PATH"

# 进入项目目录
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectDir

Write-Host "开始编译..."
& dotnet build ZtdApp\ZtdApp.csproj --configuration Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "编译成功!" -ForegroundColor Green

    # 查找可执行文件
    $exeFiles = Get-ChildItem -Path ZtdApp\bin\Debug -Recurse -Filter "*.exe"
    if ($exeFiles) {
        Write-Host "可执行文件:" -ForegroundColor Green
        $exeFiles | ForEach-Object { Write-Host "  - $($_.FullName)" }
    }
} else {
    Write-Host "编译失败，退出代码: $LASTEXITCODE" -ForegroundColor Red
}
