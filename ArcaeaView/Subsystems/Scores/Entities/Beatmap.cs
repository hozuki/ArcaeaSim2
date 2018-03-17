using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <summary>
    /// Represents an Arcaea beatmap.
    /// </summary>
    public sealed class Beatmap {

        /// <summary>
        /// Audio offset, in milliseconds.
        /// </summary>
        public int AudioOffset { get; set; }

        /// <summary>
        /// Notes in the beatmap.
        /// </summary>
        [NotNull]
        public NoteBase[] Notes { get; set; } = new NoteBase[0];

        public static bool TryParse([NotNull] StreamReader reader, [CanBeNull] out Beatmap beatmap) {
            try {
                beatmap = Parse(reader);
                return true;
            } catch (Exception) {
                beatmap = null;
                return false;
            }
        }

        public static bool TryParse([NotNull] string text, [CanBeNull] out Beatmap beatmap) {
            try {
                beatmap = Parse(text);
                return true;
            } catch (Exception) {
                beatmap = null;
                return false;
            }
        }

        [NotNull]
        public static Beatmap Parse([NotNull] string text) {
            var bytes = Encoding.UTF8.GetBytes(text);

            using (var memoryStream = new MemoryStream(bytes, false)) {
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8)) {
                    return Parse(reader);
                }
            }
        }

        [NotNull]
        public static Beatmap Parse([NotNull] StreamReader reader) {
            var line = reader.ReadLine();
            var lineCounter = 1;

            Debug.Assert(line != null, nameof(line) + " != null");
            Debug.Assert(line.StartsWith("AudioOffset:"));

            var audioOffset = int.Parse(line.Substring("AudioOffset:".Length));

            reader.ReadLine(); // "-"
            ++lineCounter;

            var notes = new List<NoteBase>();

            while (!reader.EndOfStream) {
                line = reader.ReadLine();
                ++lineCounter;

                if (string.IsNullOrWhiteSpace(line)) {
                    continue;
                }

                if (line.StartsWith("hold")) {
                    var match = HoldRegex.Match(line);

                    if (!match.Success) {
                        throw new FormatException($"Hold format error at line {lineCounter}.");
                    }

                    var content = match.Groups["content"].Value;
                    var segs = content.Split(ContentSeparator);

                    var note = new LongNote();
                    note.StartTick = Convert.ToInt32(segs[0]);
                    note.EndTick = Convert.ToInt32(segs[1]);
                    note.Track = (Track)Convert.ToInt32(segs[2]);

                    notes.Add(note);
                } else if (line.StartsWith("arc")) {
                    var match = ArcRegex.Match(line);

                    if (!match.Success) {
                        throw new FormatException($"Arc format error at line {lineCounter}.");
                    }

                    var extra = match.Groups["extra"].Value;
                    SkyNote[] arcTapNotes;

                    if (!string.IsNullOrWhiteSpace(extra)) {
                        var subNoteStrings = extra.Split(ContentSeparator);
                        var arcTapNoteList = new List<SkyNote>();

                        foreach (var subNoteString in subNoteStrings) {
                            var subMatch = ArcTapRegex.Match(subNoteString);

                            if (!subMatch.Success) {
                                throw new FormatException($"ArcTap format error at line {lineCounter}.");
                            }

                            var arcTapNote = new SkyNote();
                            arcTapNote.Tick = Convert.ToInt32(subMatch.Groups["content"].Value);

                            arcTapNoteList.Add(arcTapNote);
                        }

                        arcTapNotes = arcTapNoteList.ToArray();
                    } else {
                        arcTapNotes = null;
                    }

                    var content = match.Groups["content"].Value;
                    var segs = content.Split(ContentSeparator);

                    var note = new ArcNote();
                    note.StartTick = Convert.ToInt32(segs[0]);
                    note.EndTick = Convert.ToInt32(segs[1]);
                    note.StartX = Convert.ToSingle(segs[2]);
                    note.EndX = Convert.ToSingle(segs[3]);
                    note.Easing = ParseEasing(segs[4]);
                    note.StartY = Convert.ToSingle(segs[5]);
                    note.EndY = Convert.ToSingle(segs[6]);
                    note.Color = (ArcColor)Convert.ToInt32(segs[7]);
                    note.Unknown1 = segs[8];
                    note.IsGuiding = Convert.ToBoolean(segs[9]);
                    note.SkyNotes = arcTapNotes;

                    notes.Add(note);
                } else if (line.StartsWith("timing")) {
                    var match = TimingRegex.Match(line);

                    if (!match.Success) {
                        throw new FormatException($"Timing format error at line {lineCounter}.");
                    }

                    var content = match.Groups["content"].Value;
                    var segs = content.Split(ContentSeparator);

                    var note = new TimingNote();
                    note.Tick = Convert.ToInt32(segs[0]);
                    note.Bpm = Convert.ToSingle(segs[1]);
                    note.BeatsPerMeasure = Convert.ToSingle(segs[2]);

                    notes.Add(note);
                } else {
                    var match = TapRegex.Match(line);

                    if (!match.Success) {
                        throw new FormatException($"Tap format error at line {lineCounter}.");
                    }

                    var content = match.Groups["content"].Value;
                    var segs = content.Split(ContentSeparator);

                    var note = new FloorNote();
                    note.Tick = Convert.ToInt32(segs[0]);
                    note.Track = (Track)Convert.ToInt32(segs[1]);

                    notes.Add(note);
                }
            }

            notes.Sort(NoteBase.Compare);

            var beatmap = new Beatmap();

            beatmap.AudioOffset = audioOffset;
            beatmap.Notes = notes.ToArray();

            return beatmap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ArcEasing ParseEasing([NotNull] string str) {
            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }

            var s = str.Trim().ToLowerInvariant();

            switch (s) {
                case "s":
                    return ArcEasing.S;
                case "b":
                    return ArcEasing.CubicBezier;
                case "si":
                    return ArcEasing.Si;
                case "so":
                    return ArcEasing.So;
                case "siso":
                    return ArcEasing.SiSo;
                case "sosi":
                    return ArcEasing.SoSi;
                case "sisi":
                    return ArcEasing.SiSi;
                case "soso":
                    return ArcEasing.SoSo;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, $"Unknown easing type \"{str}\".");
            }
        }

        private static readonly Regex TapRegex = new Regex(@"^\((?<content>[^)]+)\);$");
        private static readonly Regex HoldRegex = new Regex(@"^hold\((?<content>[^)]+)\);$");
        private static readonly Regex ArcRegex = new Regex(@"^arc\((?<content>[^)]+)\)(\[(?<extra>[^\]]*)\])?;$");
        private static readonly Regex ArcTapRegex = new Regex(@"^arctap\((?<content>\d+)\)$");
        private static readonly Regex TimingRegex = new Regex(@"^timing\((?<content>[^)]+)\);$");

        private static readonly char[] ContentSeparator = { ',' };

    }
}
