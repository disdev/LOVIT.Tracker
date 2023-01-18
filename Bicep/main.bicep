param subscriptionId string = '6b21ba90-c19d-4d3b-8ce3-95eb73bf9759'
param applicationName string = 'lovit-tracker'
param environmentName string = 'dev'

@allowed([
  'F1'
  'D1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1'
  'P2'
  'P3'
  'P4'
])
param skuName string = 'F1'

@minValue(1)
param skuCapacity int = 1

param sqlAdministratorLogin string

@secure()
param sqlAdministratorLoginPassword string

param location string = resourceGroup().location

var hostingPlanName = '${applicationName}-hostingplan-${environmentName}'
var predictionsName = '${applicationName}-predictions-${environmentName}'
var webSiteName = '${applicationName}-website-${environmentName}'
var webSiteStagingSlotName = 'staging'
var sqlserverName = '${applicationName}-sqlserver-${environmentName}'
var databaseName = '${applicationName}-database'
var resourceGroupName = '${applicationName}-${environmentName}'

resource sqlserver 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: sqlserverName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
  }
}

resource sqlserverName_databaseName 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: '${sqlserver.name}/${databaseName}'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
  }
}

resource sqlserverName_AllowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2014-04-01' = {
  name: '${sqlserver.name}/AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: hostingPlanName
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: skuName
    capacity: skuCapacity
  }
}

resource webSite_predictions 'Microsoft.Web/sites@2020-06-01' = {
  name: predictionsName
  location: location
  tags: {
    'hidden-related:${hostingPlan.id}': 'empty'
    displayName: 'Predictions'
  }
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|Latest'
      http20Enabled: true
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
  }
}

resource webSite 'Microsoft.Web/sites@2020-06-01' = {
  name: webSiteName
  location: location
  tags: {
    'hidden-related:${hostingPlan.id}': 'empty'
    displayName: 'Website'
  }
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|Latest'
      http20Enabled: true
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
  }
}

resource webSiteStaging 'Microsoft.Web/sites/slots@2021-02-01' = {
  name: webSiteStagingSlotName
  location: location
  parent: webSite
  properties: {
    serverFarmId: hostingPlan.id
    cloningInfo: {
      sourceWebAppId: webSite.id
    }
  }
}

resource webSiteConnectionStrings 'Microsoft.Web/sites/config@2020-06-01' = {
  name: '${webSite.name}/connectionstrings'
  properties: {
    TrackerContext: {
      value: 'Data Source=tcp:${sqlserver.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};User Id=${sqlAdministratorLogin}@${sqlserver.properties.fullyQualifiedDomainName};Password=${sqlAdministratorLoginPassword};'
      type: 'SQLAzure'
    }
  }
}

resource AppInsights_webSiteName 'Microsoft.Insights/components@2018-05-01-preview' = {
  name: 'AppInsights${webSite.name}'
  location: location
  tags: {
    'hidden-link:${webSite.id}': 'Resource'
    displayName: 'AppInsightsComponent'
  }
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}
