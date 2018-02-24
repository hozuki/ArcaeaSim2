using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities;
using OpenMLTD.MilliSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Visualization {
    /// <inheritdoc />
    /// <summary>
    /// A visual beatmap.
    /// </summary>
    public sealed class VisualBeatmap : DisposableBase {

        /// <summary>
        /// Creates a new <see cref="VisualBeatmap"/> instance from a <see cref="Beatmap"/> using specified stage metrics.
        /// </summary>
        /// <param name="graphicsDevice">A <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> used to draw the beatmap's notes.</param>
        /// <param name="baseBeatmap"></param>
        /// <param name="metrics">Stage metrics.</param>
        public VisualBeatmap([NotNull] GraphicsDevice graphicsDevice, [NotNull] Beatmap baseBeatmap, [NotNull] StageMetrics metrics) {
            GraphicsDevice = graphicsDevice;
            BaseBeatmap = baseBeatmap;

            Timings = baseBeatmap.Notes.Where(n => n.Type == NoteType.Timing).Cast<TimingNote>().ToArray();

            var visualNotes = baseBeatmap.Notes.Where(n => n.Type != NoteType.Timing).Select<NoteBase, VisualNoteBase>(n => {
                switch (n.Type) {
                    case NoteType.Floor:
                        return new FloorVisualNote(this, (FloorNote)n, metrics);
                    case NoteType.Long:
                        return new LongVisualNote(this, (LongNote)n, metrics);
                    case NoteType.Arc:
                        return new ArcVisualNote(this, (ArcNote)n, metrics);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).ToArray();

            VisualNotes = visualNotes;

            // Scan sync sky notes and add lines between the floor note and sky note.
            // Credits: @money, @RW, @JDF, @L-F0rce

            var allArcVisualNotes = visualNotes.Where(note => note.Type == NoteType.Arc).Cast<ArcVisualNote>().ToArray();

            for (var i = 0; i < visualNotes.Length; ++i) {
                var visualNote = visualNotes[i];

                if (visualNote.Type != NoteType.Floor || i >= visualNotes.Length - 1) {
                    continue;
                }

                var n = (FloorVisualNote)visualNote;
                var floorBase = (FloorNote)n.BaseNote;

                // TODO: Naive method. Should be optimized.
                foreach (var arcVisualNote in allArcVisualNotes) {
                    if (arcVisualNote.SkyVisualNotes == null || arcVisualNote.SkyVisualNotes.Length == 0) {
                        continue;
                    }

                    foreach (var skyVisualNote in arcVisualNote.SkyVisualNotes) {
                        if (((SkyNote)skyVisualNote.BaseNote).Tick == floorBase.Tick) {
                            n.SynchronizedSkyVisualNote = skyVisualNote;
                            break;
                        }
                    }

                    if (n.SynchronizedSkyVisualNote != null) {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="Beatmap"/>.
        /// </summary>
        [NotNull]
        public Beatmap BaseBeatmap { get; }

        /// <summary>
        /// Gets the <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> used to draw this beatmap's notes.
        /// </summary>
        [NotNull]
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Gets all visual notes in the beatmap.
        /// </summary>
        [NotNull, ItemNotNull]
        public VisualNoteBase[] VisualNotes { get; }

        /// <summary>
        /// Gets all timing notes in the beatmap.
        /// </summary>
        [NotNull, ItemNotNull]
        public TimingNote[] Timings { get; }

        /// <summary>
        /// Gets the first tempo value of this beatmap.
        /// </summary>
        public float FirstBpm {
            get {
                if (_firstBpm == null) {
                    if (Timings.Length == 0) {
                        throw new FormatException("No timing note found in this beatmap.");
                    }

                    _firstBpm = Timings[0].Bpm;
                }

                return _firstBpm.Value;
            }
        }

        /// <summary>
        /// Calculate a beatmap time's Y distance according to timing and metrics.
        /// </summary>
        /// <param name="beatmapTicks">Current beatmap time, in milliseconds. Please note that beatmap time does not equal to audio time when a beatmap's audio offset is non-zero.</param>
        /// <param name="metrics">Stage metrics.</param>
        /// <param name="addition">The final addition value. This value is directly added to the calculated value. Example usage: assign this parameter the finishing line's Y distance.</param>
        /// <returns></returns>
        public float CalculateY(int beatmapTicks, [NotNull] StageMetrics metrics, float addition) {
            const float timeScale = 0.6f;

            var timingNotes = Timings;

            if (timingNotes.Length == 0) {
                return 0;
            }

            var measureLength = metrics.FloorNoteHeight * 6;

            var result = 0f;

            for (var i = 0; i < timingNotes.Length; ++i) {
                var timingNote = timingNotes[i];

                if (beatmapTicks < timingNote.Tick) {
                    break;
                }

                if (i >= timingNotes.Length - 1 || beatmapTicks < timingNotes[i + 1].Tick) {
                    result += (beatmapTicks - timingNote.Tick) * timingNote.Bpm;
                } else {
                    result += (timingNotes[i + 1].Tick - timingNote.Tick) * timingNote.Bpm;
                }
            }

            result /= 100_000f;

            result *= measureLength * timeScale * metrics.Speed;

            result += addition;

            return result;
        }

        protected override void Dispose(bool disposing) {
            if (VisualNotes.Length > 0) {
                foreach (var note in VisualNotes) {
                    note.Dispose();
                }
            }
        }

        private float? _firstBpm;

    }
}
