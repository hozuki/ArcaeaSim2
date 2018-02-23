using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moe.Mottomo.ArcaeaSim.Components;
using Moe.Mottomo.ArcaeaSim.Configuration;
using Moe.Mottomo.ArcaeaSim.Subsystems.Interactive;
using Moe.Mottomo.ArcaeaSim.Subsystems.Plugin;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;
using SharpAL.OpenAL;

namespace Moe.Mottomo.ArcaeaSim {
    /// <inheritdoc />
    /// <summary>
    /// Arcaea application.
    /// </summary>
    public sealed class ArcaeaSimApplication : BaseGame {

        public ArcaeaSimApplication([NotNull] BasePluginManager pluginManager, [NotNull] ConfigurationStore configurationStore, [NotNull] CultureSpecificInfo cultureSpecificInfo)
            : base("Contents", pluginManager) {
            ConfigurationStore = configurationStore;
            CultureSpecificInfo = cultureSpecificInfo;

            ApplyConfiguration();
        }

        public override Stage Stage => _stage;

        public override ConfigurationStore ConfigurationStore { get; }

        public override CultureSpecificInfo CultureSpecificInfo { get; }

        protected override void Initialize() {
            AppendComponents();
            AppendExtensionComponents();

            CenterWindowAndSetTitle();

            base.Initialize();

            InitializeExtensionComponents();
        }

        private void ApplyConfiguration() {
            var graphicsManager = GraphicsDeviceManager;
            var config = ConfigurationStore.Get<MainAppConfig>();

            graphicsManager.PreferredBackBufferWidth = config.Data.Window.Width;
            graphicsManager.PreferredBackBufferHeight = config.Data.Window.Height;
            graphicsManager.PreferredDepthStencilFormat = DepthFormat.Depth16;
            graphicsManager.SynchronizeWithVerticalRetrace = true;
            graphicsManager.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;
        }

        private void AppendComponents() {
            var keyboardStateHandler = new KeyboardStateHandler(this);

            Components.Add(keyboardStateHandler);

            Components.Add(new MouseCameraControl(this));

            keyboardStateHandler.KeyHold += (s, e) => {
                var cam = this.FindSingleElement<Cameraman>()?.Camera;

                if (cam == null) {
                    return;
                }

                switch (e.KeyCode) {
                    case Keys.A:
                        cam.Strafe(-0.1f);
                        break;
                    case Keys.D:
                        cam.Strafe(0.1f);
                        break;
                    case Keys.W:
                        cam.Walk(0.1f);
                        break;
                    case Keys.S:
                        cam.Walk(-0.1f);
                        break;
                    case Keys.Left:
                        cam.Yaw(0.01f);
                        break;
                    case Keys.Right:
                        cam.Yaw(-0.01f);
                        break;
                    case Keys.Up:
                        cam.Pitch(-0.01f);
                        break;
                    case Keys.Down:
                        cam.Pitch(0.01f);
                        break;
                    case Keys.R: {
                            var camera = this.FindSingleElement<Cameraman>()?.Camera;
                            camera?.Reset();
                        }
                        break;
                }
            };

            keyboardStateHandler.KeyDown += (s, e) => {
                switch (e.KeyCode) {
                    case Keys.Space: {
                            var audioController = this.FindSingleElement<AudioController>();
                            var music = audioController?.Music;

                            if (music == null) {
                                break;
                            }

                            var isPlaying = music.Source.State == ALSourceState.Playing;

                            if (!isPlaying) {
                                music.Source.PlayDirect();
                            } else {
                                music.Source.Pause();
                            }

                            var helpOverlay = this.FindSingleElement<HelpOverlay>();

                            if (helpOverlay != null) {
                                helpOverlay.Visible = isPlaying;
                            }
                        }
                        break;
                }
            };
        }

        private void AppendExtensionComponents() {
            var stage = new Stage(this, ConfigurationStore);
            _stage = stage;

            Components.Add(stage);

            var pluginManager = (ArcaeaSimPluginManager)PluginManager;

            var instantiatedIDList = new List<string>();

            foreach (var factoryID in pluginManager.InstancingFactoryIDs) {
                var factory = pluginManager.GetPluginByID<IBaseGameComponentFactory>(factoryID);
                var component = factory?.CreateComponent(this, stage);

                if (component != null) {
                    stage.Components.Add(component);
                    instantiatedIDList.Add(factoryID);
                }
            }

            if (instantiatedIDList.Count > 0) {
                GameLog.Debug("Instantiated component factories: {0}", string.Join(", ", instantiatedIDList));
            } else {
                GameLog.Debug("No component factory instantiated.");
            }
        }

        // For Direct3D, who fails to center the window on startup.
        private void CenterWindowAndSetTitle() {
            var windowBounds = Window.ClientBounds;
            var displayMode = GraphicsDevice.Adapter.CurrentDisplayMode;

            Window.Position = new Point((displayMode.Width - windowBounds.Width) / 2, (displayMode.Height - windowBounds.Height) / 2);

            var config = ConfigurationStore.Get<MainAppConfig>();

            string songTitle;
            if (ConfigurationStore.TryGetValue<BeatmapLoaderConfig>(out var scoreLoaderConfig)) {
                songTitle = scoreLoaderConfig.Data.Title;
            } else {
                songTitle = null;
            }

            var appCodeName = ApplicationHelper.CodeName;
            var windowTitle = config.Data.Window.Title;

            if (!string.IsNullOrWhiteSpace(songTitle)) {
                windowTitle = songTitle + " - " + windowTitle;
            }

            if (!string.IsNullOrWhiteSpace(appCodeName)) {
                windowTitle = windowTitle + " (\"" + appCodeName + "\")";
            }

            Window.Title = windowTitle;
        }

        private void InitializeExtensionComponents() {
            var helpOverlay = this.FindSingleElement<HelpOverlay>();

            if (helpOverlay != null) {
                helpOverlay.Visible = true;
            }

            var audioController = this.FindSingleElement<AudioController>();
            var music = audioController?.Music;

            if (music != null) {
                music.Source.PlaybackStopped += (s, e) => {
                    if (helpOverlay != null) {
                        helpOverlay.Visible = true;
                    }
                };
            }
        }

        private Stage _stage;

    }
}
