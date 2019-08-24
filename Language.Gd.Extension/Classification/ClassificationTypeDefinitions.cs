#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    // ReSharper disable UnassignedField.Local
    #pragma warning disable 0169
    static class ClassificationTypeDefinitions {

        //======================================
        //      Die Farben sollen derzeit nicht 
        //      anpassbar sein.
        //======================================
        static class Is {

            public const bool UserVisible = false;

        }

        #region Unknown

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Unknown)] [BaseDefinition("Syntax Error")]
        public static ClassificationTypeDefinition Unknown;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Unknown)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class UnknownClassificationFormatDefinition: ClassificationFormatDefinition {

            public UnknownClassificationFormatDefinition() {
                DisplayName = "Nav Unknown";
            }

        }

        #endregion

        #region SkipedToken

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Skiped)] [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        public static ClassificationTypeDefinition SkipedToken;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Skiped)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.High)]
        public sealed class SkipedTokenClassificationFormatDefinition: ClassificationFormatDefinition {

            public SkipedTokenClassificationFormatDefinition() {
                DisplayName       = "Gd Skiped Token";
                ForegroundOpacity = 0.5;
            }

        }

        #endregion

        #region Underline

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.Underline)] [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        public static ClassificationTypeDefinition Underline;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.Underline)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Default)]
        public sealed class UnderlineClassificationFormatDefinition: ClassificationFormatDefinition {

            public UnderlineClassificationFormatDefinition() {
                DisplayName = "Nav Underline";

                var underline = new System.Windows.TextDecoration {
                    PenThicknessUnit = System.Windows.TextDecorationUnit.FontRecommended
                };
                if (TextDecorations == null) {
                    TextDecorations = new System.Windows.TextDecorationCollection();
                }

                TextDecorations.Add(underline);
            }

        }

        #endregion

        #region StaticSymbol

        [Export(typeof(ClassificationTypeDefinition))] [Name(ClassificationTypeNames.StaticSymbol)] [BaseDefinition(ClassificationTypeNames.Identifier)]
        public static ClassificationTypeDefinition StaticSymbol;

        [Export(typeof(EditorFormatDefinition))]
        [Name(ClassificationTypeNames.StaticSymbol)]
        [UserVisible(Is.UserVisible)]
        [Order(Before = Priority.Low)]
        public sealed class StaticSymbolClassificationFormatDefinition: ClassificationFormatDefinition {

            public StaticSymbolClassificationFormatDefinition() {
                // IsItalic = true;
                IsBold = true;
            }

        }

        #endregion

        public static ImmutableDictionary<GdClassification, IClassificationType> GetSyntaxTokenClassificationMap(IClassificationTypeRegistryService registry) {

            // TODO Mapping...
            var classificationMap = new Dictionary<GdClassification, IClassificationType> {
                {GdClassification.Comment, registry.GetClassificationType(ClassificationTypeNames.Comment)},
                {GdClassification.Keyword, registry.GetClassificationType(ClassificationTypeNames.Keyword)},
                {GdClassification.Identifier, registry.GetClassificationType(ClassificationTypeNames.Identifier)},
                {GdClassification.Punctuation, registry.GetClassificationType(ClassificationTypeNames.Punctuation)},
                {GdClassification.StringLiteral, registry.GetClassificationType(ClassificationTypeNames.StringLiteral)},
                {GdClassification.ClassName, registry.GetClassificationType(ClassificationTypeNames.ClassName)},
                {GdClassification.NumericLiteral, registry.GetClassificationType(ClassificationTypeNames.NumericLiteral)},
                {GdClassification.EventName, registry.GetClassificationType(ClassificationTypeNames.EventName)},
                {GdClassification.MethodName, registry.GetClassificationType(ClassificationTypeNames.MethodName)},
                {GdClassification.Skiped, registry.GetClassificationType(ClassificationTypeNames.Skiped)},
                {GdClassification.Operator, registry.GetClassificationType(ClassificationTypeNames.Operator)},
                {GdClassification.Text, registry.GetClassificationType(ClassificationTypeNames.Text)},
                {GdClassification.WhiteSpace, registry.GetClassificationType(ClassificationTypeNames.WhiteSpace)},
                {GdClassification.StaticSymbol, registry.GetClassificationType(ClassificationTypeNames.StaticSymbol)},
                {GdClassification.VerbatimStringLiteral, registry.GetClassificationType(ClassificationTypeNames.VerbatimStringLiteral)},
                {GdClassification.PropertyName, registry.GetClassificationType(ClassificationTypeNames.PropertyName)},
                {GdClassification.NamespaceName, registry.GetClassificationType(ClassificationTypeNames.NamespaceName)},
                {GdClassification.ConstantName, registry.GetClassificationType(ClassificationTypeNames.ConstantName)},
                {GdClassification.Unknown, registry.GetClassificationType(ClassificationTypeNames.Unknown)},

            };

            return classificationMap.ToImmutableDictionary();
        }

    }

}