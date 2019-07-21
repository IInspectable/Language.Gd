namespace Pharmatechnik.Language.Text {

    public sealed class ClassifiedText {

        public ClassifiedText(string text, string classification) {
            Text           = text ?? "";
            Classification = classification;

        }

        public string Text           { get; }
        public string Classification { get; }

        public override string ToString() => Text;

    }

}