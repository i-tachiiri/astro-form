param name string
param location string

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: name
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableFreeTier: true
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
  }
}

output accountName string = cosmos.name
