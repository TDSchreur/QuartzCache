param name string
param location string

param law_name string

resource law 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = { name: law_name }

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: law.id
  }
}
