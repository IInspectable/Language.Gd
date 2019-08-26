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

}