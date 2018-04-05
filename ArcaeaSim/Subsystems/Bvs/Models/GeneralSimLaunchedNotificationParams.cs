using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class GeneralSimLaunchedNotificationParams {

        [JsonConstructor]
        internal GeneralSimLaunchedNotificationParams() {
        }

        [JsonProperty("server_uri")]
        public string SimulatorServerUri { get; set; }

    }
}
