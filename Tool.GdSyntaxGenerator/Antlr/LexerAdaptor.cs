using System;
using System.Reflection;
namespace Tool.GdSyntaxGenerator.Grammar
{
    using System.IO;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

#pragma warning disable CA1012 // Abstract types should not have constructors
    public abstract class LexerAdaptor : Lexer
#pragma warning restore CA1012 // But Lexer demands it - old 
    {
        // I copy a reference to the stream, so It can be used as a Char Stream, not as a IISStream
        readonly ICharStream stream;
        // Tokens are read only so I hack my way
        readonly FieldInfo tokenInput = typeof(CommonToken).GetField("type", BindingFlags.NonPublic | BindingFlags.Instance);
        protected LexerAdaptor(ICharStream input)
             : base(input)
        {
            stream = input;
        }

        /**
         * Track whether we are inside of a rule and whether it is lexical parser. _currentRuleType==TokenConstants.InvalidType
         * means that we are outside of a rule. At the first sign of a rule name reference and _currentRuleType==invalid, we
         * can assume that we are starting a parser rule. Similarly, seeing a token reference when not already in rule means
         * starting a token rule. The terminating ';' of a rule, flips this back to invalid type.
         *
         * This is not perfect logic but works. For example, "grammar T;" means that we start and stop a lexical rule for
         * the "T;". Dangerous but works.
         *
         * The whole point of this state information is to distinguish between [..arg actions..] and [charsets]. Char sets
         * can only occur in lexical rules and arg actions cannot occur.
         */
        private int CurrentRuleType { get; set; } = TokenConstants.InvalidType;

        protected void handleBeginArgument()
        {
            if (InLexerRule)
            {
                PushMode(AntlrV4Tokens.LexerCharSet);
                More();
            }
            else
            {
                PushMode(AntlrV4Tokens.Argument);
            }
        }

        protected void handleEndArgument()
        {
            PopMode();
            if (_modeStack.Count > 0)
            {
                CurrentRuleType = (AntlrV4Tokens.ARGUMENT_CONTENT);
            }
        }

        protected void handleEndAction()
        {
            PopMode();
            if (_modeStack.Count > 0)
            {
                CurrentRuleType = (AntlrV4Tokens.ACTION_CONTENT);
            }
        }


        public override IToken NextToken()
        {
            var token = base.NextToken();
            if (Type == AntlrV4Tokens.ID)
            {
                char firstChar = stream.GetText(Interval.Of(_tokenStartCharIndex, _tokenStartCharIndex))[0];
                if (char.IsUpper(firstChar))
                {
                    Type = AntlrV4Tokens.TOKEN_REF;
                    tokenInput.SetValue(token, AntlrV4Tokens.TOKEN_REF);
                }
                if (char.IsLower(firstChar))
                {

                    Type = AntlrV4Tokens.RULE_REF;
                   // token.Type=
                    tokenInput.SetValue(token, AntlrV4Tokens.RULE_REF);
                }

                if (CurrentRuleType == TokenConstants.InvalidType)
                { // if outside of rule def
                    CurrentRuleType = Type; // set to inside lexer or parser rule
                }
            }
            else if (Type == AntlrV4Tokens.SEMI)
            { // exit rule def
                CurrentRuleType = TokenConstants.InvalidType;
            }

            return token;
        }

        private bool InLexerRule => CurrentRuleType == AntlrV4Tokens.TOKEN_REF;


        private bool InParserRule => CurrentRuleType == AntlrV4Tokens.RULE_REF;

    }
}
