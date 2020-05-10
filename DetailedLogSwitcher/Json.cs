using Newtonsoft.Json;

namespace DetailedLogSwitcher
{
    public class GlobalSettings
    {

        [JsonProperty("clnMainGroup")]
        public string ClnMainGroup { get; set; }

        [JsonProperty("clnSubGroup")]
        public string ClnSubGroup { get; set; }

        [JsonProperty("clnName")]
        public string ClnName { get; set; }

        [JsonProperty("clnType")]
        public string ClnType { get; set; }

        [JsonProperty("clnValue")]
        public string ClnValue { get; set; }
    }
}