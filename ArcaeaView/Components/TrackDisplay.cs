using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            // Note: Effects registered from file by EffectManager do not need to be manually disposed.
            _vertexColorEffect = game.EffectManager.RegisterSingleton<VertexColorEffect>("Contents/res/fx/vertex_color.fx");
            _glassEffect = game.EffectManager.RegisterSingleton<GlassEffect>("Contents/res/fx/glass.fx");

            _basicEffect = new BasicEffect(game.GraphicsDevice);
            game.EffectManager.RegisterSingleton(_basicEffect);

            // Hack: register note effect list
            NoteEffects.Effects[(int)NoteType.Floor] = _vertexColorEffect;
            NoteEffects.Effects[(int)NoteType.Long] = _vertexColorEffect;
            NoteEffects.Effects[(int)NoteType.Arc] = _vertexColorEffect;
            NoteEffects.Effects[(int)NoteType.Sky] = _basicEffect;

            var beatmap = Game.FindSingleElement<BeatmapLoader>()?.Beatmap;

            if (beatmap != null) {
                _beatmap = new VisualBeatmap(game.GraphicsDevice, beatmap, _stageMetrics);
            }

            _panelTexture = ContentHelper.LoadTexture(game.GraphicsDevice, @"Contents/res/img/track_dark.png");
            _trackLaneDividerTexture = ContentHelper.LoadTexture(game.GraphicsDevice, @"Contents/res/img/track_lane_divider.png");
            _finishLineTexture = ContentHelper.LoadTexture(game.GraphicsDevice, @"Contents/res/img/air_input.png");

            _noteTexture = ContentHelper.LoadTexture(game.GraphicsDevice, @"Contents/res/img/note_dark.png");
            _noteHoldTexture = ContentHelper.LoadTexture(game.GraphicsDevice, @"Contents/res/img/note_hold_dark.png");

            var metrics = _stageMetrics;

            {
                _trackRectangle = new TexturedRectangle(game.GraphicsDevice);
                _trackRectangle.SetVertices(new Vector2(-metrics.HalfTrackFullWidth, -metrics.TrackLength * 0.1f), new Vector2(metrics.TrackFullWidth, metrics.TrackLength * 1.1f), new Color(Color.White, 0.9f));
            }
            {
                _finishLineRectangle = new TexturedRectangle(game.GraphicsDevice);
                _finishLineRectangle.SetVertices(new Vector2(-metrics.HalfTrackInnerWidth, metrics.FinishLineY - metrics.FinishLineHeight / 2), new Vector2(metrics.TrackInnerWidth, metrics.FinishLineHeight), new Color(Color.MediumPurple, 0.9f), normalizeTextureY: true, z: 0.02f);
            }
            {
                _laneDividerRectangles = new TexturedRectangle[3];
                for (var i = 1; i < 4; ++i) {
                    _laneDividerRectangles[i - 1] = new TexturedRectangle(game.GraphicsDevice);
                    var left = i * metrics.TrackInnerWidth / 4 - metrics.LaneDividerWidth / 2 - metrics.HalfTrackInnerWidth;
                    var origin = new Vector2(left, 0);
                    var size = new Vector2(metrics.LaneDividerWidth, metrics.TrackLength);
                    _laneDividerRectangles[i - 1].SetVertices(origin, size, new Color(Color.Lavender, 0.9f), normalizeTextureY: true, z: 0.01f);
                }
            }
            {
                _skyInputBox = new ColoredBox(game.GraphicsDevice);
                _skyInputBox.SetVertices(new Vector3(-metrics.SkyInputWidth / 2, metrics.FinishLineY - metrics.SkyInputHeight / 2, metrics.SkyInputZ - metrics.SkyInputTallness / 2), new Vector3(metrics.SkyInputWidth, metrics.SkyInputHeight, metrics.SkyInputTallness), Color.FromNonPremultiplied(255, 200, 200, 255));
            }
        }

        protected override void OnUnloadContents() {
            _beatmap?.Dispose();

            _finishLineTexture?.Dispose();
            _panelTexture?.Dispose();
            _trackLaneDividerTexture?.Dispose();

            _noteTexture?.Dispose();
            _noteHoldTexture?.Dispose();

            _trackRectangle?.Dispose();
            if (_laneDividerRectangles != null && _laneDividerRectangles.Length > 0) {
                foreach (var laneRect in _laneDividerRectangles) {
                    laneRect?.Dispose();
                }
            }
            _finishLineRectangle?.Dispose();

            _skyInputBox?.Dispose();

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

            var viewProjection = camera.ViewMatrix * camera.ProjectionMatrix;

            _vertexColorEffect.WorldViewProjection = viewProjection;

            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = camera.ViewMatrix;
            _basicEffect.Projection = camera.ProjectionMatrix;

            _glassEffect.CameraPosition = camera.Position;
            _glassEffect.EdgeColor = Color.White;
            _glassEffect.EdgeThickness = 0.01f;
            _glassEffect.World = Matrix.Identity;
            _glassEffect.WorldInverse = Matrix.Identity;
            _glassEffect.WorldViewProjection = viewProjection;

            var graphicsDevice = game.GraphicsDevice;

            // Then draw screen elements.

            DrawTrack(graphicsDevice);
            DrawLaneDividers(graphicsDevice);
            DrawFinishLine(graphicsDevice);
            DrawNotes(graphicsDevice);
            DrawSkyStop(graphicsDevice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTrack([NotNull] GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Alpha = 0.9f;
            _basicEffect.Texture = _panelTexture;

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
            _basicEffect.Texture = _finishLineTexture;

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
        private void DrawSkyStop([NotNull] GraphicsDevice graphicsDevice) {
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            _skyInputBox.Draw(_vertexColorEffect.CurrentTechnique);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawNotes([NotNull] GraphicsDevice graphicsDevice) {
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

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthEnabled;

            var audioTicks = (int)syncTimer.CurrentTime.TotalMilliseconds;
            var audioOffset = beatmap.BaseBeatmap.AudioOffset;
            var beatmapTicks = audioTicks - audioOffset;
            var currentY = beatmap.CalculateY(beatmapTicks, _stageMetrics, 0);

            foreach (var note in beatmap.VisualNotes) {
                if (note.IsVisible(beatmapTicks, currentY)) {
                    switch (note.Type) {
                        case NoteType.Floor:
                            note.Draw(beatmapTicks, currentY);
                            break;
                        case NoteType.Long:
                            note.Draw(beatmapTicks, currentY);
                            break;
                        case NoteType.Arc: {
                                ((ArcVisualNote)note).SetTexture1(_noteHoldTexture);
                                ((ArcVisualNote)note).SetTexture2(_noteTexture);
                                note.Draw(beatmapTicks, currentY);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
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
        private Texture2D _finishLineTexture;

        private Texture2D _noteTexture;
        private Texture2D _noteHoldTexture;

        private TexturedRectangle _trackRectangle;
        private TexturedRectangle _finishLineRectangle;
        private TexturedRectangle[] _laneDividerRectangles;
        private ColoredBox _skyInputBox;

        private VertexColorEffect _vertexColorEffect;
        private BasicEffect _basicEffect;
        private GlassEffect _glassEffect;

        private StageMetrics _stageMetrics;

    }
}



