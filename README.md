# Schools Financial Benchmarking

> ⚠️ This service is no longer being actively maintained and will be superseded by [DFE-Digital/education-benchmarking-and-insights](https://github.com/DFE-Digital/education-benchmarking-and-insights) in due course.

[![Build Status](https://agilefactory.visualstudio.com/Financial%20Benchmarking/_apis/build/status/Web%20App/Schools.Financial.Benchmarking)](https://agilefactory.visualstudio.com/Financial%20Benchmarking/_build?definitionId=471)
[![GitHub release (latest by date)](https://agilefactory.vsrm.visualstudio.com/_apis/public/Release/badge/fc33e3f0-e73b-466d-837a-10cad68c664e/4/14)](https://agilefactory.visualstudio.com/Financial%20Benchmarking/_release?definitionId=4)

## Introduction

This application is build using ASP.NET MVC with .Net Framework v4.7 referencing a CosmosDB database NoSQL management system hosted on Microsoft Azure.

## Database Dependencies

[Data Migration Docs](data-migration.md)

[Disaster Recovery Docs](disaster-recovery.md)

## Dev setup (Front end assets)

### Prerequisites

The following tools may be installed directly on Windows, or via Chocolatey. [Chocolatey](https://chocolatey.org/) is advised when installing [Node](https://nodejs.org/en) via the latest downloaded binaries and will install the VS and Python dependencies below automatically.

> **NOTE:** This solution will not build on non-Windows boxes due to the dependency on C++ binaries used by `node-gyp`

1. [`nvm` for Windows](https://github.com/coreybutler/nvm-windows)
1. [Visual Studio 2019 Build Tools](https://my.visualstudio.com/Downloads?q=visual%20studio%202019&wt.mc_id=o~msft~vscom~older-downloads)
1. [Python 3.7](https://www.python.org/downloads/release/python-379/)

Developers may also find it useful to [exclude certain paths from Windows Defender](https://gist.github.com/Braytiner/be2497d1a06f5a9d943dc7760693d460) in the case of local performance issues.

### Steps

1. Open web folder: `cd .\Web\SFB.Web.UI\`
1. Install correct `node` version: `nvm install 18.12.0`
1. Set `node` version: `nvm use 18.12.0`
1. Install `grunt` globally: `npm i grunt -g`
1. Install `node-gyp` globally: `npm i node-gyp -g`
1. Install `sass` globally: `npm i sass -g`
1. Install deps: `npm i`
1. Build assets: `npm run build:newAndLegacyDev`

> **NOTE:** The `webpack` build is slow and as at 2024 there will be a large number of warnings about deprecated packages. As long as `./public/assets/scripts/application.js` has been generated then build has been successful.

## Dev setup (.NET)

When restoring packages in the .NET Framework solution, the private package feed in DevOps will need to be authenticated with. If the built-in credential manager does not work (with `401 Unauthoized` errors from NuGet), then [try a PAT](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate) instead and use [Rider](https://www.jetbrains.com/rider/) to add to the private [package feed configuration](https://www.jetbrains.com/help/rider/Using_NuGet.html#credential-providers-for-private-nuget-feeds). Alternatively, register the PAT in the roaming config file at `%AppData%\NuGet\NuGet.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="SFB.Artifacts" value="https://agilefactory.pkgs.visualstudio.com/fc33e3f0-e73b-466d-837a-10cad68c664e/_packaging/SFB.Artifacts/nuget/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <SFB.Artifacts>
      <add key="Username" value="<USERNAME>" />
      <add key="ClearTextPassword" value="<PAT>" />
    </SFB.Artifacts>
  </packageSourceCredentials>
</configuration>
```

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
  <add key="trustHistoryCollection" value="TrustHistory-20230831"/>
  <add key="SfbApiUrl" value="localhost:44383"/>
  <add key="Feature-RevisedSchoolPage-enabled" value="True"/>
  <add key="UnderReviewSchools" value="138492"/>
  <add key="UnderReviewTrusts" value="8850163"/>
  <add key="UnderReviewMessageForSchools" value="This school's finances are currently under review and may be amended."/>
  <add key="UnderReviewMessageForTrusts" value="This trust's finances are currently under review and may be amended."/>
  <add key="CosmosConnectionMode" value="Gateway" />
</appSettings>
```

### `DeprecationInformation` feature

Additional configuration may also be included in `appSettings.config` to manage the service deprecation content. These should also be set as required on deployed app services.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<appSettings>
  <!-- ... -->
  <add key="DeprecationInformation:Enabled" value="true" />
  <add key="DeprecationInformation:Title" value="Schools financial benchmarking will no longer be updated with new data" />
  <add key="DeprecationInformation:Body" value="Find the latest data on the new [Financial Benchmarking and Insights Tool](https://financial-benchmarking-and-insights-tool.education.gov.uk/){.govuk-notification-banner__link}.\nThis will include 2023-24 data for maintained schools.\n\nThis service will include all previous financial data available too." />
  <add key="DeprecationInformation:NewServiceUrl" value="https://financial-benchmarking-and-insights-tool.education.gov.uk/" />
  <add key="DeprecationInformation:OldServiceLinkText" value="Continue to schools financial benchmarking (legacy service)" />
</appSettings>
```

> 💡 The `DeprecationInformation:Body` value is [CommonMark](https://spec.commonmark.org/) markdown with the addition of [GenericAttributes](https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md) to help manage GDS styles.
