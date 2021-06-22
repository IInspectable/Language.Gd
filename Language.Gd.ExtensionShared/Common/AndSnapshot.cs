#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {
    abstract class AndSnapshot {

        protected AndSnapshot([NotNull] ITextSnapshot snapshot) {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        [NotNull]
        public ITextSnapshot Snapshot { get; }

        public bool IsCurrent(SnapshotSpan snapshotSpan) {
            return Snapshot.Version.VersionNumber == snapshotSpan.Snapshot.Version.VersionNumber;
        }

        public bool IsCurrent(ITextSnapshot snapshot) {
            if (snapshot != Snapshot) {
                return false;
            }
            return Snapshot.Version.ReiteratedVersionNumber == snapshot.Version.ReiteratedVersionNumber;
        }

        public bool IsCurrent(ITextBuffer textBuffer) {
            return IsCurrent(textBuffer.CurrentSnapshot);
        }
    }
}
