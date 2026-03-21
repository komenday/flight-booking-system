targetScope = 'resourceGroup'

@description('Environment: dev, staging, prod')
param environment string = 'dev'

@description('Azure region for most resources')
param location string = resourceGroup().location

@description('Azure region for SQL Server and Database')
param sqlLocation string

var resourceNames = {
  appInsights:      'fbs-dev'
  storageAccount:   'storagefbsdev'
  serviceBus:       'fbs-servicebus-dev'
  sqlServer:        'fbs-sql-server-dev'
  sqlDatabase:      'fbs-db-dev'
  apiAppPlan:       'plan-fbs-dev'
  apiApp:           'fbs-dev'
  notificationsPlan: 'ASP-rgfbsdev-b9b9'
  notificationsFunc: 'fbns-func-dev'
  expirePlan:       'ASP-rgfbsdev-afa6'
  expireFunc:       'expire-func-dev'
}

var tags = {
  Environment: environment
  Project: 'FBS'
  Contact: 'Yurii Komenda'
  ManagedBy: 'Bicep'
}

// ─── Application Insights ────────────────────────────────────────────────────
module appInsights 'modules/app-insights.bicep' = {
  name: 'deploy-app-insights'
  params: {
    name: resourceNames.appInsights
    location: location
    tags: tags
  }
}

// ─── Storage Account ─────────────────────────────────────────────────────────
module storage 'modules/storage.bicep' = {
  name: 'deploy-storage'
  params: {
    name: resourceNames.storageAccount
    location: location
    tags: tags
  }
}

// ─── Service Bus ─────────────────────────────────────────────────────────────
module serviceBus 'modules/service-bus.bicep' = {
  name: 'deploy-service-bus'
  params: {
    name: resourceNames.serviceBus
    location: location
    tags: tags
  }
}

// ─── SQL ─────────────────────────────────────────────────────────────────────
module sql 'modules/sql.bicep' = {
  name: 'deploy-sql'
  params: {
    serverName: resourceNames.sqlServer
    databaseName: resourceNames.sqlDatabase
    location: sqlLocation
    tags: tags
  }
}

// ─── API App Service ─────────────────────────────────────────────────────────
module apiApp 'modules/api-app.bicep' = {
  name: 'deploy-api'
  params: {
    appName: resourceNames.apiApp
    planName: resourceNames.apiAppPlan
    location: location
    tags: tags
  }
}

// ─── Notifications Function ───────────────────────────────────────────────────
module notificationsFunc 'modules/function-app.bicep' = {
  name: 'deploy-notifications-func'
  params: {
    functionAppName: resourceNames.notificationsFunc
    planName: resourceNames.notificationsPlan
    location: location
    tags: tags
  }
  dependsOn: [storage]
}

// ─── Expire Reservations Function ────────────────────────────────────────────
module expireFunc 'modules/function-app.bicep' = {
  name: 'deploy-expire-func'
  params: {
    functionAppName: resourceNames.expireFunc
    planName: resourceNames.expirePlan
    location: location
    tags: tags
  }
  dependsOn: [storage]
}

// ─── Outputs (used in CI/CD workflows) ────────────────────────────
output apiAppPrincipalId string = apiApp.outputs.principalId
output notificationsFuncPrincipalId string = notificationsFunc.outputs.principalId
output expireFuncPrincipalId string = expireFunc.outputs.principalId
output serviceBusNamespace string = serviceBus.outputs.fullyQualifiedNamespace
output sqlServerFqdn string = sql.outputs.serverFqdn
