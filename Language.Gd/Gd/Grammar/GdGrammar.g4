/*
========================
Grammar for Gd Language
========================

Konventionen
==================
Sektionen
  Sektionen enden immer mit dem Namen *Section, und haben als Kinder eine *SectionBegin und *SectionEnd Regel
  Derartige Regeln respektive deren generierte Syntaxen implementieren dann die ISection Schnittstelle

Regeln
  Eine Regel besteht entweder nur aus einer einzigen Alternative, wobei diese Alternative wiederum aus Regeln und Terminals bestehen kann,
  oder eine Regel besteht aus mehreren Alternativen, wobei jede Alternative dann eweils nur eine Regelreferenz darstellt. 
  Dieser Fall (mehrere Alternativen) wird auf eine Vererbungshierarchie abgebildet, wobei die alternativen dfinierende 
  Regel zur Basisklasse wird.

  Bsp.:
  container  // <- Wird zur abstrakten Basisklasse ContainerSyntax
    :   formSection        // <- wird zu FormSectionSyntax und erbt von ContainerSyntax
    |   dialogSection      // <- wird zu DialogSectionSyntax und erbt von ContainerSyntax
    |   userControlSection // <- wird zu UserControlSectionSyntax und erbt von ContainerSyntax
    ;

*/

parser grammar GdGrammar;

options
{ tokenVocab = GdTokens; }

// Wegen Equals Token
@header {
    #pragma warning disable 0108
}

guiDescription 
    :   usingDeclarationSection* 
        namespaceDeclarationSection 
        EOF
    ;

usingDeclarationSection
    :   usingDeclarationSectionBegin 
            qualifiedName 
        usingDeclarationSectionEnd
    ;

usingDeclarationSectionBegin
    : Using
    ;

usingDeclarationSectionEnd
    : End Using
    ;

namespaceDeclarationSection 
    :   namespaceDeclarationSectionBegin
            container+ 
        namespaceDeclarationSectionEnd
    ;

namespaceDeclarationSectionBegin
    :   Namespace qualifiedName
    ;


namespaceDeclarationSectionEnd
    :   End Namespace
    ;


container 
    :   formSection
    |   dialogSection
    |   userControlSection
    ;


formSection 
    :   formSectionBegin
            containerDeclaration
        formSectionEnd       
    ;

formSectionBegin
    :   Form Identifier
    ;

formSectionEnd
    :   End Form Identifier?   
    ;


dialogSection 
    :   dialogSectionBegin
            containerDeclaration
        dialogSectionEnd
    ;

dialogSectionBegin
    :   Dialog Identifier
    ;

dialogSectionEnd
    :   End Dialog Identifier?
    ;

userControlSection 
    :   userControlSectionBegin 
            containerDeclaration
        userControlSectionEnd
    ;

userControlSectionBegin 
    : User Control Identifier
    ;

userControlSectionEnd 
    : End User Control Identifier? 
    ;

containerDeclaration 
    :   template? 
        layoutSection? 
        propertiesSection? 
        hotkeysSection? 
        controlsSection? 
        nonVisualControlsSection?
    ;


layoutSection 
    :   layoutSectionBegin 
            property* 
        layoutSectionEnd
    ;

layoutSectionBegin
    :   Layout Identifier
    ;

layoutSectionEnd
    :    End Layout
    ;

layoutInfoSection
    :   layoutInfoSectionBegin 
            property* 
        layoutInfoSectionEnd
    ;

layoutInfoSectionBegin
    : LayoutInfo
    ;

layoutInfoSectionEnd
    : End LayoutInfo
    ;

eventsSection
    :   eventsSectionBegin 
            eventDeclaration+ 
        eventsSectionEnd
    ;

eventsSectionBegin
    : Events
    ;

eventsSectionEnd
    : End Events
    ;

eventDeclaration
    : EventName=Identifier PlusEquals CallName=Identifier CallType=Identifier?
    ;


propertiesSection
    :   propertiesSectionBegin 
            property* 
        propertiesSectionEnd
    ;

propertiesSectionBegin
    :  Properties
    ;

propertiesSectionEnd
    :  End Properties
    ;

hotkeysSection 
    :   hotkeysSectionBegin 
            hotkeyDeclaration+
        hotkeysSectionEnd
    ;

hotkeysSectionBegin
    :   Hotkeys
    ;

hotkeysSectionEnd
    :   End Hotkeys
    ;
	   

hotkeyDeclaration
    :   Hotkey HotKeyName=Identifier modifierOption* hotkeyAssignement CallName=Identifier CallType=Identifier?
    ;

hotkeyAssignement
    :   plusEqualsHotkeyAssignement
    |   minusEqualsHotkeyAssignement
    ;

plusEqualsHotkeyAssignement
    : PlusEquals
    ;

minusEqualsHotkeyAssignement
    : MinusEquals
    ;
       
controlsSection 
    :   controlsSectionBegin 
            guiElement* 
        controlsSectionEnd
    ;

controlsSectionBegin
    :   Controls
    ;

controlsSectionEnd
    :   End Controls
    ;

guiElement
    :   panelSection 
    |   detailsPanelSection
    |   controlSection 
    |   barManagerSection
    |   tabNavigationSection
    |   multiViewSection
    ;


panelSection
    :   panelSectionBegin 
            layoutSection? 
            layoutInfoSection?
            propertiesSection?
            controlsSection?
        panelSectionEnd
    ;


panelSectionBegin
    : Panel Identifier template? 
    ;

panelSectionEnd
    : End Panel Identifier?
    ;

