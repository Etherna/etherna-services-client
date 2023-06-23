# Etherna Services Client

## Overview

A .Net client for consume Etherna services API.

### Packages

Etherna Services Client offers different NuGet packages with different scopes:

* **EthernaServicesClient.Common** offers raw generated clients from open api definitions, shared by all other projects.

* **EthernaServicesClient.Internal** exposes clients dedicated to internal "service to service" api requests.  
These clients are specific to internal use, consumed only by worker applications.

* **EthernaServicesClient.Internal.AspNetCore** implements a registration adapter for Asp.Net Core projects, dedicated to internal Api clients.  
Offers "client credentials" flow authentication, and manages access token expiration.

* **EthernaServicesClient.Users** exposes clients dedicated to user's api requets.  
These clients are dedicated to user's applications, where user's interaction may be required.

* **EthernaServicesClient.Users.Native** implements a registration adapter for native user applications.  
Offers user authentication with "oauth code" and "api key" flows, and manages access token expiration with etherna services clients.

## Issue reports

If you've discovered a bug, or have an idea for a new feature, please report it to our issue manager based on Jira https://etherna.atlassian.net/projects/ECC.

## Questions? Problems?

For questions or problems please write an email to [info@etherna.io](mailto:info@etherna.io).
