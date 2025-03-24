param location string
param name string

param serviceplan_name string
param insights_name string
param storageAccountName string

resource serviceplan 'Microsoft.Web/serverfarms@2020-12-01' existing = { name: serviceplan_name }
resource insights 'Microsoft.Insights/components@2020-02-02' existing = { name: insights_name }
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = { name: storageAccountName }

var storagekey = storageAccount.listKeys().keys[0].value

var appsettings = [
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storagekey}'
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: insights.properties.ConnectionString
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
  {
    name: 'WEBSITE_WARMUP_PATH'
    value: '/api/ready'
  }
]

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  dependsOn: [
    serviceplan
  ]
  kind: 'functionapp,linux'
  identity: { type: 'SystemAssigned' }
  properties: {
    serverFarmId: serviceplan.id
    siteConfig: {
      alwaysOn: false
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      healthCheckPath: '/api/health'
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      appSettings: appsettings
    }
    httpsOnly: true
  }
}

// resource functionAppSlot 'Microsoft.Web/sites/slots@2021-03-01' = {
//   name: 'staging'
//   location: location
//   parent: functionApp
//   kind: 'functionapp'
//   identity: { type: 'SystemAssigned' }
//   properties: {
//     siteConfig: {
//       alwaysOn: false
//       netFrameworkVersion: 'v8.0'
//       ftpsState: 'FtpsOnly'
//       minTlsVersion: '1.2'
//       appSettings: union(appsettings, [
//         {
//           name: 'WEBSITE_SWAP_WARMUP_PING_PATH'
//           value: '/api/ready'
//         }
//       ])
//     }
//     httpsOnly: true
//   }
// }
