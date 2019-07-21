lexer grammar GdTokens;

channels {
    TriviaChannel
}


// Wegen Equals Token
@header {
    #pragma warning disable 0108
}

Using             : 'USING';
Namespace         : 'NAMESPACE';
Form              : 'FORM';
Panel             : 'PANEL';
DetailsPanel      : 'DETAILSPANEL';
BarManager        : 'BARMANAGER';
TabNavigation     : 'TABNAVIGATION';
Tabs              : 'TABS';
TabPage           : 'TABPAGE';
MultiView         : 'MULTIVIEW';
User              : 'USER';
Dialog            : 'DIALOG';
Controls          : 'CONTROLS';
NonVisualControls : 'NONVISUALCONTROLS';
Control           : 'CONTROL';
Layout            : 'LAYOUT';
LayoutInfo        : 'LAYOUTINFO';
Properties        : 'PROPERTIES';
Events            : 'EVENTS';
Binding           : 'BINDING';
End               : 'END';
True              : 'true';
False             : 'false';
Template          : 'Template';
Hotkeys           : 'HOTKEYS';
Hotkey            : 'HOTKEY';
SharedControl     : 'SHAREDCONTROL';
ContextMenu       : 'CONTEXTMENU';


NewLineTrivia
    : NL
    -> channel(TriviaChannel)
    ;


WhitespaceTrivia
    : WS+
    -> channel(TriviaChannel)
    ;


SingleLineCommentTrivia
    : '//' .*? (NL | EOF)
    -> channel(TriviaChannel)
    ;


MultiLineCommentTrivia
    : '/*' .*? '*/'
    -> channel(TriviaChannel)
    ;


Identifier
    : IdentifierStartCharacter IdentifierPartCharacter*
    ;

Character
    : '\'' Char '\''
    ;

String
    :   RegularStringLiteral
    |   VerbatimStringLiteral
    ;

Integer
    : ('+'|'-')? DecimalDigit+
    ;

OpenBrace
    :   '{'
    ;

CloseBrace
    :   '}'
    ;

OpenBracket
    :   '['
    ;

CloseBracket
    :   ']'
    ;

OpenParen
    :   '('
    ;

CloseParen
    :   ')'
    ;

Equals
    :   '='
    ;

EqualsEquals
    :   '=='
    ;

Comma
    :   ','
    ;

Colon
    :   ':'
    ;

SemicolonTrivia
    : ';'
    -> channel(TriviaChannel)
    ;

Hash
    : '#'
    ;

Quote
    : '\"'
    ;

Questionmark
    : '?'
    ;

Dot
    : '.'
    ;

PlusEquals
    : '+='
    ;

MinusEquals
    : '-='
    ;

PlusCtrl
    : '+CTRL'
    ;

MinusCtrl
    : '-CTRL'
    ;

PlusAlt
    : '+ALT'
    ;

MinusAlt
    : '-ALT'
    ;

PlusShift
    : '+SHIFT'
    ;

MinusShift
    : '-SHIFT'
    ;

//------------------
// Fragments
//
fragment NL
    :   '\r\n' | '\r' | '\n'
    |   '\u0085'      // <Next Line CHARACTER (U+0085)>'
    |   '\u2028'      // '<Line Separator CHARACTER (U+2028)>'
    |   '\u2029'      // '<Paragraph Separator CHARACTER (U+2029)>'
    ;

fragment WS
    :   UnicodeClassZS //'<Any Character With Unicode Class Zs>'
    |   '\u0009'       //'<Horizontal Tab Character (U+0009)>'
    |   '\u000B'       //'<Vertical Tab Character (U+000B)>'
    |   '\u000C'       //'<Form Feed Character (U+000C)>'
    ;


fragment UnicodeClassZS
    :   '\u0020'      // SPACE
    |   '\u00A0'      // NO_BREAK SPACE
    |   '\u1680'      // OGHAM SPACE MARK
    |   '\u180E'      // MONGOLIAN VOWEL SEPARATOR
    |   '\u2000'      // EN QUAD
    |   '\u2001'      // EM QUAD
    |   '\u2002'      // EN SPACE
    |   '\u2003'      // EM SPACE
    |   '\u2004'      // THREE_PER_EM SPACE
    |   '\u2005'      // FOUR_PER_EM SPACE
    |   '\u2006'      // SIX_PER_EM SPACE
    |   '\u2008'      // PUNCTUATION SPACE
    |   '\u2009'      // THIN SPACE
    |   '\u200A'      // HAIR SPACE
    |   '\u202F'      // NARROW NO_BREAK SPACE
    |   '\u3000'      // IDEOGRAPHIC SPACE
    |   '\u205F'      // MEDIUM MATHEMATICAL SPACE
    ;


fragment IdentifierStartCharacter
    :   ('a'..'z'|'A'..'Z'|'_'|'\u00c4'|'\u00e4'|'\u00DC'|'\u00FC'|'\u00D6'|'\u00F6'|'\u00DF' )
    ;


fragment IdentifierPartCharacter
    :   ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$'|'\u00c4'|'\u00e4'|'\u00DC'|'\u00FC'|'\u00D6'|'\u00F6'|'\u00DF')
    ;


fragment DecimalDigit
    :   '0' |   '1' |   '2' |   '3' |   '4' |   '5' |   '6' |   '7' |   '8' |   '9'
    ;


fragment Char
    :   SingleCharacter
    |   SimpleEscapeSequence
    ;


fragment SingleCharacter
    :   ~( '\'' | '\\' | '\u000D' | '\u000A' | '\u2028' | '\u2029')
    ;


fragment SimpleEscapeSequence
    :   '\\' // Wird immer mit einem Backslash eingeleitet, gefolgt von
        (   '\'' // Hochkomma
        |   '\"' // Anführungsstriche
        |   '\\' // Backslash
        |   '0'
        |   'a'
        |   'b'
        |   'f'
        |   'n'
        |   'r'
        |   't'
        |   'v'
        )
    ;


// strings
fragment RegularStringLiteral
    :   '\"'   RegularStringLiteralCharacter* '\"'
    ;


fragment RegularStringLiteralCharacter
    :   SingleRegularStringLiteralCharacter
    |   SimpleEscapeSequence
    ;


fragment SingleRegularStringLiteralCharacter
    :   ~( '\"' | '\\' | '\u000D' | '\u000A' | '\u2028' | '\u2029')
    ;


// TODO Überprüfen
fragment VerbatimStringLiteral
    :   '@' '"'
        (   '\"\"'
        |   '\\'
        |   ~('"' | '\\')
        )*
        '"'
        ;


SkipedTokenTrivia
    :  .
    -> channel(TriviaChannel)
    ;