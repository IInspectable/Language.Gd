#region Using Directives

using System.Threading;

using Antlr4.StringTemplate;

using Tool.GdSyntaxGenerator.Models;
using Tool.GdSyntaxGenerator.Templates;

#endregion

namespace Tool.GdSyntaxGenerator {

    class CodeGeneratorOptions {

    }

    class CodeGeneratorContext {

    }

    // ReSharper disable InconsistentNaming
    class CodeGenerator {

        const string TemplateBeginName    = "Begin";
        const string ModelAttributeName   = "model";
        const string ContextAttributeName = "context";

        static readonly ThreadLocal<TemplateGroup> SyntaxKindEnumTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxKindEnumTemplate));

        public static string GenerateSyntaxKindEnum(SyntaxKindEnumModel model, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxKindEnumTemplateGroup.Value, model, context);
            var content  = template.Render();

            return content;
        }

        static readonly ThreadLocal<TemplateGroup> SyntaxSlotTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxSlotTemplate));

        public static string GenerateSyntaxSlot(SlotModels models, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxSlotTemplateGroup.Value, models, context);
            var content  = template.Render();

            return content;
        }

        static readonly ThreadLocal<TemplateGroup> SyntaxNodeTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxNodeTemplate));

        public static string GenerateSyntaxNode(SlotModels models, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxNodeTemplateGroup.Value, models, context);
            var content  = template.Render();

            return content;
        }

        static readonly ThreadLocal<TemplateGroup> SyntaxSlotBuilderGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxSlotBuilderTemplate));

        public static string GenerateSyntaxSlotBuilder(SlotModels models, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxSlotBuilderGroup.Value, models, context);
            var content  = template.Render();

            return content;
        }

        static readonly ThreadLocal<TemplateGroup> SyntaxVisitorGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxVisitorTemplate));

        public static string GenerateSyntaxVisitor(SlotModels models, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxVisitorGroup.Value, models, context);
            var content  = template.Render();

            return content;
        }

        static readonly ThreadLocal<TemplateGroup> SyntaxFactoryGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.SyntaxFactoryTemplate));

        public static string GenerateSyntaxFactory(SlotModels models, CodeGeneratorContext context) {

            var template = GetTemplate(SyntaxFactoryGroup.Value, models, context);
            var content  = template.Render();

            return content;
        }

        static TemplateGroup LoadTemplateGroup(string resourceName) {

            var commonTemplate = new TemplateGroupString(Resources.CommonTemplate);
            var templateGroup  = new TemplateGroupString(resourceName);

            templateGroup.ImportTemplates(commonTemplate);

            return templateGroup;
        }

        static Template GetTemplate(TemplateGroup templateGroup, object model, CodeGeneratorContext context) {

            var st = templateGroup.GetInstanceOf(TemplateBeginName);

            st.Add(ModelAttributeName,   model);
            st.Add(ContextAttributeName, context);

            return st;
        }

    }

}