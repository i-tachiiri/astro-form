param name string
param location string

resource swa 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
}

output staticSiteName string = swa.name
