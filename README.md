# LatchBox

**LatchBox** is a fully decentralized token creation, vesting, locking, and launchpad platform on aelf blockchain. LatchBox aims to provides new cryptocurrency projects a faster way to launch their token on aelf blockchain. Also, LatchBox aims to protect every cryptocurrency communities and investors from rug pulls and traitorous advisors. 

## LatchBox Platform
- **App Portal** - https://aelf-testnet-side1.latchbox.io/
- **Blockchain** - aelf blockchain
    - **Network** - TESTNET
    - **Main Chain** - https://aelf-test-node.aelf.io (Main AELF) 
    - **Side Chain** - https://tdvv-test-node.aelf.io (Side tDVV) 

## Features
- **Token Creation** - token creation on aelf is not a new feature, but LatchBox made it much easier. Anyone can create a token both on main chain and perform a cross chain token creation on side chain. 
    
    Anyone can: 
    - **Create New Token**, when someone created a token, LatchBox will create it on main chain but the issued chain id value will be the side tDVV chain id.
    - **Add Existing Token**, when someone knows the token symbol of the token, they can add it on the portal to see the token balance. If you're the token issuer and your token exists **Main AELF** chain and not on **Side tDVV** chain, LatchBox will send a transaction to token contract `CrossChainCreateToken` method on side tDVV chain.
    - **Issue Token**, if you're the token issuer of a specific token, you can able to issue/mint new tokens.

- **Token Lock** provides locking of tokens in a smart contract for certain period of time. This is designed for those people who loves to hold their tokens for a long time and have no intention selling it for a cheap price. 
    
    The `initiator` of the lock can:
    - **Add Lock**, the initiator can choose the token to be locked, define the unlock date, option to make it revocable anytime, and the token amount and address of each receiver of the lock.
    - **Revoke Lock**, the initiator can revoke the lock anytime only if it is defined on **Add Lock** that the lock was revocable and all receiver doesn't claim it yet.
    - **Claim Refund**, after revoking the lock, only unclaimed token can be refunded.

    The `receiver` of the lock can:
    - **Claim Lock**, the `receiver` of a lock can claim their token when the unlock date has passed and given that the initiator of the lock doesn't revoke it.

    And anyone can view:
    - **Lock Previewer**, it contains the lock details including the receivers' details (amount and address) and it has a link that is shareable and publicly viewable.

- **Token Vesting** is similar with token lock but it supports multiple unlock periods. This is designed for newly launched/upcoming cryptocurrency projects on aelf blockchain that underwent/will undergo presale/ICO and have vesting period for the releases of their token for their investors without manually releasing it to them. Also, this could be use to lock team allocated tokens and only unlock on the promised period and to gain and keep the trust of their community to them.
    
    The `initiator` of the vesting can:
    - **Add Vesting**, the initiator can choose the token to be vested, define the periods and option to make it revocable anytime. For every period, the initiator can define unlock date and the token amount and address of each receivers of that period.
    - **Revoke Vesting**, the initiator can revoke the vesting anytime only if it is defined on **Add Vesting** that the vesting was revocable and all receiver doesn't claim it yet.
    - **Claim Refund**, after revoking the vesting, only unclaimed token can be refunded.

    The `receiver` of the vesting can:
    - **Claim Vesting**, the `receiver` of a specific vesting period can claim their token when the unlock date has passed and given that the initiator of that vesting doesn't revoke it.

    And anyone can view:
    - **Vesting Previewer**, it contains the vesting details including the period timeline and the receivers' details and it has a link that is shareable and publicly viewable.

- **Launchpad** provides new cryptocurrency project on aelf to launch their token. 

    Token issuer can:
    - **Create New Launchpad**, the token issuer can create a launchpad to raise funds (ELF). They need to specify the token conversation, sale duration and soft/hard cap of the token sale. A smart contract will hold the total amount of new token based from ELF to token conversation.
    - **Cancel Launchpad**, the token issuer can anytime cancel a launchpad, LatchBox will return the raised funds (ELF) to all investors and the token to the token issuer.
    - **Complete Launchpad**, if the soft or hard cap has met and the sale was ended, the token issuer can complete the sale and will get the raised funds. The total token amount that will be distributed to investors will be locked using LatchBox's **Token Lock** feature, based on the lockup time specified on the token sale. The excess token on the sale will be refunded to the token issuer.
    - **Refund Launchpad**, if the soft cap has not met and the sale was ended, the token issuer can issue a refund by click only a button. LatchBox will return the raised funds to all investors and the token to the token issuer of that sale0. 

    Anyone can be an investor and can perform the following:
    - **Invest**, anyone can invest their ELF. Each token sale has a limit purchase for each wallet address specified by the token issuer/launchpad creator. All the token sale details has all the information and should be helpful to anyone that wants to invest on a token sale. The smart contract will hold the raised funds made by the investors.
    - **Claim Lock**, after the token sale, if the sale goal has met, and token issuer completed the token sale, the investor can check their investments on the **Invested Launchpad**. Each successful token sale will display a Lock Id that has details about when the token can be claim. Or simply they can go to **Locks/My Claims** section to check their claims.

    and anyone can view:
    - **Launchpad Viewer**, it contains the token sale details including the investments made by anyone, and it has a link that is shareable and publicly viewable.

## Dashboard Statistics
LatchBox Platform has dashboard to track the following:
- **Nodes** - Blockchain Node Information 
- **Locks** - Lock Token Vault Contract, total locks, # of Locked Tokens  
- **Vestings** - Vesting Token Vault Contract, total created vestings
- **Launchpads** - Launchpad Contract, total # of upcoming ssale, total # of ongoing sales, total # of created token sales.  

## Smart Contracts (Testnet)
- **Lock Token Vault Contract** - `2q7NLAr6eqF4CTsnNeXnBZ9k4XcmiUeM61CLWYaym6WsUmbg1k` [[See Code](src/chain/contract/LatchBox.Contracts.LockTokenVaultContract/)] [[See Explorer](https://explorer-test-side01.aelf.io/address/2q7NLAr6eqF4CTsnNeXnBZ9k4XcmiUeM61CLWYaym6WsUmbg1k)]
- **Vesting Token Vault Contract** - `22tVtWLFwGxFu5Xk5rQgCdQnmsNA7PpTzZbkpGr1REgt5GEaN5` [[See Code](src/chain/contract/LatchBox.Contracts.VestingTokenVaultContract/)] [[See Explorer](https://explorer-test-side01.aelf.io/address/22tVtWLFwGxFu5Xk5rQgCdQnmsNA7PpTzZbkpGr1REgt5GEaN5)]
- **Launchpad Contract** - `2cGT3RZZy6UJJ3eJPZdWMmuoH2TZBihvMtAtKvLJUaBnvskK2x` [[See Code](src/chain/contract/LatchBox.Contracts.MultiCrowdSaleContract/)] [[See Explorer](https://explorer-test-side01.aelf.io/address/2cGT3RZZy6UJJ3eJPZdWMmuoH2TZBihvMtAtKvLJUaBnvskK2x)]

## Technology Stack & Tools
- Cloud Service Provider: Microsoft Azure
- Web Frontend: 
    - IDE: Visual Studio 2022
    - Web Framework: Blazor Server/.NET 6
	- C# as Programming Language 
	- Deployed on Azure App Service.
- Smart Contract:
    - IDE: Visual Studio 2022
    - C# as Programming Language
    - [Aelf Boilerplate](https://github.com/AElfProject/aelf-boilerplate)
    - Deployment: aelf-command

## Setup Guide:
- LatchBox Portal [Setup Guide](src/client-portal/README.md)