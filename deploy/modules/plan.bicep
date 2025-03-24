param name string
param location string

resource serviceplan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: name
  location: location
  sku: {
    name: 'B1'
    tier: 'standard'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}
