namespace Pharmatechnik.Language.Gd {

    partial class ContainerSyntax: IContainerSyntax {

        ContainerDeclarationSyntax IContainerSyntax.ContainerDeclaration => ContainerDeclarationOverride;

        ISectionSyntax IContainerSyntax. Section => (ISectionSyntax) this;
        ContainerSyntax IContainerSyntax.Syntax  => this;

        private protected abstract ContainerDeclarationSyntax ContainerDeclarationOverride { get; }

    }

    partial class DialogSectionSyntax {

        private protected override ContainerDeclarationSyntax ContainerDeclarationOverride => ContainerDeclaration;

    }

    partial class FormSectionSyntax {

        private protected override ContainerDeclarationSyntax ContainerDeclarationOverride => ContainerDeclaration;

    }

    partial class UserControlSectionSyntax {

        private protected override ContainerDeclarationSyntax ContainerDeclarationOverride => ContainerDeclaration;

    }

}