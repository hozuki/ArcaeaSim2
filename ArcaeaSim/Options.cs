using CommandLine;

namespace Moe.Mottomo.ArcaeaSim {
    public sealed class Options {

        [Option("debug", HelpText = "Enable debug mode", Required = false, Default = false)]
        public bool IsDebugEnabled { get; set; }

        [Option("editor_server_uri", HelpText = "BVSP editor server URI", Required = false, Default = null)]
        public string EditorServerUri { get; set; }

        [Option("bvsp_port", HelpText = "BVSP simulator server port override", Required = false, Default = 0)]
        public int SimulatorServerPort { get; set; }

    }
}
