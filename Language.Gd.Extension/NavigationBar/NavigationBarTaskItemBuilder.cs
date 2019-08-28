#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarTaskItemBuilder: SyntaxListener {

        protected NavigationBarTaskItemBuilder() {
            NavigationItems = new List<NavigationBarItem>();
            MemberItems     = new List<NavigationBarItem>();
        }

        public List<NavigationBarItem> NavigationItems { get; }
        public List<NavigationBarItem> MemberItems     { get; }

        public static ImmutableList<NavigationBarItem> Build(SyntaxTreeAndSnapshot codeGenerationUnitAndSnapshot) {

            var syntaxRoot = codeGenerationUnitAndSnapshot?.SyntaxTree.Root;
            if (syntaxRoot == null) {
                return ImmutableList<NavigationBarItem>.Empty;
            }

            var builder = new NavigationBarTaskItemBuilder();
            builder.Visit(syntaxRoot);
            // TODO Fill NavigationBar Items

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Start)
                               .ToImmutableList();

            return items;

        }

        protected override void VisitGuiElementSyntax(GuiElementSyntax guiElement) {
            var sectionSyntax = guiElement as ISectionSyntax;

            var sectionBegin = sectionSyntax?.SectionBegin;
            if (sectionBegin != null) {

                // TODO Eigene Icons für die Controls
                NavigationItems.Add(new NavigationBarItem(
                                        displayName: sectionBegin.GetText(),
                                        imageIndex: 1,
                                        extent: guiElement.FullExtent,
                                        navigationPoint: guiElement.ExtentStart));
            }

            DefaultVisit(guiElement);
        }

        #if ShowMemberCombobox
        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationBarItem(signalTriggerSymbol.Name, NavigationBarImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
        #endif

    }

}