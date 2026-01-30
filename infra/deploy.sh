#!/bin/bash
# Helper script to deploy Azure infrastructure for Savings application

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    print_error "Azure CLI is not installed. Please install it from https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if user is logged in to Azure
if ! az account show &> /dev/null; then
    print_error "You are not logged in to Azure. Please run 'az login' first."
    exit 1
fi

# Parse command line arguments
ENVIRONMENT=${1:-dev}
ACTION=${2:-validate}

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|prod)$ ]]; then
    print_error "Invalid environment: $ENVIRONMENT. Must be dev, staging, or prod."
    exit 1
fi

# Validate action
if [[ ! "$ACTION" =~ ^(validate|what-if|deploy)$ ]]; then
    print_error "Invalid action: $ACTION. Must be validate, what-if, or deploy."
    exit 1
fi

RESOURCE_GROUP="rg-savings-${ENVIRONMENT}"
LOCATION="eastus"
TEMPLATE_FILE="infra/bicep/main.bicep"
PARAMETERS_FILE="infra/parameters/${ENVIRONMENT}.parameters.json"

print_info "Environment: $ENVIRONMENT"
print_info "Action: $ACTION"
print_info "Resource Group: $RESOURCE_GROUP"
print_info "Location: $LOCATION"
echo ""

# Check if parameter file exists
if [ ! -f "$PARAMETERS_FILE" ]; then
    print_error "Parameter file not found: $PARAMETERS_FILE"
    exit 1
fi

# Check if template file exists
if [ ! -f "$TEMPLATE_FILE" ]; then
    print_error "Template file not found: $TEMPLATE_FILE"
    exit 1
fi

# Prompt for SQL admin password if deploying
if [ "$ACTION" == "deploy" ] || [ "$ACTION" == "what-if" ]; then
    print_warning "You will need to provide the SQL admin password."
    read -sp "Enter SQL admin password: " SQL_ADMIN_PASSWORD
    echo ""
    
    if [ -z "$SQL_ADMIN_PASSWORD" ]; then
        print_error "SQL admin password cannot be empty."
        exit 1
    fi
fi

# Create resource group if it doesn't exist
print_info "Ensuring resource group exists..."
if ! az group show --name "$RESOURCE_GROUP" &> /dev/null; then
    print_info "Creating resource group $RESOURCE_GROUP in $LOCATION..."
    az group create \
        --name "$RESOURCE_GROUP" \
        --location "$LOCATION" \
        --tags Environment="$ENVIRONMENT" Application=Savings
else
    print_info "Resource group $RESOURCE_GROUP already exists."
fi

echo ""

# Perform the action
case $ACTION in
    validate)
        print_info "Validating Bicep template..."
        az deployment group validate \
            --resource-group "$RESOURCE_GROUP" \
            --template-file "$TEMPLATE_FILE" \
            --parameters "$PARAMETERS_FILE"
        
        print_info "Validation successful!"
        ;;
    
    what-if)
        print_info "Running what-if analysis..."
        az deployment group what-if \
            --resource-group "$RESOURCE_GROUP" \
            --template-file "$TEMPLATE_FILE" \
            --parameters "$PARAMETERS_FILE" \
            --parameters sqlAdminPassword="$SQL_ADMIN_PASSWORD"
        ;;
    
    deploy)
        print_info "Deploying infrastructure..."
        az deployment group create \
            --resource-group "$RESOURCE_GROUP" \
            --template-file "$TEMPLATE_FILE" \
            --parameters "$PARAMETERS_FILE" \
            --parameters sqlAdminPassword="$SQL_ADMIN_PASSWORD" \
            --mode Incremental
        
        echo ""
        print_info "Deployment successful!"
        echo ""
        print_info "Getting deployment outputs..."
        outputs=$(az deployment group show \
            --resource-group "$RESOURCE_GROUP" \
            --name main \
            --query properties.outputs)
        
        echo ""
        print_info "Deployment Outputs:"
        echo "$outputs" | jq '.'
        
        echo ""
        print_info "Next steps:"
        echo "1. Update your GitHub environment variables with the App Service names"
        echo "2. Run the backend and frontend deployment workflows"
        ;;
esac
