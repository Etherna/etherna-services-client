# Etherna SDK .Net

## Overview

A .Net client to consume Etherna services API.

### Packages

Etherna SDK .Net offers different NuGet packages with different scopes:

* **EthernaSdk.Common** offers raw generated clients from open api definitions, shared by all other projects.

* **EthernaSdk.Internal** exposes clients dedicated to internal "service to service" api requests.  
These clients are specific to internal use, consumed only by worker applications.

* **EthernaSdk.Internal.AspNetCore** implements a registration adapter for Asp.Net Core projects, dedicated to internal Api clients.  
Offers "client credentials" flow authentication, and manages access token expiration.

* **EthernaSdk.Users** exposes clients dedicated to user's api requets.  
These clients are dedicated to user's applications, where user's interaction may be required.

* **EthernaSdk.Users.Native** implements a registration adapter for native user applications.  
Offers user authentication with "oauth code" and "api key" flows, and manages access token expiration with etherna services clients.

## Issue reports

If you've discovered a bug, or have an idea for a new feature, please report it to our issue manager based on Jira https://etherna.atlassian.net/projects/ECC.

## Questions? Problems?

For questions or problems please write an email to [info@etherna.io](mailto:info@etherna.io).

## License

![LGPL Logo](https://www.gnu.org/graphics/lgplv3-with-text-154x68.png)

We use the GNU Lesser General Public License v3 (LGPL-3.0) for this project.
If you require a custom license, you can contact us at [license@etherna.io](mailto:license@etherna.io).
