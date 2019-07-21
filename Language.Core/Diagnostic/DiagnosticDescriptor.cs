#region Using Directives

using System;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language {

    public sealed class DiagnosticDescriptor : IEquatable<DiagnosticDescriptor> {
        
        public DiagnosticDescriptor(string id, string messageFormat, DiagnosticCategory category, DiagnosticSeverity defaultSeverity) {

            if (String.IsNullOrWhiteSpace(id)) {
                throw new ArgumentException("Diagnostic id can't be null or whitespace", nameof(id));
            }

            Id              = id;
            MessageFormat   = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));
            Category        = category;
            DefaultSeverity = defaultSeverity;
        }

        [NotNull]
        public string Id { get; }

        [NotNull]
        public string MessageFormat { get; }

        public DiagnosticCategory Category { get; }

        public DiagnosticSeverity DefaultSeverity { get; }

        public bool Equals(DiagnosticDescriptor other) {

            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return String.Equals(Id, other.Id) &&
                   String.Equals(MessageFormat, other.MessageFormat) &&
                   String.Equals(Category, other.Category) &&
                   DefaultSeverity == other.DefaultSeverity;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj is DiagnosticDescriptor descriptor && Equals(descriptor);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ MessageFormat.GetHashCode();
                hashCode = (hashCode * 397) ^ Category.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)DefaultSeverity;
                return hashCode;
            }
        }

        public static bool operator ==(DiagnosticDescriptor left, DiagnosticDescriptor right) {
            return Equals(left, right);
        }

        public static bool operator !=(DiagnosticDescriptor left, DiagnosticDescriptor right) {
            return !Equals(left, right);
        }

        public override string ToString() {
            return $"{Category} {DefaultSeverity} {Id} : {MessageFormat}";
        }
    }
}