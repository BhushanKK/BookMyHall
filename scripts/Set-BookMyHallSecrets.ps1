# Run PowerShell as Administrator

$ErrorActionPreference = "Stop"

Write-Host "Configuring BookMyHall production secrets..." -ForegroundColor Cyan

# Cloudflare R2
$R2AccessKeyId = Read-Host "Enter Cloudflare R2 Access Key ID"
$R2SecretAccessKey = Read-Host "Enter Cloudflare R2 Secret Access Key" -AsSecureString

# Convert SecureString to plain text only for setting the environment variable
$R2SecretPlainText = [System.Net.NetworkCredential]::new(
    "",
    $R2SecretAccessKey
).Password

[Environment]::SetEnvironmentVariable(
    "CloudflareR2__AccessKeyId",
    $R2AccessKeyId,
    "Machine"
)

[Environment]::SetEnvironmentVariable(
    "CloudflareR2__SecretAccessKey",
    $R2SecretPlainText,
    "Machine"
)

# Clear local variable
$R2SecretPlainText = $null

Write-Host ""
Write-Host "Cloudflare R2 secrets registered successfully." -ForegroundColor Green

# Verify existence without displaying the secret
$accessKey = [Environment]::GetEnvironmentVariable(
    "CloudflareR2__AccessKeyId",
    "Machine"
)

$secretKey = [Environment]::GetEnvironmentVariable(
    "CloudflareR2__SecretAccessKey",
    "Machine"
)

if ($accessKey -and $secretKey) {
    Write-Host "R2 Access Key: configured" -ForegroundColor Green
    Write-Host "R2 Secret Key: configured" -ForegroundColor Green
}
else {
    Write-Host "R2 configuration failed." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Restart IIS/Application Service after this script." -ForegroundColor Yellow