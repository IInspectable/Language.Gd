#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;

#endregion

namespace Pharmatechnik.Language.Gd.Antlr {

    static class SyntaxSlotBuilderExtensions {

        public static SyntaxSlot Optional<TContext, TSlot>(
            this TContext context,
            Func<TContext, TSlot> slotVisit)
            where TSlot : SyntaxSlot
            where TContext : ParserRuleContext {

            return context == null ? null : slotVisit(context);
        }

        public static SyntaxSlot Required<TContext>(
            this TContext context,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            return context == null ? null : slotVisit(context);
        }

        public static IEnumerable<SyntaxSlot> OneOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            // Kann immer auch kein Element haben, wenn Syntax/Semantik Fehler im Code
            return ZeroOrMore(contexts, slotVisit);
        }

        public static IEnumerable<SyntaxSlot> ZeroOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            return contexts.Select(slotVisit);

        }

        // Nur nicht null, wenn keine Syntaxfehler...
        [CanBeNull]
        public static TSlot OfType<TSlot>(this SyntaxSlot slot)
            where TSlot : SyntaxSlot {

            return slot as TSlot;
        }

        [NotNull]
        public static SyntaxSlotList<TSlot> OfType<TSlot>(this IEnumerable<SyntaxSlot> requiredSlots)
            where TSlot : SyntaxSlot {

            return new SyntaxSlotList<TSlot>(
                requiredSlots.Where(slot => slot != null)
                             .Cast<TSlot>()
                             .ToImmutableArray());
        }

    }

}