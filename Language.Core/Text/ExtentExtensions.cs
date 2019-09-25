#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Language.Text {

    static class ExtentExtensions {

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