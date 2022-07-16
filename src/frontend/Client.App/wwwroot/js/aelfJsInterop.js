// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

const tokenContractName = 'AElf.ContractNames.Token';
let tokenContractAddress;

let aelf;
let nightElfInstance = null;
let walletAddress = null;

export function loadJs(sourceUrl) {
    if (sourceUrl.Length == 0) {
        console.error("Invalid source URL");
        return;
    }

    var tag = document.createElement('script');
    tag.src = sourceUrl;
    tag.type = "text/javascript";

    //tag.onload = function () {
    //    console.log("Script loaded successfully");
    //}

    tag.onerror = function () {
        console.error("Failed to load script");
    }

    document.body.appendChild(tag);
}


class NightElfCheck {
    constructor() {
        const readyMessage = 'NightElf is ready';
        let resovleTemp = null;
        this.check = new Promise((resolve, reject) => {
            if (window.NightElf) {
                resolve(readyMessage);
            }
            setTimeout(() => {
                reject({
                    error: 200001,
                    message: 'timeout / can not find NightElf / please install the extension'
                });
            }, 1000);
            resovleTemp = resolve;
        });
        document.addEventListener('NightElf', result => {
            console.log('test.js check the status of extension named nightElf: ', result);
            resovleTemp(readyMessage);
        });
    }
    static getInstance() {
        if (!nightElfInstance) {
            nightElfInstance = new NightElfCheck();
            return nightElfInstance;
        }
        return nightElfInstance;
    }
}

export async function Initialize(nodeUrl, appName) {
    const nightElfCheck = NightElfCheck.getInstance();
    var message = await nightElfCheck.check;

    // connectChain -> Login -> initContract -> call contract methods
    console.log(message);
    aelf = new window.NightElf.AElf({
        httpProvider: [
            nodeUrl,
        ],
        appName: appName,
        pure: true
    });

    console.log(aelf.getVersion());
}

export async function GetBalance() {
    const walletPayload = {
        address: walletAddress
    };

    var contractResult = await aelf.chain.contractAt(tokenContractAddress, walletPayload);

    if (contractResult) {
        const payload1 = {
            symbol: 'ELF',
            owner: walletPayload.address
        };

        var callResult = await contractResult.GetBalance.call(payload1);

        if (callResult) {
            return callResult;
        }
    }

    return null;
}

export async function ReadSmartContract(address, method, payload) {
    const walletPayload = {
        address: walletAddress
    };

    var contractResult = await aelf.chain.contractAt(address, walletPayload);

    if (contractResult) {
        var callResult = await contractResult[method].call(payload);

        if (callResult) {
            return callResult;
        }
    }

    return null;
}


export async function ExecuteSmartContract(address, method, payload) {
    const walletPayload = {
        address: walletAddress
    };

    var contractResult = await aelf.chain.contractAt(address, walletPayload);

    if (contractResult) {
        var callResult = await contractResult[method](payload);

        if (callResult) {
            return callResult.TransactionId;
        }
    }

    return null;
}

export async function GetTxStatus(txId) {
    var result = await aelf.chain.getTxResult(txId);
    return result;
};

export async function HasNightElf() {
    return !!window.NightElf;
}

export async function IsConnected() {
    //console.log(window.NightElf)
    //console.log(walletAddress)
    return !!(window.NightElf && walletAddress)
}

export async function GetAddress() {
    return walletAddress;
}

export async function Login() {
    if (aelf) {
        let result = await aelf.login({
            chainId: "AELF",
            payload: {
                method: "LOGIN",
            },
        });

        if (result.detail)
        {
            walletAddress = JSON.parse(result.detail).address;
            //const wallet1 = {
            //    address: walletAddress
            //};
            //// get chain status
            //const chainStatus = await aelf.chain.getChainStatus();
            //// get genesis contract address
            //const GenesisContractAddress = chainStatus.GenesisContractAddress;
            //// get genesis contract instance
            //const zeroContract = await aelf.chain.contractAt(GenesisContractAddress, wallet1);
            //// Get contract address by the read only method `GetContractAddressByName` of genesis contract
            //tokenContractAddress = await zeroContract.GetContractAddressByName.call(AElf.utils.sha256(tokenContractName));
            return true;
        }

        return false;
    }

    return null;
}

export async function Logout() {
    if (aelf && walletAddress) {
        var result = await aelf.logout({
            chainId: "AELF",
            address: walletAddress,
        });

        walletAddress = null;
    }
}

export async function Test() {
    // get chain status
    const chainStatus = await aelf.chain.getChainStatus();
    console.log(chainStatus);
    // get genesis contract address
    // const GenesisContractAddress = chainStatus.GenesisContractAddress;
    // get genesis contract instance
    //const zeroContract = await aelf.chain.contractAt(GenesisContractAddress, newWallet);
    // Get contract address by the read only method `GetContractAddressByName` of genesis contract
    //tokenContractAddress = await zeroContract.GetContractAddressByName.call(AElf.utils.sha256(tokenContractName));
};