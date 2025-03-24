param name string
param location string

resource eventGridNamespace 'Microsoft.EventGrid/namespaces@2023-06-01-preview' = {
  name: name
  location: location
  sku: {
    name: 'Standard'
    capacity: 1
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    inboundIpRules: []
  }
}
