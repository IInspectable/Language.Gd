#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public static class ClassifiedTextExtensions {

        public static string JoinText(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(new StringBuilder(), (sb, p) => sb.Append(p.Text), sb => sb.ToString());
        }

        public static int Length(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(0, (acc, ct) => acc + ct.Text.Length);
        }

        public static ImmutableArray<ClassifiedText> ToClassifiedText(this string text) {
            return ToClassifiedText(text, GdClassification.Text);
        }

        public static ImmutableArray<ClassifiedText> ToClassifiedText(this string text, GdClassification classification) {
            return ImmutableArray.Create(new ClassifiedText(text, classification));
        }

    }

}