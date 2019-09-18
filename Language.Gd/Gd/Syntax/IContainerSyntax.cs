namespace Pharmatechnik.Language.Gd {

    public interface IContainerSyntax {

        ContainerDeclarationSyntax ContainerDeclaration { get; }
        ISectionSyntax             Section              { get; }
        ContainerSyntax            Syntax               { get; }

    }

}