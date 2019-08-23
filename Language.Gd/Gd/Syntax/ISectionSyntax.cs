namespace Pharmatechnik.Language.Gd {

    public interface ISectionSyntax {

        SyntaxNode Begin { get; }
        SyntaxNode End   { get; }

    }

    public interface INamedSectionSyntax: ISectionSyntax {

        SyntaxToken? NameBegin { get; }
        SyntaxToken? NameEnd   { get; }

    }

    // TODO Nur zum Testen...
    partial class PropertiesSectionSyntax: ISectionSyntax {

        SyntaxNode ISectionSyntax.Begin => PropertiesSectionBegin;
        SyntaxNode ISectionSyntax.End   => PropertiesSectionEnd;

    }

    partial class ControlSectionSyntax: INamedSectionSyntax {

        SyntaxNode ISectionSyntax.Begin => ControlSectionBegin;
        SyntaxNode ISectionSyntax.End   => ControlSectionEnd;

        SyntaxToken? INamedSectionSyntax.NameBegin => ControlSectionBegin?.ControlNameToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => ControlSectionEnd?.ControlNameToken;

    }

}