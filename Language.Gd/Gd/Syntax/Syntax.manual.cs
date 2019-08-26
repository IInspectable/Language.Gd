namespace Pharmatechnik.Language.Gd {

    // TODO Evtl. werden die Namen zu Syntax ohne Section + Identifier, z.B. ControlSectionSyntax => ControlIdentifier
    // TODO Dadurch automatische generierung der INamedSectionSyntax Interfaces möglich
    partial class ControlSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.ControlIdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.ControlIdentifierToken;

    }

    partial class FormSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class DialogSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class UserControlSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class PanelSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class DetailsPanelSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class BarManagerSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class TabNavigationSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class TabPageSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

    partial class MultiViewSectionSyntax: INamedSectionSyntax {

        SyntaxToken? INamedSectionSyntax.NameBegin => SectionBegin?.IdentifierToken;
        SyntaxToken? INamedSectionSyntax.NameEnd   => SectionEnd?.IdentifierToken;

    }

}