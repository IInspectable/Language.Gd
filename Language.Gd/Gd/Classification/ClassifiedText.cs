using System;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public struct ClassifiedText: IEquatable<ClassifiedText> {

        readonly string _text;

        public ClassifiedText(string text, GdClassification classification) {
            _text          = text;
            Classification = classification;

        }

        public ClassifiedText WithClassification(GdClassification classification) => new ClassifiedText(Text, classification);

        [NotNull]
        public string Text => _text ?? String.Empty;

        public GdClassification Classification { get; }

        public override string ToString() => Text;

        public bool Equals(ClassifiedText other) {
            return Text == other.Text && Classification == other.Classification;
        }

        public override bool Equals(object obj) {
            return obj is ClassifiedText other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Text.GetHashCode()) * 397) ^ (int) Classification;
            }
        }

        public static bool operator ==(ClassifiedText left, ClassifiedText right) {
            return left.Equals(right);
        }

        public static bool operator !=(ClassifiedText left, ClassifiedText right) {
            return !left.Equals(right);
        }

    }

}