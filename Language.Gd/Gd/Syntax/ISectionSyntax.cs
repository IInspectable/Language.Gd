using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd {

    public interface ISectionSyntax {

        [CanBeNull]
        SyntaxNode SectionBegin { get; }

        [CanBeNull]
        SyntaxNode SectionEnd { get; }

    }

    public interface INamedSectionSyntax: ISectionSyntax {

        SyntaxToken? NameBegin { get; }
        SyntaxToken? NameEnd   { get; }

    }

    // TODO Nur zum Testen...

    partial class ControlSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => ControlSectionBegin?.ControlNameToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => ControlSectionEnd?.ControlNameToken;

    }

    partial class FormSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => FormSectionBegin?.FormToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => FormSectionEnd?.FormToken;

    }

}