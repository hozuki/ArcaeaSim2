using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Subsystems.Bvs.Models.Proposals;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Bvs.Models {
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class GeneralSimInitializeResponseResult {

        [JsonConstructor]
        internal GeneralSimInitializeResponseResult() {
        }

        [CanBeNull]
        public SelectedFormatDescriptor SelectedFormat { get; set; }

    }
}
