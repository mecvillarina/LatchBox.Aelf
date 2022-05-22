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
            },
            "App": {
                "Name": "LatchBox",
                "JwtAudience": "LatchBoxUsers",
                "JwtIssuer": "LatchBoxPortal",
                "JwtSecret": "[JWTSECRET]"
            },
            "AELF": {
                "MainChainNode": "https://aelf-test-node.aelf.io",
                "SideChainNode": "https://tdvv-test-node.aelf.io",
                "MainChainExplorer": "https://explorer-test.aelf.io",
                "SideChainExplorer": "https://explorer-test-side01.aelf.io",
                "MultiTokenContractAddress": "AElf.ContractNames.Token",
                "FaucetContractAddress": "2M24EKAecggCnttZ9DUUMCXi4xC67rozA87kFgid9qEwRUMHTs",
                "MultiCrowdSaleContractAddress": "2cGT3RZZy6UJJ3eJPZdWMmuoH2TZBihvMtAtKvLJUaBnvskK2x",
                "LockTokenVaultContractAddress": "2q7NLAr6eqF4CTsnNeXnBZ9k4XcmiUeM61CLWYaym6WsUmbg1k",
                "VestingTokenVaultContractAddress": "22tVtWLFwGxFu5Xk5rQgCdQnmsNA7PpTzZbkpGr1REgt5GEaN5"
            }
        }
      ```
    - Set `[JWTSECET]` - to any value you want, make sure that the dev and production [JWTSECET] is not the same. 

- Set **Client** Project as Startup Project
- Click Run or Press F5.
- If you wish to publish the app to Azure App Service, please see [Guide](https://docs.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-aspnet-web-app?view=vs-2022&tabs=azure)
