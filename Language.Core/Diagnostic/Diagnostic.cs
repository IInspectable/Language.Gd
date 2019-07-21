#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language {

    [Serializable]
    public sealed class Diagnostic: IEquatable<Diagnostic> {

        [NotNull] readonly object[] _messageArgs;

        public Diagnostic(Location location, DiagnosticDescriptor descriptor, params object[] messageArgs) {
            Location            = location   ?? throw new ArgumentNullException(nameof(location));
            Descriptor          = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            AdditionalLocations = EmptyAdditionalLocations;
            _messageArgs        = messageArgs ?? EmptyMessageArgs;
        }

        public Diagnostic(Location location, Location additionalLocation, DiagnosticDescriptor descriptor, params object[] messageArgs)
            : this(location, new[] {additionalLocation}, descriptor, messageArgs) {
        }

        public Diagnostic(Location location, IEnumerable<Location> additionalLocations, DiagnosticDescriptor descriptor, params object[] messageArgs) {
            Location            = location                                                          ?? throw new ArgumentNullException(nameof(location));
            Descriptor          = descriptor                                                        ?? throw new ArgumentNullException(nameof(descriptor));
            AdditionalLocations = additionalLocations?.Where(loc => loc != null).ToImmutableArray() ?? EmptyAdditionalLocations;
            _messageArgs        = messageArgs                                                       ?? EmptyMessageArgs;
        }

        public Diagnostic WithLocation(Location location) {
            return new Diagnostic(location, Descriptor, _messageArgs);
        }

        static readonly object[]                EmptyMessageArgs         = { };
        static readonly IReadOnlyList<Location> EmptyAdditionalLocations = Enumerable.Empty<Location>().ToImmutableList();

        [NotNull]
        public Location Location { get; }

        [NotNull]
        public IReadOnlyList<Location> AdditionalLocations { get; }

        public IEnumerable<Location> GetLocations() {
            yield return Location;

            foreach (var location in AdditionalLocations) {
                yield return location;
            }
        }

        public IEnumerable<Diagnostic> ExpandLocations() {
            return GetLocations().Select(WithLocation);
        }

        public DiagnosticDescriptor Descriptor { get; }
        public DiagnosticSeverity   Severity   => Descriptor.DefaultSeverity;
        public DiagnosticCategory   Category   => Descriptor.Category;
        public String               Message    => FormatMessage();

        public override string ToString() {
            return ToString(null);
        }

        public string ToString(DiagnosticFormatter formatter) {
            formatter = formatter ?? DiagnosticFormatter.Instance;
            return formatter.Format(this);
        }

        #region Equality members

        public bool Equals(Diagnostic other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Location.Equals(other.Location) && Equals(Descriptor, other.Descriptor);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj is Diagnostic diagnostic && Equals(diagnostic);
        }

        public override int GetHashCode() {
            unchecked {
                return (Location.GetHashCode() * 397) ^ (Descriptor != null ? Descriptor.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Diagnostic left, Diagnostic right) {
            return Equals(left, right);
        }

        public static bool operator !=(Diagnostic left, Diagnostic right) {
            return !Equals(left, right);
        }

        #endregion

        string FormatMessage() {
            if (_messageArgs.Length != 0) {
                return String.Format(Descriptor.MessageFormat, _messageArgs);
            } else {
                return Descriptor.MessageFormat;
            }
        }

    }

}