namespace Pharmatechnik.Language.Gd {

    public class ValueSuggestion {

        public ValueSuggestion(string suggestionValue, SuggestionType suggestionType) {
            SuggestionValue = suggestionValue;
            SuggestionType  = suggestionType;

        }

        public string         SuggestionValue { get; }
        public SuggestionType SuggestionType  { get; }

    }

}