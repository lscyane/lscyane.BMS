# エラー発生時に処理を停止する設定
$ErrorActionPreference = "Stop"

# ==================================================
# 設定エリア
# ==================================================
# 1. パッケージ化するプロジェクトのフォルダ（sln階層からの相対パス）
$projectPath = ".\lscyane.BMS"

# 2. パッケージが出力されるディレクトリ
$targetDir = Join-Path $projectPath "bin\Release"

# 3. APIキーファイルのパス
$keyPath = "../../_Tools/NugetApiKey.txt" 

$nugetSource = "https://api.nuget.org/v3/index.json"

try {
    # ==================================================
    # 1. APIキーの読み込み
    # ==================================================
    if (-not (Test-Path $keyPath)) {
        throw "APIキーファイルが見つかりません: $keyPath"
    }
    
    $apiKey = (Get-Content $keyPath -TotalCount 1).Trim()
    
    if ([string]::IsNullOrWhiteSpace($apiKey)) {
        throw "APIキーが空、または正しく読み込めませんでした。"
    }

    # ==================================================
    # 2. パッケージの作成 (Pack)
    # ==================================================
    Write-Host "NuGetパッケージを作成中 (dotnet pack)..." -ForegroundColor Cyan
    
    # 対象プロジェクトを指定してパックを実行
    dotnet pack $projectPath -c Release
    if ($LASTEXITCODE -ne 0) { throw "dotnet pack の実行に失敗しました。" }

    # ==================================================
    # 3. 最新の .nupkg ファイルを特定
    # ==================================================
    if (-not (Test-Path $targetDir)) {
        throw "出力先ディレクトリが見つかりません: $targetDir"
    }

    # lscyane.BMS/bin/Release 以下の .nupkg を取得
    $latestPackage = Get-ChildItem -Path $targetDir -Filter "*.nupkg" |
                     Where-Object { $_.Name -notmatch "symbols" } |
                     Sort-Object LastWriteTime -Descending |
                     Select-Object -First 1

    if ($null -eq $latestPackage) {
        throw "発行対象の .nupkg ファイルが見つかりませんでした。($targetDir)"
    }

    Write-Host "`n発行対象: $($latestPackage.Name)" -ForegroundColor Yellow

    # ==================================================
    # 4. パッケージの発行 (Push)
    # ==================================================
    Write-Host "`nNuGet.org へ発行中 (dotnet nuget push)..." -ForegroundColor Cyan
    
    dotnet nuget push $latestPackage.FullName --api-key $apiKey --source $nugetSource
    if ($LASTEXITCODE -ne 0) { throw "dotnet nuget push の実行に失敗しました。" }

    Write-Host "`n[SUCCESS] すべての処理が正常に完了しました。" -ForegroundColor Green
}
catch {
    Write-Host "`n[ERROR] $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    $apiKey = $null
}

Read-Host "`n終了するには Enter キーを押してください..."