using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd.Antlr {

    static class SyntaxSlotBuilderExtensions {

        public static OptionalSlot Optional<TContext, TSlot>(
            this TContext context,
            Func<TContext, TSlot> slotVisit)
            where TSlot : SyntaxSlot
            where TContext : ParserRuleContext {

            return context == null ? new OptionalSlot(null) : new OptionalSlot(slotVisit(context));
        }

        public static RequiredSlot Required<TContext>(
            this TContext context,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            return context == null ? new RequiredSlot(null) : new RequiredSlot(slotVisit(context));
        }

        public static IEnumerable<RequiredSlot> OneOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            // Kann immer auch kein Element haben, wenn Syntax/Semantik Fehler im Code
            return ZeroOrMore(contexts, slotVisit);
        }

        public static IEnumerable<RequiredSlot> ZeroOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {

            return contexts.Select(slotVisit)
                           .Select(slot => new RequiredSlot(slot));

        }

        [CanBeNull]
        public static TSlot OfType<TSlot>(this OptionalSlot optional)
            where TSlot : SyntaxSlot {

            return optional.Slot as TSlot;
        }

        // Nur nicht null, wenn keine Syntaxfehler...
        [CanBeNull]
        public static TSlot OfType<TSlot>(this RequiredSlot required)
            where TSlot : SyntaxSlot {

            return required.Slot as TSlot;
        }

        [NotNull]
        public static SyntaxSlotList<TSlot> OfType<TSlot>(this IEnumerable<RequiredSlot> requiredSlots)
            where TSlot : SyntaxSlot {

            return new SyntaxSlotList<TSlot>(
                requiredSlots.Select(rs => rs.Slot)
                             .Where(slot => slot != null)
                             .Cast<TSlot>()
                             .ToImmutableArray());
        }

        // TODO OptionalSlot haben eigentlich keinen Sinn, da nur bei korrekter Syntax gültig
        internal struct OptionalSlot {

            public OptionalSlot([CanBeNull] SyntaxSlot slot) {
                Slot = slot;
            }

            [CanBeNull]
            public SyntaxSlot Slot { get; }

        }

        internal struct RequiredSlot {

            public RequiredSlot([CanBeNull] SyntaxSlot slot) {
                Slot = slot;
            }

            [CanBeNull]
            public SyntaxSlot Slot { get; }

        }

    }

}