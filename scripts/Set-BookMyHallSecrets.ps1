# ============================================
# BookMyHall - Local Development Secrets
# ============================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " BookMyHall - Local Secrets Configuration" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# --------------------------------------------------
# Determine repository/project path
# --------------------------------------------------

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

$projectRelativePath = "..\src\BookMyHall.Api\BookMyHall.Api.csproj"

$projectPath = Join-Path `
    -Path $scriptDirectory `
    -ChildPath $projectRelativePath

try {
    $projectPath = (Resolve-Path $projectPath).Path
}
catch {
    Write-Host "API project could not be found." -ForegroundColor Red
    Write-Host ""
    Write-Host "Expected project:" -ForegroundColor Yellow
    Write-Host $projectPath
    Write-Host ""
    exit 1
}

Write-Host "API Project:" -ForegroundColor DarkGray
Write-Host $projectPath
Write-Host ""

# --------------------------------------------------
# Verify project
# --------------------------------------------------

if (-not (Test-Path $projectPath)) {

    Write-Host "API project not found:" -ForegroundColor Red
    Write-Host $projectPath

    exit 1
}

Write-Host "API project found." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Initialize User Secrets
# --------------------------------------------------

Write-Host "Initializing .NET User Secrets..." -ForegroundColor Yellow

dotnet user-secrets init --project $projectPath

if ($LASTEXITCODE -ne 0) {
    throw "Failed to initialize .NET User Secrets."
}

Write-Host "User Secrets initialized." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# PostgreSQL Connection String
# --------------------------------------------------

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " PostgreSQL Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$connectionString = Read-Host "Enter PostgreSQL connection string"

if ([string]::IsNullOrWhiteSpace($connectionString)) {
    throw "PostgreSQL connection string cannot be empty."
}

dotnet user-secrets set `
    "ConnectionStrings:DefaultConnection" `
    $connectionString `
    --project $projectPath

if ($LASTEXITCODE -ne 0) {
    throw "Failed to save PostgreSQL connection string."
}

Write-Host "PostgreSQL connection string saved." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Cloudflare R2 Access Key
# --------------------------------------------------

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " Cloudflare R2 Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$r2AccessKeyId = Read-Host "Enter Cloudflare R2 Access Key ID"

if ([string]::IsNullOrWhiteSpace($r2AccessKeyId)) {
    throw "Cloudflare R2 Access Key ID cannot be empty."
}

dotnet user-secrets set `
    "CloudflareR2:AccessKeyId" `
    $r2AccessKeyId `
    --project $projectPath

if ($LASTEXITCODE -ne 0) {
    throw "Failed to save Cloudflare R2 Access Key ID."
}

Write-Host "R2 Access Key ID saved." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Cloudflare R2 Secret Access Key
# --------------------------------------------------

$r2SecretAccessKey = Read-Host "Enter Cloudflare R2 Secret Access Key" -AsSecureString

if ($null -eq $r2SecretAccessKey) {
    throw "Cloudflare R2 Secret Access Key cannot be empty."
}

# Convert SecureString temporarily for dotnet user-secrets
$secretPtr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR(
    $r2SecretAccessKey
)

try {

    $r2SecretPlainText = [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $secretPtr
    )

    if ([string]::IsNullOrWhiteSpace($r2SecretPlainText)) {
        throw "Cloudflare R2 Secret Access Key cannot be empty."
    }

    dotnet user-secrets set `
        "CloudflareR2:SecretAccessKey" `
        $r2SecretPlainText `
        --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to save Cloudflare R2 Secret Access Key."
    }

}
finally {

    if ($secretPtr -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($secretPtr)
    }

    $r2SecretPlainText = $null
}

Write-Host "R2 Secret Access Key saved." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Email / SMTP Password
# --------------------------------------------------

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " Email Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$emailPassword = Read-Host "Enter Gmail App Password" -AsSecureString

$emailSecretPtr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR(
    $emailPassword
)

try {

    $emailPasswordPlainText = [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $emailSecretPtr
    )

    if ([string]::IsNullOrWhiteSpace($emailPasswordPlainText)) {
        throw "Email password cannot be empty."
    }

    dotnet user-secrets set `
        "Email:Password" `
        $emailPasswordPlainText `
        --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to save Email password."
    }

}
finally {

    if ($emailSecretPtr -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($emailSecretPtr)
    }

    $emailPasswordPlainText = $null
}

Write-Host "Email password saved." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# JWT Secret
# --------------------------------------------------

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " JWT Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$jwtSecret = Read-Host "Enter JWT Secret Key" -AsSecureString

$jwtSecretPtr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR(
    $jwtSecret
)

try {

    $jwtSecretPlainText = [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $jwtSecretPtr
    )

    if ([string]::IsNullOrWhiteSpace($jwtSecretPlainText)) {
        throw "JWT Secret Key cannot be empty."
    }

    dotnet user-secrets set `
        "Jwt:SecretKey" `
        $jwtSecretPlainText `
        --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to save JWT Secret Key."
    }

}
finally {

    if ($jwtSecretPtr -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($jwtSecretPtr)
    }

    $jwtSecretPlainText = $null
}

Write-Host "JWT Secret Key saved." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Remove previously-created incorrect secret
# --------------------------------------------------

$wrongSecretKey =
    "CloudflareR2:bbcb52e94efee2267f4aced530d6c2cf57eba2cb70e48c3e4c104e3cfd2806cd"

Write-Host "Cleaning previously created incorrect R2 secret key..." -ForegroundColor Yellow

dotnet user-secrets remove `
    $wrongSecretKey `
    --project $projectPath `
    2>$null

Write-Host "Cleanup completed." -ForegroundColor Green
Write-Host ""

# --------------------------------------------------
# Display configured KEY NAMES only
# --------------------------------------------------

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " Local Secrets Configuration Completed" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Configured secret keys:" -ForegroundColor Cyan
Write-Host ""

# Don't use `dotnet user-secrets list` here because it
# prints the actual secret values.

$secretsJson = dotnet user-secrets list `
    --project $projectPath `
    --json 2>$null

if ($LASTEXITCODE -eq 0 -and $secretsJson) {

    try {

        $secrets = $secretsJson | ConvertFrom-Json

        foreach ($property in $secrets.PSObject.Properties) {
            Write-Host "  [OK] $($property.Name)" -ForegroundColor Green
        }

    }
    catch {

        Write-Host "Secrets were saved successfully." -ForegroundColor Green
        Write-Host "Run 'dotnet user-secrets list' manually if needed." -ForegroundColor Yellow
    }

}
else {

    Write-Host "Secrets were saved successfully." -ForegroundColor Green
}

Write-Host ""
Write-Host "IMPORTANT:" -ForegroundColor Yellow
Write-Host "Restart your BookMyHall API after changing User Secrets."
Write-Host ""