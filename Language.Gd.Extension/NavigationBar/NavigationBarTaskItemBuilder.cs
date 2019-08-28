#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.NavigationBar {

    class NavigationBarTaskItemBuilder: SyntaxVisitor {

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

            // TODO Fill NavigationBar Items
            foreach (var symbol in syntaxRoot.DescendantNodesAndSelf().Where(s => s is ISectionSyntax)) {
                builder.Visit(symbol);
            }

            var items = builder.NavigationItems
                               .OrderBy(ni => ni.Start)
                               .ToImmutableList();

            return items;
        }

        #if ShowMemberCombobox
        public override void VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {
            MemberItems.Add(new NavigationBarItem(signalTriggerSymbol.Name, NavigationBarImages.Index.TriggerSymbol, signalTriggerSymbol.Transition.Location, signalTriggerSymbol.Start));
        }
        #endif

    }

}