detailsPanelSection
    :   detailsPanelSectionBegin
            layoutSection? 
            layoutInfoSection? 
            propertiesSection? 
            controlsSection? 
        detailsPanelSectionEnd
    ;

detailsPanelSectionBegin
    :   DetailsPanel Identifier template?
    ;

detailsPanelSectionEnd
    :   End DetailsPanel Identifier?
    ;

controlSection
    :   controlSectionBegin 
            layoutInfoSection? 
            propertiesSection? 
            eventsSection? 
            hotkeysSection?
            contextMenuSection? 
            bindingSection? 
        controlSectionEnd
    ;     
            

controlSectionBegin
    : ControlKeyword=Control ControlType=Identifier ControlIdentifier=Identifier template?
    ;

controlSectionEnd
    : EndKeyword=End ControlKeyword=Control ControlIdentifier=Identifier?
    ;

barManagerSection
    :   barManagerSectionBegin 
            propertiesSection? 
            controlsSection? 
        barManagerSectionEnd
    ;
      

barManagerSectionBegin
    :   BarManager Identifier
    ;

barManagerSectionEnd
    :   End BarManager Identifier?
    ;

tabNavigationSection
    :   tabNavigationSectionBegin 
            propertiesSection? 
            eventsSection?
            hotkeysSection? 
            contextMenuSection?  
            tabsSection? 
            sharedControlSection? 
        tabNavigationSectionEnd
      ;

tabNavigationSectionBegin
    :  TabNavigation Identifier
    ;

tabNavigationSectionEnd
    :  End TabNavigation Identifier?
    ;

tabsSection
    :   tabsSectionBegin 
            tabPageSection* 
        tabsSectionEnd
      ;
      
tabsSectionBegin
    :   Tabs
    ;

tabsSectionEnd
    :   End Tabs
    ;

tabPageSection
    :   tabPageSectionBegin
            propertiesSection? 
            eventsSection? 
            contextMenuSection? 
            controlsSection? 
        tabPageSectionEnd
    ;

tabPageSectionBegin
    :   TabPage Identifier template? 
    ;

tabPageSectionEnd
    :   End TabPage Identifier?
    ;


multiViewSection
    :   multiViewSectionBegin 
            layoutInfoSection? 
            propertiesSection? 
            eventsSection? 
            hotkeysSection? 
            contextMenuSection? 
            userControlsSection?
        multiViewSectionEnd
    ;


multiViewSectionBegin
    :  MultiView Identifier template?;


multiViewSectionEnd
    :  End MultiView Identifier?;


sharedControlSection
    :   sharedControlSectionBegin
            controlSection* 
        sharedControlSectionEnd
    ;

sharedControlSectionBegin
    : SharedControl
    ;

sharedControlSectionEnd
    : End SharedControl
    ;


nonVisualControlsSection
    :   nonVisualControlsSectionBegin 
            controlSection* 
        nonVisualControlsSectionEnd
    ;

nonVisualControlsSectionBegin
    :   NonVisualControls
    ;

nonVisualControlsSectionEnd
    :   End NonVisualControls
    ;

userControlsSection 
    :   userControlsSectionBegin 
            controlSection* 
        userControlsSectionEnd
    ;

userControlsSectionBegin
    :   Controls
    ;

userControlsSectionEnd
    :   End Controls
    ;

contextMenuSection
    :   contextMenuSectionBegin 
            property* 
        contextMenuSectionEnd
    ;

contextMenuSectionBegin
    : ContextMenu
    ;

contextMenuSectionEnd
    : End ContextMenu
    ;

bindingSection
    :   bindingSectionBegin
        bindingSectionEnd
    ;

bindingSectionBegin
    : Binding
    ;

bindingSectionEnd
    : End Binding
    ;

modifierOption
    : Comma modifier
    ;

modifier
    : plusCtrlModifier 
    | minusCtrlModifier 
    | plusAltModifier 
    | minusAltModifier 
    | plusShiftModifier 
    | minusShiftModifier
    ;

plusCtrlModifier
    : PlusCtrl
    ;

minusCtrlModifier
    : MinusCtrl
    ;

plusAltModifier
    : PlusAlt
    ;

minusAltModifier
    : MinusAlt
    ;

plusShiftModifier
    : PlusShift
    ;

minusShiftModifier
    : MinusShift
    ;
property 
    : propertyAssign
    | propertyAddAssign
    ;
    
    propertyAssign
    : lvalueExpression Equals rvalue
    ;

    // TODO rvalues in rules und damit in Syntaxen umwandeln
rvalue
    : identifierValue
    | integerValue
    | stringValue
    | characterValue
    | trueValue
    | falseValue           
    ;

identifierValue
    : Identifier
    ;

integerValue
    : Integer
    ;

stringValue
    : String
    ;

characterValue
    : Character
    ;

trueValue
    : True
    ;

falseValue
    : False
    ;


propertyAddAssign
    :   lvalueExpression PlusEquals CallName=Identifier CallType=Identifier?
    ;


lvalueExpression 
    : memberAccessExpression lvalueExpressionContinuation?
    ;

lvalueExpressionContinuation
    : Dot lvalueExpression
    ;

memberAccessExpression
    : simpleMemberAccessExpression 
    | elementAccessExpression
    ;


simpleMemberAccessExpression
    : identifierName
    ;


elementAccessExpression
    : identifierName OpenBracket Integer CloseBracket
    ;
    

template 
    : Template Equals qualifiedName
    ;


identifierName
    : Identifier
    ;


qualifiedName 
    : identifierName qualifiedNameContinuation?
    ;

qualifiedNameContinuation
    : Dot qualifiedName
    ;