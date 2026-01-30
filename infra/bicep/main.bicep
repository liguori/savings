// Main Bicep template for Savings Application on Azure

@description('The name of the environment (dev, staging, prod)')
param environment string

@description('The location for all resources')
param location string = resourceGroup().location

@description('The SKU for the App Service Plan')
@allowed([
  'F1'  // Free tier
  'D1'  // Shared tier
  'B1'  // Basic tier
  'B2'
  'B3'
  'S1'  // Standard tier
  'S2'
  'S3'
  'P1v2' // Premium v2 tier
  'P2v2'
  'P3v2'
])
param appServicePlanSku string = 'B1'

@description('The SKU for the SQL Database')
@allowed([
  'Basic'
  'S0'
  'S1'
  'S2'
  'P1'
  'P2'
])
param sqlDatabaseSku string = 'Basic'

@description('SQL Server administrator login')
@secure()
param sqlAdminLogin string

@description('SQL Server administrator password')
@secure()
param sqlAdminPassword string

@description('Tags to apply to all resources')
param tags object = {
  Environment: environment
  Application: 'Savings'
}

// Variables
var appServicePlanName = 'asp-savings-${environment}-${uniqueString(resourceGroup().id)}'
var apiAppServiceName = 'app-savings-api-${environment}-${uniqueString(resourceGroup().id)}'
var spaAppServiceName = 'app-savings-spa-${environment}-${uniqueString(resourceGroup().id)}'
var sqlServerName = 'sql-savings-${environment}-${uniqueString(resourceGroup().id)}'
var sqlDatabaseName = 'sqldb-savings-${environment}'
var appInsightsName = 'appi-savings-${environment}-${uniqueString(resourceGroup().id)}'
var logAnalyticsWorkspaceName = 'log-savings-${environment}-${uniqueString(resourceGroup().id)}'

// Log Analytics Workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true // Linux
  }
  kind: 'linux'
}

// API App Service
resource apiAppService 'Microsoft.Web/sites@2022-09-01' = {
  name: apiAppServiceName
  location: location
  tags: tags
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: appServicePlanSku != 'F1' && appServicePlanSku != 'D1'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
          type: 'SQLAzure'
        }
      ]
    }
  }
}

// SPA App Service
resource spaAppService 'Microsoft.Web/sites@2022-09-01' = {
  name: spaAppServiceName
  location: location
  tags: tags
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn: appServicePlanSku != 'F1' && appServicePlanSku != 'D1'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
      ]
    }
  }
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  tags: tags
  sku: {
    name: sqlDatabaseSku
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648 // 2GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
  }
}

// SQL Server Firewall Rule - Allow Azure Services
resource sqlFirewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Outputs
output apiAppServiceName string = apiAppService.name
output apiAppServiceUrl string = 'https://${apiAppService.properties.defaultHostName}'
output spaAppServiceName string = spaAppService.name
output spaAppServiceUrl string = 'https://${spaAppService.properties.defaultHostName}'
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDatabase.name
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
