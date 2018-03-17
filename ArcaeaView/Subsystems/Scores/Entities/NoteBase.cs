using System;
using JetBrains.Annotations;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Scores.Entities {
    /// <summary>
    /// Arcaea note base.
    /// </summary>
    public abstract class NoteBase {

        /// <summary>
        /// Type of the note.
        /// </summary>
        public abstract NoteType Type { get; }

        /// <summary>
        /// Common note comparison function. It compares the notes' tick (or start tick).
        /// </summary>
        /// <param name="note1">The first note to be compared.</param>
        /// <param name="note2">The second note to be compared.</param>
        /// <returns>Comparison result. 1, 0, -1 mean greater than, equal, and less than respectively.</returns>
        public static int Compare([CanBeNull] NoteBase note1, [CanBeNull] NoteBase note2) {
            if (note1 == null) {
                if (note2 == null) {
                    return 0;
                } else {
                    return -1;
                }
            } else {
                if (note2 == null) {
                    return 1;
                } else {
                    int tick1, tick2;

                    if (note1 is IHasTick t1) {
                        tick1 = t1.Tick;
                    } else if (note1 is IHasTicks ts1) {
                        tick1 = ts1.StartTick;
                    } else {
                        throw new NotSupportedException();
                    }

                    if (note2 is IHasTick t2) {
                        tick2 = t2.Tick;
                    } else if (note2 is IHasTicks ts2) {
                        tick2 = ts2.StartTick;
                    } else {
                        throw new NotSupportedException();
                    }

                    return tick1.CompareTo(tick2);
                }
            }
        }

    }
}
