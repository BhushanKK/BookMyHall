# ============================================

# BookMyLawns - Production VM Secrets

# ============================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " BookMyLawns - Production Secrets" -ForegroundColor Cyan
Write-Host " Windows Machine Environment Variables" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$currentPrincipal = New-Object Security.Principal.WindowsPrincipal(
[Security.Principal.WindowsIdentity]::GetCurrent()
)

if (-not $currentPrincipal.IsInRole(
[Security.Principal.WindowsBuiltInRole]::Administrator
)) {
Write-Host "ERROR: PowerShell must be run as Administrator." -ForegroundColor Red
Write-Host ""
Write-Host "Right-click PowerShell -> Run as Administrator." -ForegroundColor Yellow
exit 1
}

Write-Host "[OK] Administrator privileges confirmed." -ForegroundColor Green
Write-Host ""

function Set-MachineSecret {
param (
[Parameter(Mandatory = $true)]
[string]$Name,


    [Parameter(Mandatory = $true)]
    [string]$Value
)

if ([string]::IsNullOrWhiteSpace($Value)) {
    throw "$Name cannot be empty."
}

[Environment]::SetEnvironmentVariable(
    $Name,
    $Value,
    [EnvironmentVariableTarget]::Machine
)

Write-Host "[OK] $Name registered." -ForegroundColor Green


}

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " PostgreSQL Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$connectionString = Read-Host "Enter PostgreSQL connection string"

Set-MachineSecret `    "ConnectionStrings__DefaultConnection"`
$connectionString

Write-Host ""

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " Cloudflare R2 Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$r2AccessKeyId = Read-Host "Enter Cloudflare R2 Access Key ID"

Set-MachineSecret `    "CloudflareR2__AccessKeyId"`
$r2AccessKeyId

Write-Host ""

$r2SecretAccessKey = Read-Host `    "Enter Cloudflare R2 Secret Access Key"`
-AsSecureString

$r2SecretPtr = [IntPtr]::Zero

try {
$r2SecretPtr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR(
$r2SecretAccessKey
)


$r2SecretPlainText =
    [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $r2SecretPtr
    )

Set-MachineSecret `
    "CloudflareR2__SecretAccessKey" `
    $r2SecretPlainText


}
finally {
if ($r2SecretPtr -ne [IntPtr]::Zero) {
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR(
$r2SecretPtr
)
}


$r2SecretPlainText = $null


}

Write-Host ""

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " Titan Mail Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

Write-Host "Email account: [info@bookmylawns.in](mailto:info@bookmylawns.in)" -ForegroundColor Yellow
Write-Host "SMTP server: smtp.titan.email" -ForegroundColor Yellow
Write-Host ""

$emailPassword = Read-Host `    "Enter Titan Mail Password / App Password"`
-AsSecureString

$emailSecretPtr = [IntPtr]::Zero

try {
$emailSecretPtr =
[Runtime.InteropServices.Marshal]::SecureStringToBSTR(
$emailPassword
)


$emailPasswordPlainText =
    [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $emailSecretPtr
    )

Set-MachineSecret `
    "Email__Password" `
    $emailPasswordPlainText


}
finally {
if ($emailSecretPtr -ne [IntPtr]::Zero) {
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR(
$emailSecretPtr
)
}


$emailPasswordPlainText = $null


}

Write-Host ""

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " JWT Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$jwtSecret = Read-Host `    "Enter JWT Secret Key"`
-AsSecureString

$jwtSecretPtr = [IntPtr]::Zero

try {
$jwtSecretPtr =
[Runtime.InteropServices.Marshal]::SecureStringToBSTR(
$jwtSecret
)


$jwtSecretPlainText =
    [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $jwtSecretPtr
    )

Set-MachineSecret `
    "Jwt__SecretKey" `
    $jwtSecretPlainText


}
finally {
if ($jwtSecretPtr -ne [IntPtr]::Zero) {
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR(
$jwtSecretPtr
)
}


$jwtSecretPlainText = $null


}

Write-Host ""

Write-Host "-----------------------------------------" -ForegroundColor DarkGray
Write-Host " RabbitMQ Configuration" -ForegroundColor Cyan
Write-Host "-----------------------------------------" -ForegroundColor DarkGray

$rabbitMqUserName = Read-Host "Enter RabbitMQ Username"

Set-MachineSecret `    "RabbitMq__UserName"`
$rabbitMqUserName

Write-Host ""

$rabbitMqPassword = Read-Host `    "Enter RabbitMQ Password"`
-AsSecureString

$rabbitMqSecretPtr = [IntPtr]::Zero

try {
$rabbitMqSecretPtr =
[Runtime.InteropServices.Marshal]::SecureStringToBSTR(
$rabbitMqPassword
)


$rabbitMqPasswordPlainText =
    [Runtime.InteropServices.Marshal]::PtrToStringBSTR(
        $rabbitMqSecretPtr
    )

Set-MachineSecret `
    "RabbitMq__Password" `
    $rabbitMqPasswordPlainText


}
finally {
if ($rabbitMqSecretPtr -ne [IntPtr]::Zero) {
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR(
$rabbitMqSecretPtr
)
}


$rabbitMqPasswordPlainText = $null


}

Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " Verification" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$secretNames = @(
"ConnectionStrings__DefaultConnection",
"CloudflareR2__AccessKeyId",
"CloudflareR2__SecretAccessKey",
"Email__Password",
"Jwt__SecretKey",
"RabbitMq__UserName",
"RabbitMq__Password"
)

foreach ($name in $secretNames) {
$value = [Environment]::GetEnvironmentVariable(
$name,
[EnvironmentVariableTarget]::Machine
)


if ([string]::IsNullOrWhiteSpace($value)) {
    Write-Host "[MISSING] $name" -ForegroundColor Red
}
else {
    Write-Host "[OK]      $name" -ForegroundColor Green
}


}

Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " Production Secrets Configuration Done" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Email configuration:" -ForegroundColor Yellow
Write-Host "From: [info@bookmylawns.in](mailto:info@bookmylawns.in)"
Write-Host "SMTP: smtp.titan.email"
Write-Host "Port: 587"
Write-Host ""

Write-Host "IMPORTANT:" -ForegroundColor Yellow
Write-Host "Restart the BookMyLawns API/service after changing secrets."
Write-Host ""
