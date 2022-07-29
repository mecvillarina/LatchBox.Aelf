using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Routes
{
    public static class CrossChainOperationEndpoints
    {
        public const string Create = "api/crosschain/operation/create";
        public const string GetPending = "api/crosschain/operation/pending?from={0}&issueChainId={1}&contractName={2}";
    }
}
