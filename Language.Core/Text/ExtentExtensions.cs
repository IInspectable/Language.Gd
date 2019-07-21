#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Language.Text {

    static class ExtentExtensions {

        public static TElement NextOrPreviousElement<TExtent, TElement>(this IReadOnlyList<TElement> tokens, TExtent extent, TElement currentToken, bool nextToken, TElement missing)
            where TExtent : IExtent
            where TElement : IExtent {

            if (extent == null || currentToken == null) {
                return missing;
            }

            int index = FindIndexAtPosition(tokens, currentToken.Start);
            if (index < 0) {
                return missing; // eingenlich könnte man hier auch eine ArgumentException werfen...
            }

            index = nextToken ? index + 1 : index - 1;

            if (index < 0 || index >= tokens.Count) {
                return missing;
            }

            var resultToken = tokens[index];

            if (resultToken.Start < extent.Start || resultToken.End > extent.End) {
                return missing;
            }

            return resultToken;
        }

        public static IEnumerable<TElement> GetElements<TElement, TExtent>(this IReadOnlyList<TElement> tokens,
                                                                           TExtent extent, bool includeOverlapping)
            where TElement : IExtent
            where TExtent : IExtent {

            if (!includeOverlapping) {
                return GetElementsInside(tokens, extent);
            }

            return GetElementsIncludeOverlapping(tokens, extent);
        }

        static IEnumerable<TElement> GetElementsIncludeOverlapping<TElement, TExtent>(this IReadOnlyList<TElement> tokens,
                                                                                      TExtent extent)
            where TElement : IExtent
            where TExtent : IExtent {

            int startIndex = tokens.FindIndexAtOrAfterPosition(extent.Start);
            if (startIndex < 0) {
                yield break;
            }

            // Sonderlocke für "Punktsuche"
            if (extent.Start == extent.End) {
                yield return tokens[startIndex];

                yield break;
            }

            for (int index = startIndex; index < tokens.Count; index++) {
                var token = tokens[index];
                if (token.Start >= extent.End) {
                    break;
                }

                yield return token;
            }
        }

        static IEnumerable<TElement> GetElementsInside<TElement, TExtent>(this IReadOnlyList<TElement> tokens,
                                                                          TExtent extent)
            where TElement : IExtent
            where TExtent : IExtent {

            int startIndex = tokens.FindIndexAtOrAfterPosition(extent.Start);
            if (startIndex < 0) {
                yield break;
            }

            for (int index = startIndex; index < tokens.Count; index++) {
                var token = tokens[index];

                // TODO Wie kann das sein, dass FindIndexAtOrAfterPosition ein ISymbol findet,
                // das vor extent.Start liegt? Hängt das damit zusammen, dass Symbole
                // nicht lückenlos aneinander liegen?
                if (token.Start < extent.Start) {
                    continue;
                }

                if (token.End > extent.End) {
                    break;
                }

                yield return token;
            }
        }

        public static T FindElementAtPosition<T>(this IReadOnlyList<T> tokens, int position, bool defaultIfNotFound = false) where T : IExtent {

            int index = tokens.FindIndexAtOrAfterPosition(position);
            if (index < 0) {
                if (defaultIfNotFound) {
                    return default;
                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var token = tokens[index];
            if (token.Start <= position && token.End > position) {
                return token;
            }

            if (defaultIfNotFound) {
                return default;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// Findet den Index des ersten Tokens, dessen Start größer oder gleich der angegebenen Position ist. 
        /// </summary>
        public static int FindIndexAtOrAfterPosition<T>(this IReadOnlyList<T> tokens, int pos) where T : IExtent {
            // TODO Sollte bei pos < 0 eine Ausnahme geworfen werden?

            var index = FindIndexAtPosition(tokens, pos);
            if (index < 0) {
                index = ~index - 1;
            }

            return index;
        }

        /// <summary>
        /// Findet den Index des ersten Tokens, dessen Start gleich der angegebenen Position ist. 
        /// </summary>
        public static int FindIndexAtPosition<T>(this IReadOnlyList<T> tokens, int pos) where T : IExtent {

            int iMin = 0;
            int iMax = tokens.Count - 1;
            while (iMin <= iMax) {

                int iMid  = iMin + (iMax - iMin >> 1);
                int value = tokens[iMid].Start;

                if (value == pos) {
                    return iMid;
                }

                if (value < pos) {
                    iMin = iMid + 1;
                } else {
                    iMax = iMid - 1;
                }
            }

            return ~iMin;
        }

    }

}