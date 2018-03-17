using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Configuration.Extending;
using OpenMLTD.MilliSim.Foundation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Configuration {
    /// <summary>
    /// Helper functions about <see cref="ConfigurationStore"/>.
    /// </summary>
    internal static class ConfigurationHelper {

        /// <summary>
        /// Creates a new <see cref="ConfigurationStore"/> from default entry file using custom YAML converters.
        /// </summary>
        /// <param name="pluginManager">The <see cref="BasePluginManager"/> that contains loaded plugins.</param>
        /// <returns>Created <see cref="ConfigurationStore"/>.</returns>
        [NotNull]
        internal static ConfigurationStore CreateConfigurationStore([NotNull] BasePluginManager pluginManager) {
            return CreateConfigurationStore(DefaultConfigurationEntryFile, pluginManager);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationStore"/> from specified entry file using custom YAML type converters.
        /// </summary>
        /// <param name="entryFilePath">Path of the entry configuration file.</param>
        /// <param name="pluginManager">The <see cref="BasePluginManager"/> that contains loaded plugins.</param>
        /// <returns>Created <see cref="ConfigurationStore"/>.</returns>
        [NotNull]
        internal static ConfigurationStore CreateConfigurationStore([NotNull] string entryFilePath, [NotNull] BasePluginManager pluginManager) {
            var typeConverterFactories = pluginManager.GetPluginsOfType<IConfigTypeConverterFactory>();

            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention());

            // External converters
            foreach (var factory in typeConverterFactories) {
                var converter = factory.CreateTypeConverter();
                deserializerBuilder.WithTypeConverter(converter);
            }

            var deserializer = deserializerBuilder.Build();

            return ConfigurationStore.Load(entryFilePath, deserializer);
        }

        private const string DefaultConfigurationEntryFile = "Contents/app.config.yml";

    }
}
