# Microsoft OPC Publisher and Azure Industrial IoT Platform

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Build Status](https://msazure.visualstudio.com/One/_apis/build/status/Custom/Azure_IOT/Industrial/Components/Azure.Industrial-IoT?branchName=main)](https://msazure.visualstudio.com/One/_build/latest?definitionId=86580&branchName=main)

## Discover, register and manage your OPC UA enabled Assets with Azure

Microsoft [OPC Publisher](docs/modules/publisher.md) and the optional Azure Industrial IoT companion web service allow you to discover and operate OPC UA enabled industrial assets.

OPC Publisher enables you to access and harness the power of OPC UA and Azure IoT. OPC Publisher is a fully compliant OPC UA PubSub telemetry publisher (both JSON and UADP binary) and provides a large set of the OPC UA services through its control plane. OPC Publisher is an Azure IoT Edge module that runs on on-premises. OPC Publisher API can be accessed via HTTP(s), an MQTT Broker or through Azure IoT Hub device methods.  

If your industrial equipment doesn't support OPC UA, we have worked with our large partner network to support all types of industrial interfaces through the use of adapters, fully integrated with our platform. Please check out the [Azure IoT Edge Marketplace](https://azuremarketplace.microsoft.com/marketplace/apps/category/internet-of-things?page=1&subcategories=iot-edge-modules).

The companion cloud service with REST interface runs Azure App Service and provides a cloud side REST API to command the OPC Publisher at the edge. Microsoft provides pre-built Docker containers in the Microsoft Container Registry (MCR) for both edge and cloud components. An easy-to-use deployment script guides you step-by-step through deploying the cloud service and dependencies, as well as an optional simulation environment in Azure.

## Get started

* Learn about [OPC Publisher](docs/opc-publisher/readme.md) and how to operate it in production.
* Check out all of the [Industrial IoT Platform documentation](docs/readme.md).
* [Release announcements](docs/release-announcement.md) and all [releases of the platform](https://github.com/Azure/Industrial-IoT/releases).

> If you are using 2.8 LTS today and are looking for the corresponding documentation and code then you will find it in the [latest 2.8 branch](https://github.com/Azure/Industrial-IoT/tree/release/2.8.6).

## Get support

Please report any security related issues by following our [security process](security.md).

If you are an Azure customer, please create an Azure Support Request. More information can be found [here](https://azure.microsoft.com/en-us/support/create-ticket/). (Azure Support SLA applies).

Otherwise, please report bugs, feature requests, or suggestions as [GitHub issues](https://github.com/Azure/Industrial-IoT/issues) (No SLA available).

### Supported releases and support policy

Our releases are tagged following semantic versioning (“semver”) conventions. Minor and patch releases do not break backwards compatibility. Minor releases and patch releases address customer reported issues and receive security updates. Minor releases are either regular releases or LTS (Long term support) releases. Currently only the 2.8 release is receiving long-term support (2 years of support after release, only security fixes in the second year). Regular (non-LTS) releases are supported for 6 months after the subsequent major.minor release, or for 1 year after its release whichever comes first. This is reflected in the following table. Releases not shown in the table (e.g., 2.3, 2.4, or 2.6) are out of support already.

| Release (tag)                               | Latest patch release (tag) | End of support | Successor (tag)       | Update instructions                                       |
|---------------------------------------------|----------------------------|----------------|-----------------------|-----------------------------------------------------------|
| 2.5                                         | 2.5.5                      | 12/1/2021      | 2.8  (LTS) (>= 2.8.2) | [Migration Path](docs/modules/publisher-migrationpath.md) |
| 2.7                                         | 2.7.206                    | 1/15/2022      | 2.8  (LTS) (>= 2.8.0) | N/A                                                       |
| 2.8 (LTS)                                   | [2.8.6](https://github.com/Azure/Industrial-IoT/tree/release/2.8.6) | 7/15/2023      | 2.9                   | N/A                                                       |
| [2.9](https://github.com/Azure/Industrial-IoT)| 2.9.0-preview2             | N/A            | 2.10 (LTS)            | [Migration Path](docs/modules/publisher-migrationpath.md) |

We only support the latest patch version of a release which per semantic versioning convention is identified by the 3rd part of the version string. Preview releases are only supported through GitHub issues. If you are using a container image with a major.minor version tag that is supported per above table, but a patch version lower than the latest patch version, you need to update your images to the latest version to ensure secure operation and take advantage of the latest fixes. If you unexpectedly encounter bugs and require help, please ensure you are running the latest patch release as we might already have addressed the issue you are seeing. If you are not, please update first and try to reproduce the issue on the latest patch version.

Security-critical updates are made to the last patch version of the major.minor release containing the vulnerability. Bug fixes that are not security related are made only to the main branch and to the last supported release. The version the fix will be in can be found in the version.json file of the respective branch.  

Our [official Microsoft support](https://azure.microsoft.com/en-us/support/create-ticket/) and any related SLA only covers officially released docker containers obtained from MCR (Microsoft Container Registry) and deployed to Azure and hosted by Azure Kubernetes Service (AKS) or IoT Edge (in case of OPC Twin, OPC Publisher and OPC Discovery module images) using the documentation and IAI tool provided as part of the latest release. Also, all Azure services deployed, the installed IoT Edge runtime, as well as Operating System and other middleware and combinations thereof must be officially supported as per their published support policy and SLA.

Otherwise, support is provided on a best effort basis through [GitHub issues](https://github.com/Azure/Industrial-IoT/issues). We aim to release patch releases on a regular cadence (approximately every 3 months), so if you are blocked, and you can suggest or contribute fixes, the chances of getting it into the next patch release are high.

## Contribute

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

If you want/plan to contribute, we ask you to sign a [CLA](https://cla.microsoft.com/) (Contribution License Agreement) and follow the project 's [code submission guidelines](contributing.md). A friendly bot will remind you about it when you submit a pull-request.

## License

Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the [MIT](LICENSE) License.  
