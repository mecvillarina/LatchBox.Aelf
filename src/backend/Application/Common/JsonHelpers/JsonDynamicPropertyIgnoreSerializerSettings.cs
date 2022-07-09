using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.Common.JsonHelpers
{
    public class JsonDynamicPropertyIgnoreSerializerSettings : JsonSerializerSettings
    {
        public JsonDynamicPropertyIgnoreSerializerSettings(params string[] ignoreProperties)
        {
            ContractResolver = new JsonDynamicPropertyIgnoreResolver(new List<string>(ignoreProperties));
        }
    }
}
