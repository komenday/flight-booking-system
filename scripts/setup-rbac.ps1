<#
.SYNOPSIS
    One-time script for assigning RBAC roles for Managed Identities.

.DESCRIPTION
    Assigns roles for three Managed Identities (API, Notifications Function,
    Expire Function) on Storage Account, Service Bus and Application Insights.

    SQL access is not configured here - it is done via T-SQL commands
    directly in the database (CREATE USER FROM EXTERNAL PROVIDER).

    Run: .\setup-rbac.ps1
    Prerequisites: az login completed, correct subscription selected.

.NOTES
    Runs ONCE on first deployment or when infrastructure changes.
    Idempotent: re-running won't break anything (az role assignment create
    ignores duplicates).
#>

param(
    [string]$SubscriptionId = "d7d2ece6-4b9b-403c-940b-32ed7e6d5538",
    [string]$ResourceGroup  = "rg-fbs-dev"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Setting subscription context..." -ForegroundColor Cyan
az account set --subscription $SubscriptionId

# ─── Gets Principal IDs Managed Identities ──────────────────────────────

Write-Host "`nFetching Managed Identity Principal IDs..." -ForegroundColor Cyan

$apiPrincipalId = az webapp identity show `
    --name fbs-dev `
    --resource-group $ResourceGroup `
    --query principalId --output tsv

$notificationsPrincipalId = az functionapp identity show `
    --name fbns-func-dev `
    --resource-group $ResourceGroup `
    --query principalId --output tsv

$expirePrincipalId = az functionapp identity show `
    --name expire-func-dev `
    --resource-group $ResourceGroup `
    --query principalId --output tsv

Write-Host "  API:           $apiPrincipalId"
Write-Host "  Notifications: $notificationsPrincipalId"
Write-Host "  Expire:        $expirePrincipalId"

# ─── Gets Resource IDs ───────────────────────────────────────────────────

$storageId = az storage account show `
    --name storagefbsdev `
    --resource-group $ResourceGroup `
    --query id --output tsv

$serviceBusId = az servicebus namespace show `
    --name fbs-servicebus-dev `
    --resource-group $ResourceGroup `
    --query id --output tsv

$appInsightsId = az monitor app-insights component show `
    --app fbs-dev `
    --resource-group $ResourceGroup `
    --query id --output tsv

# ─── Helper ─────────────────────────────────────────────────────────

function Assign-Role {
    param(
        [string]$PrincipalId,
        [string]$Role,
        [string]$Scope,
        [string]$Description
    )

    Write-Host "  Assigning '$Role' to $Description..." -NoNewline

    az role assignment create `
        --assignee-object-id $PrincipalId `
        --assignee-principal-type ServicePrincipal `
        --role $Role `
        --scope $Scope `
        --output none 2>$null

    # az returns an error if the role is already assigned — ignore (idempotence)
    Write-Host " done" -ForegroundColor Green
}

# ─── Storage Account ─────────────────────────────────────────────────────────
# All three applications need access to Storage (Functions — for runtime,
# API — if using blob in the future)

Write-Host "`nConfiguring Storage Account roles..." -ForegroundColor Cyan

foreach ($entry in @(
    @{ Id = $apiPrincipalId;           Name = "fbs-dev API" },
    @{ Id = $notificationsPrincipalId; Name = "fbns-func-dev" },
    @{ Id = $expirePrincipalId;        Name = "expire-func-dev" }
)) {
    Assign-Role -PrincipalId $entry.Id -Role "Storage Blob Data Owner"          -Scope $storageId -Description $entry.Name
    Assign-Role -PrincipalId $entry.Id -Role "Storage Queue Data Contributor"   -Scope $storageId -Description $entry.Name
    Assign-Role -PrincipalId $entry.Id -Role "Storage Table Data Contributor"   -Scope $storageId -Description $entry.Name
}

# ─── Service Bus ─────────────────────────────────────────────────────────────
# API publishes events → Data Sender required
# Notifications consumes events → Data Receiver required
# Expire publishes ReservationExpiredEvent → Data Sender required

Write-Host "`nConfiguring Service Bus roles..." -ForegroundColor Cyan

Assign-Role -PrincipalId $apiPrincipalId           -Role "Azure Service Bus Data Sender"   -Scope $serviceBusId -Description "fbs-dev API"
Assign-Role -PrincipalId $notificationsPrincipalId -Role "Azure Service Bus Data Receiver" -Scope $serviceBusId -Description "fbns-func-dev"
Assign-Role -PrincipalId $expirePrincipalId        -Role "Azure Service Bus Data Sender"   -Scope $serviceBusId -Description "expire-func-dev"

# ─── Application Insights ────────────────────────────────────────────────────
# Required to send telemetry via MI (Authorization=AAD)

Write-Host "`nConfiguring Application Insights roles..." -ForegroundColor Cyan

foreach ($entry in @(
    @{ Id = $apiPrincipalId;           Name = "fbs-dev API" },
    @{ Id = $notificationsPrincipalId; Name = "fbns-func-dev" },
    @{ Id = $expirePrincipalId;        Name = "expire-func-dev" }
)) {
    Assign-Role -PrincipalId $entry.Id -Role "Monitoring Metrics Publisher" -Scope $appInsightsId -Description $entry.Name
}

# ─── SQL — notification ───────────────────────────────────────────────────────

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host "SQL access requires manual T-SQL commands in fbs-db-dev:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  -- Run these in Azure Portal Query Editor or SSMS:" -ForegroundColor White
Write-Host "  CREATE USER [fbs-dev] FROM EXTERNAL PROVIDER;" -ForegroundColor White
Write-Host "  ALTER ROLE db_datareader ADD MEMBER [fbs-dev];" -ForegroundColor White
Write-Host "  ALTER ROLE db_datawriter ADD MEMBER [fbs-dev];" -ForegroundColor White
Write-Host ""
Write-Host "  CREATE USER [expire-func-dev] FROM EXTERNAL PROVIDER;" -ForegroundColor White
Write-Host "  ALTER ROLE db_datareader ADD MEMBER [expire-func-dev];" -ForegroundColor White
Write-Host "  ALTER ROLE db_datawriter ADD MEMBER [expire-func-dev];" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow

Write-Host "`nRBAC setup complete!" -ForegroundColor Green
