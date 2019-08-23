using System;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public struct ClassifiedExtent: IEquatable<ClassifiedExtent> {

        public ClassifiedExtent(TextExtent extent, Classification classification) {
            Extent         = extent;
            Classification = classification;

        }

        public TextExtent     Extent         { get; }
        public Classification Classification { get; }

        public override string ToString()
        {
            return $"{Extent} {Classification}";
        }

        public bool Equals(ClassifiedExtent other) {
            return Extent.Equals(other.Extent) && Classification == other.Classification;
        }

        public override bool Equals(object obj) {
            return obj is ClassifiedExtent other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (Extent.GetHashCode() * 397) ^ Classification.GetHashCode();
            }
        }

        public static bool operator ==(ClassifiedExtent left, ClassifiedExtent right) {
            return left.Equals(right);
        }

        public static bool operator !=(ClassifiedExtent left, ClassifiedExtent right) {
            return !left.Equals(right);
        }

    }

}