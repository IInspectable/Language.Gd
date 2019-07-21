using System.Linq;

namespace Pharmatechnik.Language.Gd.Antlr {

    partial class GdGrammar {

        void CheckMatchingContainerIdentifier(string identifier1, string identifier2, params int[] containerTokens) {

            if (string.IsNullOrEmpty(identifier2) || identifier1 == identifier2) {
                return;
            }

            var containerName = string.Join(" ", containerTokens.Select(token => Vocabulary.GetSymbolicName(token)));
            var endKeyword    = Vocabulary.GetSymbolicName(End);

            // TODO Das muss evtl. irgendwie zum Sematic Error werden, oder per Id unterscheidbar.
            NotifyErrorListeners($"{containerName} and {endKeyword} {containerName} identifiers do not match: {identifier1} <-> {identifier2}");
        }

    }

}