#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension
{
    sealed class GdLanguageContentDefinitions
    {
        public const string ContentType   = "Gd";
        public const string LanguageName  = "Gd";
        public const string FileExtension = ".gd";

        [Export]
        [Name(ContentType)]
        [BaseDefinition("code")]
        internal ContentTypeDefinition GuiModelContentTypeDefinition = null;

        [Export]
        [ContentType(ContentType)]
        [FileExtension(FileExtension)]
        internal FileExtensionToContentTypeDefinition GuiModelFileExtensionDefinition = null;
    }
}
