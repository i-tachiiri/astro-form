param name string
param location string
param databaseName string = 'astroform-db'

resource account 'Microsoft.DocumentDB/databaseAccounts@2023-03-15' = {
  name: name
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-03-15' = {
  parent: account
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
    options: {}
  }
}

var containers = [
  {
    name: 'Users'
    partition: '/id'
  }
  {
    name: 'Forms'
    partition: '/userId'
  }
  {
    name: 'FormSubmissions'
    partition: '/formId'
  }
  {
    name: 'ActivityLogs'
    partition: '/partitionKey'
  }
]

resource containerResources 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-03-15' = [for c in containers: {
  parent: database
  name: c.name
  properties: {
    resource: {
      id: c.name
      partitionKey: {
        paths: [c.partition]
        kind: 'Hash'
      }
    }
    options: {}
  }
}]
