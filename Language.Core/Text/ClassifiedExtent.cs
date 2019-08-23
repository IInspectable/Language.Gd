using System;

namespace Pharmatechnik.Language.Text {

    public struct ClassifiedExtent: IEquatable<ClassifiedExtent> {

        public ClassifiedExtent(TextExtent extent, string classificationName) {
            Extent             = extent;
            ClassificationName = classificationName;

        }

        public TextExtent Extent             { get; }
        public string     ClassificationName { get; }

        public bool Equals(ClassifiedExtent other) {
            return Extent.Equals(other.Extent) && ClassificationName == other.ClassificationName;
        }

        public override bool Equals(object obj) {
            return obj is ClassifiedExtent other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (Extent.GetHashCode() * 397) ^ (ClassificationName != null ? ClassificationName.GetHashCode() : 0);
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