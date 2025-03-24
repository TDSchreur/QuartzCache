param name string
param location string

param keyVaultName string
param functionAppName string

// Reference to existing Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

// Reference to existing Function App
resource functionApp 'Microsoft.Web/sites@2022-09-01' existing = {
  name: functionAppName
}

// Create Event Grid System Topic
resource systemTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: name
  location: location
  properties: {
    source: keyVault.id // '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.KeyVault/vaults/${keyVaultName}'
    topicType: 'Microsoft.KeyVault.vaults'
  }
}

// Create Event Subscription
resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2022-06-15' = {
  parent: systemTopic
  name: 'tds-rdw-kv-events' //'${name}-subscription'
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '${functionApp.id}/functions/KeyVaultEvents'
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
      }
    }
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    filter: {
      includedEventTypes: [
        'Microsoft.KeyVault.CertificateNewVersionCreated'
        'Microsoft.KeyVault.CertificateNearExpiry'
        'Microsoft.KeyVault.CertificateExpired'
        'Microsoft.KeyVault.SecretNewVersionCreated'
        'Microsoft.KeyVault.SecretNearExpiry'
        'Microsoft.KeyVault.SecretExpired'
        'Microsoft.KeyVault.KeyNewVersionCreated'
        'Microsoft.KeyVault.KeyNearExpiry'
        'Microsoft.KeyVault.KeyExpired'
      ]
    }
  }
}
