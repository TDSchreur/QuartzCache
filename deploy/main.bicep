param location string = resourceGroup().location
param projectname string = 'tds-rdw'

var law_name = '${projectname}-law'
var egns_name = '${projectname}-eg-namespace'
var insights_name = '${projectname}-ai'
var kv_name = '${projectname}-kv'
var storage_name = 'tdsrdwtestsa523'
var serviceplan_name = '${projectname}-function-plan'
var function_name = '${projectname}-function'
var egst_name = '${projectname}-kv-egst'

module egns 'modules/eventgridnamespace.bicep' = {
  name: '${egns_name}-deployment'
  params: {
    name: egns_name
    location: location
  }
}

module law 'modules/law.bicep' = {
  name: '${law_name}-deployment'
  params: {
    name: law_name
    location: location
  }
}

module insights './modules/insights.bicep' = {
  name: '${insights_name}-deployment'
  params: {
    name: insights_name
    location: location
    law_name: law_name
  }
}

module kv 'modules/kv.bicep' = {
  name: '${kv_name}-deployment'
  params: {
    name: kv_name
    location: location
  }
}

module storage './modules/storage.bicep' = {
  name: '${storage_name}-deployment'
  params: {
    name: storage_name
    location: location
  }
}

module plan 'modules/plan.bicep' = {
  name: '${serviceplan_name}-deployment'
  params: {
    location: location
    name: serviceplan_name
  }
}

module function 'modules/function.bicep' = {
  name: '${function_name}-deployment'
  params: {
    name: function_name
    location: location
    serviceplan_name: serviceplan_name
    storageAccountName: storage_name
    insights_name: insights_name
  }
}

module egst 'modules/eventgridsystemtopic.bicep' = {
  name: '${egst_name}-egst-deployment'
  params: {
    name: egst_name
    location: location
    keyVaultName: kv_name
    functionAppName: function_name
  }
}
