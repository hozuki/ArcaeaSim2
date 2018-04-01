using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Configuration;
using Moe.Mottomo.ArcaeaSim.Subsystems.Rendering;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace Moe.Mottomo.ArcaeaSim.Components {
    /// <summary>
    /// Track renderer component.
    /// Deriving from <see cref="BufferedVisual"/> but not <see cref="Visual"/> is because we need a <see cref="RenderTarget2D"/> with depth buffer, but the backbuffer (the screen) does not have a depth buffer.
    /// </summary>
    public sealed class TrackDisplay : BufferedVisual {

        public TrackDisplay([NotNull] BaseGame game, [CanBeNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _stageMetrics = new StageMetrics();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var game = Game.ToBaseGame();

            _basicEffect = new BasicEffect(game.GraphicsDevice);
            game.EffectManager.RegisterSingleton(_basicEffect);

            // Hack: register note effect list
            NoteEffects.Effects[(int)NoteType.Floor] = _basicEffect;
            NoteEffects.Effects[(int)NoteType.Long] = _basicEffect;
            NoteEffects.Effects[(int)NoteType.Arc] = _basicEffect;
            NoteEffects.Effects[(int)NoteType.Sky] = _basicEffect;

            var beatmap = Game.FindSingleElement<BeatmapLoader>()?.Beatmap;

            if (beatmap != null) {
                _beatmap = new VisualBeatmap(game.GraphicsDevice, beatmap, _stageMetrics);
            }

            var config = ConfigurationStore.Get<TrackDisplayConfig>();

            _panelTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.PanelTexture);
            _trackLaneDividerTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.TrackLaneDividerTexture);
            _skyInputTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.SkyInputTexture);

            _noteTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.NoteTexture);
            _noteHoldTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.NoteHoldTexture);
            _noteHoldHighlightedTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.NoteHoldHighlightedTexture);
            _noteSkyTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.NoteSkyTexture);
            _noteArcTexture = ContentHelper.LoadTexture(game.GraphicsDevice, config.Data.NoteArcTexture);

            var metrics = _stageMetrics;

            {
                _trackRectangle = new TexturedRectangle(game.GraphicsDevice);
                _trackRectangle.SetVerticesXY(new Vector2(-metrics.HalfTrackFullWidth, -metrics.TrackLength * 0.1f), new Vector2(metrics.TrackFullWidth, metrics.TrackLength * 1.1f), new Color(Color.White, 0.9f), 0, 0);
            }
            {
                _finishLineRectangle = new TexturedRectangle(game.GraphicsDevice);
                _finishLineRectangle.SetVerticesXY(new Vector2(-metrics.HalfTrackInnerWidth, metrics.FinishLineY - metrics.FinishLineHeight / 2), new Vector2(metrics.TrackInnerWidth, metrics.FinishLineHeight), new Color(Color.MediumPurple, 0.9f), 0.02f);
            }
            {
                _laneDividerRectangles = new TexturedRectangle[3];
                for (var i = 1; i < 4; ++i) {
                    _laneDividerRectangles[i - 1] = new TexturedRectangle(game.GraphicsDevice);
                    var left = i * metrics.TrackInnerWidth / 4 - metrics.LaneDividerWidth / 2 - metrics.HalfTrackInnerWidth;
                    var origin = new Vector2(left, 0);
                    var size = new Vector2(metrics.LaneDividerWidth, metrics.TrackLength);
                    _laneDividerRectangles[i - 1].SetVerticesXY(origin, size, new Color(Color.Lavender, 0.9f), 0.01f);
                }
            }
            {
                _skyInputRectangle = new TexturedRectangle(game.GraphicsDevice);
                _skyInputRectangle.SetVerticesXZ(new Vector2(-metrics.SkyInputWidth / 2, metrics.SkyInputZ - metrics.SkyInputTallness / 2), new Vector2(metrics.SkyInputWidth, metrics.SkyInputTallness), Color.White, metrics.FinishLineY);
            }
        }

        protected override void OnUnloadContents() {
            _beatmap?.Dispose();

            _skyInputTexture?.Dispose();
            _panelTexture?.Dispose();
            _trackLaneDividerTexture?.Dispose();

            _noteTexture?.Dispose();
            _noteHoldTexture?.Dispose();
            _noteSkyTexture?.Dispose();
            _noteArcTexture?.Dispose();

            _trackRectangle?.Dispose();
            if (_laneDividerRectangles != null && _laneDividerRectangles.Length > 0) {
                foreach (var laneRect in _laneDividerRectangles) {
                    laneRect?.Dispose();
                }
            }
            _finishLineRectangle?.Dispose();

            _skyInputRectangle?.Dispose();

            _basicEffect?.Dispose();

            base.OnUnloadContents();
        }

        protected override void OnDrawBuffer(GameTime gameTime) {
            base.OnDrawBuffer(gameTime);

            var game = Game.ToBaseGame();
            var camera = game.FindSingleElement<Cameraman>()?.Camera;

            if (camera == null) {
                return;
            }

            // First, set the effect parameters for the effects. 

            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = camera.ViewMatrix;
            _basicEffect.Projection = camera.ProjectionMatrix;

            GetTrackParams(out var beatmapTicks, out var currentY);

            var graphicsDevice = game.GraphicsDevice;

            // Then draw screen elements.

            DrawTrack(graphicsDevice, beatmapTicks, currentY);
            DrawLaneDividers(graphicsDevice);
            DrawFinishLine(graphicsDevice);
            DrawNotes(graphicsDevice, beatmapTicks, currentY);
            DrawSkyInput(graphicsDevice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTrack([NotNull] GraphicsDevice graphicsDevice, int beatmapTicks, float currentY) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Alpha = 1.0f;
            _basicEffect.Texture = _panelTexture;

            var metrics = _stageMetrics;

            const float offsetZoomScale = 1.0f;

            _trackRectangle.SetVerticesXY(new Vector2(-metrics.HalfTrackFullWidth, -metrics.TrackLength * 0.1f), new Vector2(metrics.TrackFullWidth, metrics.TrackLength * 1.1f), Color.White, currentY * offsetZoomScale, 0);

            _trackRectangle.Draw(_basicEffect.CurrentTechnique);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawFinishLine([NotNull] GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = false;
            _basicEffect.Alpha = 0.8f;
            _basicEffect.Texture = _skyInputTexture;

            _finishLineRectangle.Draw(_basicEffect.CurrentTechnique);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawLaneDividers([NotNull] GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Alpha = 1f;
            _basicEffect.Texture = _trackLaneDividerTexture;

            for (var i = 0; i < _laneDividerRectangles.Length; ++i) {
                _laneDividerRectangles[i].Draw(_basicEffect.CurrentTechnique);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawSkyInput([NotNull] GraphicsDevice graphicsDevice) {
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Alpha = 1f;
            _basicEffect.Texture = _skyInputTexture;

            _skyInputRectangle.Draw(_basicEffect.CurrentTechnique);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawNotes([NotNull] GraphicsDevice graphicsDevice, int beatmapTicks, float currentY) {
            var beatmap = _beatmap;

            if (beatmap == null) {
                return;
            }

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            foreach (var note in beatmap.VisualNotes) {
                if (note.IsVisible(beatmapTicks, currentY)) {
                    switch (note.Type) {
                        case NoteType.Floor: {
                                var n = (FloorVisualNote)note;
                                n.SetTexture(_noteTexture);
                                n.Draw(beatmapTicks, currentY);
                            }
                            break;
                        case NoteType.Long: {
                                var n = (LongVisualNote)note;
                                n.SetTextures(_noteHoldTexture, _noteHoldHighlightedTexture);
                                n.Draw(beatmapTicks, currentY);
                            }
                            break;
                        case NoteType.Arc: {
                                var n = (ArcVisualNote)note;
                                n.SetSkyNoteTexture(_noteSkyTexture);
                                n.SetSupportTexture(_skyInputTexture);
                                n.SetArcTexture(_noteArcTexture);
                                n.Draw(beatmapTicks, currentY);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void GetTrackParams(out int beatmapTicks, out float currentY) {
            beatmapTicks = 0;
            currentY = 0;

            var beatmap = _beatmap;

            if (beatmap == null) {
                return;
            }

            if (_syncTimer == null) {
                _syncTimer = Game.FindSingleElement<SyncTimer>();
            }

            var syncTimer = _syncTimer;

            if (syncTimer == null) {
                return;
            }

            var audioTicks = (int)syncTimer.CurrentTime.TotalMilliseconds;
            var audioOffset = beatmap.BaseBeatmap.AudioOffset;
            beatmapTicks = audioTicks - audioOffset;
            currentY = beatmap.CalculateY(beatmapTicks, _stageMetrics, 0);
        }

        private static readonly DepthStencilState DepthEnabled = new DepthStencilState {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
            DepthBufferFunction = CompareFunction.LessEqual
        };

        [CanBeNull]
        private SyncTimer _syncTimer;

        [CanBeNull]
        private VisualBeatmap _beatmap;

        private Texture2D _panelTexture;
        private Texture2D _trackLaneDividerTexture;
        private Texture2D _skyInputTexture;

        private Texture2D _noteTexture;
        private Texture2D _noteHoldTexture;
        private Texture2D _noteHoldHighlightedTexture;
        private Texture2D _noteSkyTexture;
        private Texture2D _noteArcTexture;

        private TexturedRectangle _trackRectangle;
        private TexturedRectangle _finishLineRectangle;
        private TexturedRectangle[] _laneDividerRectangles;
        private TexturedRectangle _skyInputRectangle;

        private BasicEffect _basicEffect;

        private StageMetrics _stageMetrics;

    }
}



