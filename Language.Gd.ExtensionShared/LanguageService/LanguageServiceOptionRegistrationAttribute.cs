using Microsoft.VisualStudio.Shell;

namespace Pharmatechnik.Language.Gd.Extension.LanguageService {

    // Registering a Legacy Language Service:
    // https://msdn.microsoft.com/en-us/library/bb166421.aspx
    abstract class LanguageServiceOptionRegistrationAttribute: RegistrationAttribute {

        public override void Register(RegistrationContext context) {
            using (Key serviceKey = context.CreateKey(KeyName)) {
                serviceKey.SetValue(OptionName, 1);
            }
        }

        public override void Unregister(RegistrationContext context) {
        }

        protected virtual string KeyName => $"Languages\\Language Services\\{GdLanguageContentDefinitions.LanguageName}";

        protected abstract string OptionName { get; }

    }

}