# LatchBox App Portal Guide

## Prerequisites
- C#/.NET experience at least immediate level
- Knowledge on Blazor Server
- Knowledge on Provisioning Azure Resources 
- Local installations of the .NET SDK (6.0) and Visual Studio 2022

## Azure Cloud Prerequisites
- Create or Provision Azure App Service

## Instruction
- After completing the Azure Cloud Prerequisites
- Open LatchBox.Portal.sln on Visual Studio
- On **Client** Project, find the file named **appsettings.json** and make sure that the content of it is same with the one below:
	- ```json
		{
            "Logging": {
                "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
                }
            },
            "AllowedHosts": "*",
            "Azure": {
                "SignalR": {
                "Enabled": "true"
                }
            }
        }
      ```

- Set **Client** Project as Startup Project
- Click Run or Press F5.
- If you wish to publish the app to Azure App Service, please see [Guide](https://docs.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-aspnet-web-app?view=vs-2022&tabs=azure)
