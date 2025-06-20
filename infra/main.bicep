param env string
param location string = resourceGroup().location
var prefix = 'astroform'

module swa 'modules/swa.bicep' = {
  name: 'swa'
  params: {
    name: '${prefix}-swa-${env}'
    location: location
  }
}

module func 'modules/function.bicep' = {
  name: 'func'
  params: {
    name: '${prefix}-api-${env}'
    location: location
  }
}

module blob 'modules/blob.bicep' = {
  name: 'blob'
  params: {
    name: '${prefix}storage${env}'
    location: location
  }
}

module kv 'modules/keyvault.bicep' = {
  name: 'kv'
  params: {
    name: '${prefix}-kv-${env}'
    location: location
  }
}

module cosmos 'modules/cosmos.bicep' = {
  name: 'cosmos'
  params: {
    name: '${prefix}-cosmos-${env}'
    location: location
  }
}
