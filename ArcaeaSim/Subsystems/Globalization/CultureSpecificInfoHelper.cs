using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using Moe.Mottomo.ArcaeaSim.Configuration;
using OpenMLTD.MilliSim.Globalization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Globalization {
    /// <summary>
    /// Helper functions about <see cref="CultureSpecificInfo"/>.
    /// </summary>
    internal static class CultureSpecificInfoHelper {

        /// <summary>
        /// Creates a new <see cref="CultureSpecificInfo"/> from default entry file.
        /// </summary> 
        /// <returns>Created <see cref="CultureSpecificInfo"/>.</returns>
        [NotNull]
        internal static CultureSpecificInfo CreateCultureSpecificInfo() {
            return CreateCultureSpecificInfo(DefaultGlobalizationEntryFile);
        }

        /// <summary>
        /// Creates a new <see cref="CultureSpecificInfo"/> from specified entry file.
        /// </summary>
        /// <param name="entryFilePath">Path of the globalization configuration entry file.</param>
        /// <returns>Created <see cref="CultureSpecificInfo"/>.</returns>
        [NotNull]
        internal static CultureSpecificInfo CreateCultureSpecificInfo([NotNull] string entryFilePath) {
            var info = new CultureSpecificInfo(CultureInfo.CurrentUICulture);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            var globalizationConfigFileInfo = new FileInfo(entryFilePath);
            GlobalizationConfig config;

            using (var fileStream = File.Open(globalizationConfigFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var reader = new StreamReader(fileStream)) {
                    config = deserializer.Deserialize<GlobalizationConfig>(reader);
                }
            }

            var globalizationConfigBaseDirectory = globalizationConfigFileInfo.Directory;

            if (globalizationConfigBaseDirectory == null) {
                throw new ApplicationException("Unexpected: base directory for globalization files is null!");
            }

            foreach (var translationFileGlob in config.TranslationFiles) {
                info.TranslationManager.AddTranslationsFromGlob(globalizationConfigBaseDirectory, translationFileGlob);
            }

            return info;
        }

        private const string DefaultGlobalizationEntryFile = "Contents/globalization.yml";

    }
}
