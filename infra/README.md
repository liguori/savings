# Azure Infrastructure as Code

This folder contains the Infrastructure as Code (IaC) for deploying the Savings application to Azure using Bicep templates.

## Architecture Overview

The infrastructure deploys the following Azure resources:

- **Resource Group**: Container for all resources
- **App Service Plan**: Hosts both the API and SPA applications
- **API App Service**: Runs the ASP.NET Core Web API backend
- **SPA App Service**: Runs the Blazor WebAssembly frontend
- **Azure SQL Server**: Hosts the SQL Database
- **Azure SQL Database**: Stores application data
- **Application Insights**: Application monitoring and telemetry
- **Log Analytics Workspace**: Centralized logging

## Prerequisites

Before deploying the infrastructure, ensure you have:

1. **Azure Subscription**: An active Azure subscription
2. **Azure CLI**: Install from [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. **GitHub Repository Secrets**: Configure the following secrets in your GitHub repository:
   - `AZURE_SERVICE_PRINCIPAL`: Azure service principal credentials (JSON format)
   - `SQL_ADMIN_PASSWORD`: Password for the SQL Server administrator

### Setting up Azure Service Principal

Create a service principal with the following command:

```bash
az ad sp create-for-rbac --name "savings-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/rg-savings-{environment} \
  --sdk-auth
```

Copy the JSON output and save it as the `AZURE_SERVICE_PRINCIPAL` secret in GitHub.

## Deployment

### Using GitHub Actions (Recommended)

1. Go to the **Actions** tab in your GitHub repository
2. Select the **Deploy Infrastructure on Azure** workflow
3. Click **Run workflow**
4. Select the environment (dev, staging, prod)
5. Choose the action:
   - **validate**: Validates the Bicep template without deploying
   - **what-if**: Shows what changes will be made without deploying
   - **deploy**: Deploys the infrastructure

### Manual Deployment with Azure CLI

#### 1. Login to Azure

```bash
az login
```

#### 2. Create Resource Group

```bash
az group create \
  --name rg-savings-dev \
  --location eastus \
  --tags Environment=dev Application=Savings
```

#### 3. Validate the Template

```bash
az deployment group validate \
  --resource-group rg-savings-dev \
  --template-file infra/bicep/main.bicep \
  --parameters infra/parameters/dev.parameters.json \
  --parameters sqlAdminPassword='YourSecurePassword123!'
```

#### 4. Preview Changes (What-If)

```bash
az deployment group what-if \
  --resource-group rg-savings-dev \
  --template-file infra/bicep/main.bicep \
  --parameters infra/parameters/dev.parameters.json \
  --parameters sqlAdminPassword='YourSecurePassword123!'
```

#### 5. Deploy Infrastructure

```bash
az deployment group create \
  --resource-group rg-savings-dev \
  --template-file infra/bicep/main.bicep \
  --parameters infra/parameters/dev.parameters.json \
  --parameters sqlAdminPassword='YourSecurePassword123!' \
  --mode Incremental
```

#### 6. Get Deployment Outputs

```bash
az deployment group show \
  --resource-group rg-savings-dev \
  --name savings-infra-deployment \
  --query properties.outputs
```

## Environment Configuration

### Parameter Files

The infrastructure supports three environments, each with its own parameter file:

- **dev.parameters.json**: Development environment (Basic tier SQL, B1 App Service Plan)
- **staging.parameters.json**: Staging environment (S0 tier SQL, S1 App Service Plan)
- **prod.parameters.json**: Production environment (S1 tier SQL, P1v2 App Service Plan)

### Modifying Parameters

To customize the deployment for your needs, edit the appropriate parameter file in `infra/parameters/`:

```json
{
  "environment": {
    "value": "dev"
  },
  "location": {
    "value": "eastus"
  },
  "appServicePlanSku": {
    "value": "B1"
  },
  "sqlDatabaseSku": {
    "value": "Basic"
  }
}
```

### Secrets Management

The SQL admin password is passed as a parameter during deployment and should be stored securely:

- **For GitHub Actions**: Store the password in GitHub Secrets as `SQL_ADMIN_PASSWORD`
- **For manual deployment**: Pass it directly via the command line `--parameters sqlAdminPassword='YourSecurePassword123!'`

#### Optional: Using Azure Key Vault for Secrets

For enhanced security, you can configure parameter files to reference Azure Key Vault:

1. Create an Azure Key Vault:
```bash
az keyvault create \
  --name kv-savings-dev \
  --resource-group rg-savings-dev \
  --location eastus
```

2. Store the SQL admin password:
```bash
az keyvault secret set \
  --vault-name kv-savings-dev \
  --name sql-admin-password \
  --value 'YourSecurePassword123!'
```

3. Update the parameter file with your Key Vault reference:
```json
"sqlAdminPassword": {
  "reference": {
    "keyVault": {
      "id": "/subscriptions/{subscription-id}/resourceGroups/rg-savings-dev/providers/Microsoft.KeyVault/vaults/kv-savings-dev"
    },
    "secretName": "sql-admin-password"
  }
}
```

## Post-Deployment Steps

After deploying the infrastructure:

1. **Update GitHub Environment Variables**: Set the following variables in your GitHub environment:
   - `APPSERVICENAME`: The API App Service name from deployment outputs
   - `APPSERVICENAME_FE`: The SPA App Service name from deployment outputs
   - `APPSERVICE_FE_CONFIG_APIURL`: The API URL from deployment outputs

2. **Configure Azure AD Authentication** (if using Azure AD):
   - Create App Registrations for the API and SPA
   - Configure API permissions and scopes
   - Update the deployment workflows with the appropriate configuration values

3. **Deploy Application Code**:
   - Run the `Deploy Backend on Azure` workflow
   - Run the `Deploy Frontend on Azure` workflow

## Troubleshooting

### Common Issues

**Issue**: Deployment fails with "Resource name already exists"
- **Solution**: Resource names include a unique string based on the resource group ID. Ensure you're deploying to the correct resource group.

**Issue**: SQL Database connection fails
- **Solution**: Verify the firewall rules allow connections from Azure services and check the connection string.

**Issue**: App Service deployment fails
- **Solution**: Ensure the App Service Plan has sufficient capacity and the deployment package is valid.

### Checking Deployment Status

```bash
# List all deployments in a resource group
az deployment group list \
  --resource-group rg-savings-dev \
  --query "[].{Name:name, State:properties.provisioningState, Timestamp:properties.timestamp}" \
  --output table

# Get detailed deployment information
az deployment group show \
  --resource-group rg-savings-dev \
  --name main
```

## Cost Estimation

Approximate monthly costs by environment (prices may vary by region):

- **Dev**: ~$15-30/month (B1 App Service, Basic SQL)
- **Staging**: ~$100-150/month (S1 App Service, S0 SQL)
- **Prod**: ~$200-300/month (P1v2 App Service, S1 SQL)

Use the [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/) for accurate estimates.

## Cleanup

To delete all resources in an environment:

```bash
az group delete --name rg-savings-dev --yes
```

## Additional Resources

- [Azure Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/en-us/azure/azure-sql/)
