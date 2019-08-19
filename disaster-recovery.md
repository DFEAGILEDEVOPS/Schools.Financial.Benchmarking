# Disaster Recovery Process

To re-deploy the SFB web application the following steps should be taken.

## Deploy the web application

From VSO trigger the *[ENTERPRISE] Deploy to PRODUCTION staging slot* build definition.
Once VSO has provisioned the Azure production components it will deploy the web application to the App Service staging slot.

## Request restoration of the Azure Cosmos DB backup

Raise a service desk ticket through the Product Owner (currently Carl Fagan) for the Cosmos DB named *sfb-prod* in resource group *rg-t1pr-sfb* to be restored.  Once a service desk ticket number has been issued, raise this with CloudOps to forward to Microsoft.

## Rebuild Search Indexes

Requirements: NodeJs, command prompt (windows) or terminal (macos/linux)

First, delete the existing indexers, indexes and data sources using Azure portal.
Navigate to the `deploy/azure-search/app` folder within the solution.  
Open app/config.json and update the CosmosDB and Azure Search instance details to those that have been provisioned.  The required values can be found in the Azure Portal.
Open schema/datasource/search.json and schema/datasource/search-trust.json and update the data container name you want to index from.

Open a terminal instance and from the `deploy/azure-search/app` directory execute `node index.js search` and then `node index.js search-trust`. If this is the first time you are running, run `npm install` prior to that.  This will rebuild the search indexes, datasources and indexers.  Finally it will schedule the indexer to run immediately.  If the program reports back that the indexer cannot be run at this time("cannot be used while offline" error message), wait a few minutes for the search index to catch up with the datasource and re-run the commands above.

##Flush Redis cache
In Azure Portal, go to sfb-prod instance of Azure Cache for Redis and open the console. Type "flushall" and press enter. Wait for the "OK" message. Alternatively from your local, install and configure "stunnel" then run "flush-redis-cache.bat" under 'deploy/redis' directory. This will purge the cached data (including output cache for school details pages).

## Domain name configuration

If the app service had to be completely rebuilt, it may be required to re-add DNS configuration to the app service to ensure it accepts requests for the schools benchmarking domain name (not finalised at this point).  [Official Microsoft Documentation](https://docs.microsoft.com/en-us/azure/app-service-web/app-service-web-tutorial-custom-domain)

## Completion

Once the search indexer has completed indexing the website should be operational.  This should be manually checked.

## Restart the web application

This is required to be done after data restored because the web application builds User Defined Functions in DB during its start.

