﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

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
            // TODO Error Handling wenn doch null?
            return new RequiredSlot(slotVisit(context));
        }

        public static IEnumerable<RequiredSlot> OneOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {
            // TODO Error Handling wenn nicht mindestens 1 Element?
            return ZeroOrMore(contexts, slotVisit);

        }

        public static IEnumerable<RequiredSlot> ZeroOrMore<TContext>(
            this IEnumerable<TContext> contexts,
            Func<TContext, SyntaxSlot> slotVisit)
            where TContext : ParserRuleContext {
            // TODO Error Handling wenn doch null?
            return contexts.Select(slotVisit)
                           .Select(slot => new RequiredSlot(slot));

        }

        public static TSlot OfType<TSlot>(this OptionalSlot optional)
            where TSlot : SyntaxSlot {

            return optional.Slot as TSlot;
        }

        [NotNull]
        public static TSlot OfType<TSlot>(this RequiredSlot required)
            where TSlot : SyntaxSlot {

            return (TSlot) required.Slot;
        }

        [NotNull]
        public static SyntaxListSlot<TSlot> OfType<TSlot>(this IEnumerable<RequiredSlot> requiredSlots)
            where TSlot : SyntaxSlot {

            // TODO extent
            return new SyntaxListSlot<TSlot>(
                TextExtent.Empty, requiredSlots.Select(rs => rs.Slot)
                                               .Cast<TSlot>()
                                               .ToImmutableArray());
        }

        internal struct OptionalSlot {

            public OptionalSlot([CanBeNull] SyntaxSlot slot) {
                Slot = slot;
            }

            [CanBeNull]
            public SyntaxSlot Slot { get; }

        }

        internal struct RequiredSlot {

            public RequiredSlot([NotNull] SyntaxSlot slot) {
                Slot = slot;
            }

            [NotNull]
            public SyntaxSlot Slot { get; }

        }

    }

}