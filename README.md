# LatchBox

**LatchBox** is a fully decentralized token creation, vesting, locking, and launchpad platform on aelf blockchain. LatchBox aims to provides new cryptocurrency projects a faster way to launch their token on aelf blockchain. Also, LatchBox aims to protect every cryptocurrency communities and investors from rug pulls and traitorous advisors. 

## LatchBox Platform
- **App Portal** - https://aelf-testnet-side1.latchbox.io/
- **Blockchain** - aelf blockchain
    - **Network** - TESTNET
    - **Main Chain** - https://aelf-test-node.aelf.io (Main AELF) 
    - **Side Chain** - https://tdvv-test-node.aelf.io (Side tDVV) 

## Features
- **Token Creation** 
- **Token Lock** provides locking of tokens in a Smart Contract for certain period of time. This is designed for those people who loves to hold their tokens for a long time and have no intention selling it for a cheap price. 
    
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

- **Launchpad**

## Dashboard Statistics
LatchBox Platform has dashboard to track the following:
- **Nodes** - 
- **Locks** - 
- **Vestings** - 
- **Launchpads** - 

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

## Setup Guide:
- LatchBox Portal [Setup Guide](src/client-portal/README.md)