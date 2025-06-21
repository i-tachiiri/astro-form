param prefix string
param env string
param location string = resourceGroup().location

var suffix = env == 'prod' ? '' : '-${env}'

module storage './modules/blob.bicep' = {
  name: 'storage'
  params: {
    name: '${prefix}storage${suffix}'
    location: location
  }
}

module kv './modules/keyvault.bicep' = {
  name: 'kv'
  params: {
    name: '${prefix}kv${suffix}'
    location: location
    tenantId: tenant().tenantId
  }
}

module cosmos './modules/cosmos.bicep' = {
  name: 'cosmos'
  params: {
    name: '${prefix}cosmos${suffix}'
    location: location
  }
}

module function './modules/function.bicep' = {
  name: 'function'
  params: {
    name: '${prefix}func${suffix}'
    location: location
    storageAccountName: storage.outputs.storageAccountName
  }
  dependsOn: [storage]
}

module swa './modules/swa.bicep' = {
  name: 'swa'
  params: {
    name: '${prefix}swa${suffix}'
    location: location
  }
}
