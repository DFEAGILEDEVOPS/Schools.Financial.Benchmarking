# Schools Financial Benchmarking

[![Build Status](https://agilefactory.visualstudio.com/Financial%20Benchmarking/_apis/build/status/SFB.Web.Application?branchName=master)](https://agilefactory.visualstudio.com/Financial%20Benchmarking/_build/latest?definitionId=471&branchName=master) ![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/DFEAGILEDEVOPS/schools-financial-benchmarking?include_prereleases) ![GitHub release (latest by date)](https://img.shields.io/github/v/release/DFEAGILEDEVOPS/schools-financial-benchmarking)

## Introduction
This application is build using ASP.NET MVC with .Net Framework v4.7 referencing a CosmosDB database NoSQL management system hosted on Microsoft Azure.

## Database Dependencies

[Data Migration Docs](data-migration.md)

[Disaster Recovery Docs](disaster-recovery.md)

## Latest data update

Maintained - 15/11/2023 19:30

## Dev setup (Front end assets)

### Prerequisites

The following tools may be installed directly on Windows, or via Chocolatey. [Chocolatey](https://chocolatey.org/) is advised when installing [Node](https://nodejs.org/en) via the latest downloaded binaries and will install the VS and Python dependencies below automatically.

> **NOTE:** This solution will not build on non-Windows boxes due to the dependency on C++ binaries used by `node-gyp`

1. [`nvm` for Windows](https://github.com/coreybutler/nvm-windows)
1. [Visual Studio 2019 Build Tools](https://my.visualstudio.com/Downloads?q=visual%20studio%202019&wt.mc_id=o~msft~vscom~older-downloads)
1. [Python 3.7](https://www.python.org/downloads/release/python-379/)

### Steps

1. Open web folder: `cd .\Web\SFB.Web.UI\`
1. Install correct `node` version: `nvm install 16.20.2`
1. Set `node` version: `nvm use 16.20.2`
1. Install `grunt` globally: `npm i grunt -g`
1. Install `node-gyp` globally: `npm i node-gyp -g`
1. Install `node-sass` globally: `npm i node-sass -g`
1. Install deps: `npm i`
1. Build assets: `npm run build:newAndLegacyDev`

> **NOTE:** The `webpack` build is slow and as at 2024 there will be a large number of warnings about deprecated packages. As long as `./public/assets/scripts/application.js` has been generated then build has been successful.

## Dev setup (.NET)

When restoring packages in the .NET Framework solution, the private package feed in DevOps will need to be authenticated with. If the built-in credential manager does not work (with `401 Unauthoized` errors from NuGet), then [try a PAT](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate) instead and use [Rider](https://www.jetbrains.com/rider/) to add to the private [package feed configuration](https://www.jetbrains.com/help/rider/Using_NuGet.html#credential-providers-for-private-nuget-feeds).

The following `appSettings.config` file is needed in the root of `.\Web\SFB.Web.UI` before the application will be able to run locally:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<appSettings>
  <add key="webpages:Version" value="3.0.0.0" />
  <add key="webpages:Enabled" value="false" />
  <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true" />
  <add key="ClientValidationEnabled" value="true" />
  <add key="UnobtrusiveJavaScriptEnabled" value="true" />
  <add key="SessionExpireTimeSpan" value="00:05:00" />
  <add key="database" value="sfb-dev" />
  <add key="endpoint" value="https://cm-t1dv-sfb.documents.azure.com:443/" />
  <add key="authKey" value="•••" />
  <add key="SearchInstance" value="ss-t1dv-sfb" />
  <add key="SearchKey" value="•••" />
  <add key="SearchIndex" value="search" />
  <add key="SearchIndexTrust" value="search-trust" />
  <add key="GoogleAnalyticsId" value="UA-123353106-1" />
  <add key="GoogleAPIKey" value="•••" />
  <add key="GoogleTagManagerId" value="GTM-TTHL2RR" />
  <add key="DynamicHeaderContent" value="If you would like to help us improve the usability of the beta site through a testing session, please email &#x3C;a href=&#x22;mailto:school.resourcemanagement@education.gov.uk&#x22; target=&#x22;_top&#x22;&#x3E;school.resourcemanagement@education.gov.uk&#x3C;span class=&#x22;visuallyhidden&#x22;&#x3E; Email link&#x3C;/span&#x3E;&#x3C;/a&#x3E;" />
  <add key="EnableAITelemetry" value="false"/>
  <add key="EnableElmahLogs" value="false"/>
  <add key="SptApiUrl" value="https://www.find-school-performance-data.service.gov.uk" />
  <add key="GiasApiUrl" value="https://www.get-information-schools.service.gov.uk" />
  <add key="ExternalServiceCacheHours" value="0.5"/>
  <add key="SptApiUserName" value="internal" />
  <add key="SptApiPassword" value="hansgruber1988" />
  <add key="AuthenticationEnabled" value="false" />
  <add key="TestUserName" value="internal" />
  <add key="TestPassword" value="•••" />
  <add key="AzureMapsAPIKey" value="•••"/>
  <add key="RedisConnectionString" value="sfb-dev.redis.cache.windows.net:6380,password=•••,ssl=True,abortConnect=False,sslprotocols=tls12"/>
  <add key="NotifyAPIKey" value="•••"/>
  <add key="UserEmailTemplateId" value="•••"/>
  <add key="DfEEmailTemplateId" value="•••"/>
  <add key="ContactUsUserEmailTemplateId" value="•••"/>
  <add key="ContactUsDfEEmailTemplateId" value="•••"/>
  <add key="RecruitmentEmailTemplateId" value="•••"/>
  <add key="BetaSurveyUrl" value="https://dferesearch.fra1.qualtrics.com/jfe/form/SV_cUt5o7hpEyXBUOh"/>
  <add key="BMSurveyUrl" value="https://dferesearch.fra1.qualtrics.com/jfe/form/SV_1ZV7aparOQ145OR"/>
  <add key="SRMEmailAddress" value="BetterFinancialReporting.COMMS@education.gov.uk"/>
  <add key="EfficiencyMetricsUrl" value="http://localhost:4201/efficiency-metric"/>
  <add key="SelfAssessmentUrl" value="http://localhost:4200/self-assessment"/>
  <add key="cookieDomain" value="localhost"/>
  <add key="emCollection" value="20210318000000-EM-2021-2022"/>
  <add key="trustHistoryCollection" value="20210713-TrustHistory-2021-2022"/>
  <add key="SfbApiUrl" value="localhost:44383"/>
  <add key="Feature-RevisedSchoolPage-enabled" value="True"/>
  <add key="UnderReviewSchools" value="138492"/>
  <add key="UnderReviewTrusts" value="8850163"/>
  <add key="UnderReviewMessageForSchools" value="This school's finances are currently under review and may be amended."/>
  <add key="UnderReviewMessageForTrusts" value="This trust's finances are currently under review and may be amended."/>
  <add key="CosmosConnectionMode" value="Gateway" />
</appSettings>
```
