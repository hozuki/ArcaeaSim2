using System;
using System.Diagnostics;
using CommandLine;
using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Subsystems.Configuration;
using Moe.Mottomo.ArcaeaSim.Subsystems.Globalization;
using Moe.Mottomo.ArcaeaSim.Subsystems.Plugin;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace Moe.Mottomo.ArcaeaSim {
#if WINDOWS || LINUX
    /// <summary>
    /// The main program class.
    /// </summary>
    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main([NotNull, ItemNotNull] string[] args) {
            BaseGame.GraphicsBackend = GraphicsBackend.Direct3D11;

            GameLog.Initialize("arcaea-debug");
            GameLog.Enabled = true;

            var exitCode = -1;

            var parser = new Parser(settings => {
                settings.IgnoreUnknownArguments = true;
                settings.CaseInsensitiveEnumValues = true;
            });

            var optionsParsingResult = parser.ParseArguments<Options>(args);

            try {
                if (optionsParsingResult.Tag == ParserResultType.Parsed) {
                    var options = ((Parsed<Options>)optionsParsingResult).Value;

                    // Enable game log if the app is launched with "--debug" switch.
                    GameLog.Enabled = options.IsDebugEnabled;

                    using (var pluginManager = new ArcaeaSimPluginManager()) {
                        pluginManager.LoadPlugins();

                        var configurationStore = ConfigurationHelper.CreateConfigurationStore(pluginManager);
                        var cultureSpecificInfo = CultureSpecificInfoHelper.CreateCultureSpecificInfo();

                        using (var game = new ArcaeaSimApplication(pluginManager, configurationStore, cultureSpecificInfo)) {
                            game.Run();
                        }

                        exitCode = 0;
                    }
                } else {
                    var helpText = CommandLine.Text.HelpText.AutoBuild(optionsParsingResult);

                    GameLog.Info(helpText);
                }
            } catch (Exception ex) {
                GameLog.Error(ex.Message);
                GameLog.Error(ex.StackTrace);
                Debug.Print(ex.ToString());
            }

            return exitCode;
        }

    }
#endif
}
