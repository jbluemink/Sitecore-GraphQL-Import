# Sitecore GraphQL Import

Easily migrate content to Sitecore XM Cloud using the Sitecore Authoring and Management GraphQL API.
See [Seamles Content Migrating with GraphQL](https://uxbee.eu/insights/seamless-content-migration-with-graphql)

## Overview
This repository provides a C# demo of the Sitecore GraphQL API. The included console app showcases various capabilities, such as retrieving Sitecore items, obtaining a list of websites, inserting sample items, and uploading media files. To get started, simply fill in your hostname, API key, and access token in the app.config file.

<b>Please Note:</b> This tool is not limited to Sitecore XM Cloud and is compatible with Sitecore 10.3 and later.

## Sitecore Authoring and Management GraphQL API
This API empowers you to perform mutations on your Sitecore instance.<br/>
Authentication: Bearer token

## Sitecore Edge Preview GraphQL IDE (CM) API
With this API, you can read items from the master database.<br/>
Authentication: GUID (located at /sitecore/system/Settings/Services/API Keys)

## Get access token
See [Walkthrough: Enabling and authorizing requests to the Authoring and Management API](https://doc.sitecore.com/xmc/en/developers/xm-cloud/walkthrough--enabling-and-authorizing-requests-to-the-authoring-and-management-api.html)
you can also retrieve it from the .sitecore/user.json file if you have a valid connection in PowerShell with XM Cloud (using 'dotnet sitecore login'). The access token, bearer token is a string of about 5KB in length.

## Get API key
(GUID), which can be found below /sitecore/system/Settings/Services/API Keys in Sitecore
