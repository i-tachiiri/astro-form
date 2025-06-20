param name string
param location string

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2023-03-15' = {
  name: name
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
  }
}
